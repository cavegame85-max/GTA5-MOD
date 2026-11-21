#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <cstdio>
#include <cstdint>

static HMODULE g_module = nullptr;
static volatile LONG g_running = 1;

static void write_log(FILE* file, const wchar_t* exeName) {
    POINT p{};
    const BOOL havePoint = GetCursorPos(&p);
    HWND foreground = GetForegroundWindow();
    wchar_t title[256]{};
    if (foreground) GetWindowTextW(foreground, title, 255);

    const SHORT left = GetAsyncKeyState(VK_LBUTTON);
    const SHORT right = GetAsyncKeyState(VK_RBUTTON);
    const SHORT middle = GetAsyncKeyState(VK_MBUTTON);

    std::fwprintf(file,
        L"exe=%ls | foreground=%p | title=%ls | cursor=%ls(%ld,%ld) | buttons=L:%d R:%d M:%d\n",
        exeName, foreground, title, havePoint ? L"yes" : L"no",
        p.x, p.y, (left & 0x8000) != 0, (right & 0x8000) != 0,
        (middle & 0x8000) != 0);
    std::fflush(file);
}

static DWORD WINAPI diagnostic_thread(void*) {
    wchar_t userProfile[MAX_PATH]{};
    wchar_t exePath[MAX_PATH]{};
    wchar_t exeName[MAX_PATH] = L"(unknown)";
    GetEnvironmentVariableW(L"USERPROFILE", userProfile, MAX_PATH);
    GetModuleFileNameW(nullptr, exePath, MAX_PATH);
    const wchar_t* lastSlash = wcsrchr(exePath, L'\\');
    if (lastSlash) {
        wcsncpy_s(exeName, lastSlash + 1, _TRUNCATE);
    }

    wchar_t logPath[MAX_PATH]{};
    swprintf_s(logPath, L"%ls\\Documents\\OMNIA_MouseFix.log", userProfile);
    FILE* file = nullptr;
    _wfopen_s(&file, logPath, L"wt, ccs=UTF-8");
    if (!file) return 0;

    std::fwprintf(file, L"OMNIA MouseFix diagnostic bootstrap 0.1.0\n");
    std::fwprintf(file, L"Process: %ls\n", exePath);
    std::fwprintf(file, L"Architecture: x86 ASI build\n");
    std::fwprintf(file, L"NOTE: diagnostic-only build; it does not alter game input.\n\n");
    std::fflush(file);

    while (InterlockedCompareExchange(&g_running, 1, 1)) {
        write_log(file, exeName);
        Sleep(1000);
    }
    fclose(file);
    return 0;
}

BOOL APIENTRY DllMain(HMODULE module, DWORD reason, LPVOID) {
    if (reason == DLL_PROCESS_ATTACH) {
        g_module = module;
        DisableThreadLibraryCalls(module);
        HANDLE thread = CreateThread(nullptr, 0, diagnostic_thread, nullptr, 0, nullptr);
        if (thread) CloseHandle(thread);
    } else if (reason == DLL_PROCESS_DETACH) {
        InterlockedExchange(&g_running, 0);
    }
    return TRUE;
}
