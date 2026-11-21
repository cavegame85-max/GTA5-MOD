#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <dinput.h>
#include <cstdio>
#include <cstdint>
#include "MinHook.h"

#pragma comment(lib, "dinput8.lib")
#pragma comment(lib, "dxguid.lib")

using DirectInput8Create_t = HRESULT (WINAPI*)(HINSTANCE, DWORD, REFIID, LPVOID*, LPUNKNOWN);
using CreateDevice_t = HRESULT (STDMETHODCALLTYPE*)(LPDIRECTINPUT8A, REFGUID, LPDIRECTINPUTDEVICE8A*, LPUNKNOWN);
using Acquire_t = HRESULT (STDMETHODCALLTYPE*)(LPDIRECTINPUTDEVICE8A);
using GetDeviceState_t = HRESULT (STDMETHODCALLTYPE*)(LPDIRECTINPUTDEVICE8A, DWORD, LPVOID);
using GetDeviceData_t = HRESULT (STDMETHODCALLTYPE*)(LPDIRECTINPUTDEVICE8A, DWORD, LPDIDEVICEOBJECTDATA, LPDWORD, DWORD);

static DirectInput8Create_t g_realDirectInput8Create = nullptr;
static CreateDevice_t g_realCreateDevice = nullptr;
static Acquire_t g_realAcquire = nullptr;
static GetDeviceState_t g_realGetDeviceState = nullptr;
static GetDeviceData_t g_realGetDeviceData = nullptr;
static HMODULE g_realDinput = nullptr;
static volatile LONG g_started = 0;

static bool patch_vtable(void** slot, void* replacement, void** original) {
    DWORD oldProtect = 0;
    if (!VirtualProtect(slot, sizeof(void*), PAGE_EXECUTE_READWRITE, &oldProtect))
        return false;
    if (!*original) *original = *slot;
    *slot = replacement;
    DWORD ignored = 0;
    VirtualProtect(slot, sizeof(void*), oldProtect, &ignored);
    FlushInstructionCache(GetCurrentProcess(), slot, sizeof(void*));
    return true;
}

static HRESULT STDMETHODCALLTYPE omn_acquire(LPDIRECTINPUTDEVICE8A device) {
    return g_realAcquire ? g_realAcquire(device) : DIERR_NOTINITIALIZED;
}

static HRESULT STDMETHODCALLTYPE omn_get_state(LPDIRECTINPUTDEVICE8A device, DWORD size, LPVOID data) {
    if (!g_realGetDeviceState) return DIERR_NOTINITIALIZED;
    HRESULT hr = g_realGetDeviceState(device, size, data);
    if (hr == DIERR_INPUTLOST || hr == DIERR_NOTACQUIRED) {
        HRESULT acquireResult = g_realAcquire ? g_realAcquire(device) : DIERR_NOTINITIALIZED;
        if (SUCCEEDED(acquireResult) || acquireResult == S_FALSE)
            hr = g_realGetDeviceState(device, size, data);
    }
    return hr;
}

static HRESULT STDMETHODCALLTYPE omn_get_data(LPDIRECTINPUTDEVICE8A device, DWORD size,
    LPDIDEVICEOBJECTDATA data, LPDWORD count, DWORD flags) {
    if (!g_realGetDeviceData) return DIERR_NOTINITIALIZED;
    HRESULT hr = g_realGetDeviceData(device, size, data, count, flags);
    if (hr == DIERR_INPUTLOST || hr == DIERR_NOTACQUIRED) {
        HRESULT acquireResult = g_realAcquire ? g_realAcquire(device) : DIERR_NOTINITIALIZED;
        if (SUCCEEDED(acquireResult) || acquireResult == S_FALSE)
            hr = g_realGetDeviceData(device, size, data, count, flags);
    }
    return hr;
}

static void hook_mouse_device(LPDIRECTINPUTDEVICE8A device) {
    if (!device) return;
    void** vtable = *reinterpret_cast<void***>(device);
    // IDirectInputDevice8A: IUnknown(0..2), Acquire(7), GetDeviceState(9), GetDeviceData(10).
    patch_vtable(&vtable[7], reinterpret_cast<void*>(&omn_acquire), reinterpret_cast<void**>(&g_realAcquire));
    patch_vtable(&vtable[9], reinterpret_cast<void*>(&omn_get_state), reinterpret_cast<void**>(&g_realGetDeviceState));
    patch_vtable(&vtable[10], reinterpret_cast<void*>(&omn_get_data), reinterpret_cast<void**>(&g_realGetDeviceData));
}

static HRESULT STDMETHODCALLTYPE omn_create_device(LPDIRECTINPUT8A input, REFGUID guid,
    LPDIRECTINPUTDEVICE8A* outDevice, LPUNKNOWN outer) {
    if (!g_realCreateDevice) return DIERR_NOTINITIALIZED;
    HRESULT hr = g_realCreateDevice(input, guid, outDevice, outer);
    if (SUCCEEDED(hr) && outDevice && *outDevice) {
        if (IsEqualGUID(guid, GUID_SysMouse))
            hook_mouse_device(*outDevice);
    }
    return hr;
}

static HRESULT WINAPI omn_direct_input8_create(HINSTANCE instance, DWORD version,
    REFIID iid, LPVOID* out, LPUNKNOWN outer) {
    if (!g_realDirectInput8Create) return DIERR_NOTINITIALIZED;
    HRESULT hr = g_realDirectInput8Create(instance, version, iid, out, outer);
    if (SUCCEEDED(hr) && out && *out && IsEqualIID(iid, IID_IDirectInput8A)) {
        LPDIRECTINPUT8A input = static_cast<LPDIRECTINPUT8A>(*out);
        void** vtable = *reinterpret_cast<void***>(input);
        patch_vtable(&vtable[3], reinterpret_cast<void*>(&omn_create_device),
                     reinterpret_cast<void**>(&g_realCreateDevice));
    }
    return hr;
}


static void write_mod_catalog() {
    char gamePath[MAX_PATH]{};
    if (!GetModuleFileNameA(nullptr, gamePath, MAX_PATH)) return;

    char* slash = strrchr(gamePath, '\\');
    if (!slash) return;
    *(slash + 1) = '\0';

    char catalogPath[MAX_PATH]{};
    sprintf_s(catalogPath, "%sOMNIA_mods.txt", gamePath);
    FILE* file = nullptr;
    fopen_s(&file, catalogPath, "wb");
    if (!file) return;

    unsigned int count = 0;
    WIN32_FIND_DATAA data{};
    char pattern[MAX_PATH]{};
    sprintf_s(pattern, "%s*.asi", gamePath);
    HANDLE search = FindFirstFileA(pattern, &data);
    if (search != INVALID_HANDLE_VALUE) {
        do {
            if (!(data.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)) {
                fprintf(file, "ASI: %s\n", data.cFileName);
                ++count;
            }
        } while (FindNextFileA(search, &data));
        FindClose(search);
    }

    char cleoPath[MAX_PATH]{};
    sprintf_s(cleoPath, "%scleo\\", gamePath);
    sprintf_s(pattern, "%s*.cs", cleoPath);
    search = FindFirstFileA(pattern, &data);
    if (search != INVALID_HANDLE_VALUE) {
        do {
            if (!(data.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)) {
                fprintf(file, "CLEO: %s\n", data.cFileName);
                ++count;
            }
        } while (FindNextFileA(search, &data));
        FindClose(search);
    }

    if (count == 0) fprintf(file, "No supported ASI/CLEO mods found.\n");
    fclose(file);
}

static DWORD WINAPI initialize_hooks(void*) {
    if (InterlockedCompareExchange(&g_started, 1, 0) != 0) return 0;

    write_mod_catalog();

    wchar_t systemDir[MAX_PATH]{};
    GetSystemDirectoryW(systemDir, MAX_PATH);
    wchar_t path[MAX_PATH]{};
    swprintf_s(path, L"%ls\\dinput8.dll", systemDir);
    g_realDinput = LoadLibraryW(path);
    if (!g_realDinput) return 0;

    g_realDirectInput8Create = reinterpret_cast<DirectInput8Create_t>(
        GetProcAddress(g_realDinput, "DirectInput8Create"));
    if (!g_realDirectInput8Create) return 0;

    if (MH_Initialize() != MH_OK) return 0;
    if (MH_CreateHook(reinterpret_cast<void*>(g_realDirectInput8Create),
        reinterpret_cast<void*>(&omn_direct_input8_create),
        reinterpret_cast<void**>(&g_realDirectInput8Create)) != MH_OK) return 0;
    MH_EnableHook(reinterpret_cast<void*>(g_realDirectInput8Create));
    return 0;
}

BOOL APIENTRY DllMain(HMODULE module, DWORD reason, LPVOID) {
    if (reason == DLL_PROCESS_ATTACH) {
        DisableThreadLibraryCalls(module);
        HANDLE thread = CreateThread(nullptr, 0, initialize_hooks, nullptr, 0, nullptr);
        if (thread) CloseHandle(thread);
    }
    return TRUE;
}
