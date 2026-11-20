# GTA5-MOD — OMNIA Mod Manager

An in-game mod catalogue and control panel for **Grand Theft Auto V Legacy — Story Mode**.

## Current milestone: 0.1 foundation

The first build establishes:
- an in-game overlay opened with **F10**;
- a registry for supported OMNIA mods;
- a count of detected third-party plugin files without exposing unclassified entries;
- detection of the archive-modification folder;
- saved ON/OFF requests for future integration with the OMNIA desktop installer.

### Supported categories

The initial catalogue is limited to graphics, audio, vehicle cosmetics, maps, UI, and quality-of-life modifications. Entries must have an OMNIA `mod.ini` manifest with an approved category. Unclassified plugins are counted but are not listed or managed.

GTA V and its plugin loaders do not expose one universal API that reports every installed or currently-running mod. Content embedded inside RPF archives cannot be individually enumerated by this first build.

### Important toggle limitation

Changing an external plugin while GTA V is already running is not generally safe. This first in-game build records the requested state and marks external changes as requiring a restart and a future OMNIA desktop/launcher component. It does not pretend to unload arbitrary third-party plugins in-place.

## Target

- GTA V **Legacy**
- Windows 10/11
- Story Mode only
- ScriptHookV + a compatible ScriptHookVDotNet 3 runtime installed separately

## Build

GitHub Actions builds the .NET Framework 4.8 plugin and creates `OMNIA-GTA5-ModManager.zip`.

Open **Actions → Build OMNIA Mod Manager → Run workflow**, or push a change to `main`.

The ZIP contains only OMNIA's own plugin and configuration/readme. Third-party loaders are not bundled.

## Install

1. Install the compatible ScriptHookV and ScriptHookVDotNet runtime separately, following their official instructions.
2. Extract the OMNIA ZIP into the GTA V Legacy folder containing `GTA5.exe`.
3. Start Story Mode and press **F10**.
4. Use **Up/Down** to select an entry, **Enter** to request a state change, and **F10/Esc** to close.

See [docs/INSTALLATION.md](docs/INSTALLATION.md).

## Registering a supported mod

Place a folder under `scripts/OMNIA/Mods/<mod-id>/` and add a `mod.ini` file containing:

```ini
id=example-mod
name=Example Visual Mod
category=graphics
```

Allowed categories: `graphics`, `audio`, `vehicle-cosmetics`, `maps`, `ui`, `quality-of-life`.

## Safety

Use modded files in Story Mode only. Disable/remove third-party modifications before entering GTA Online. Back up the game folder before installing mods. OMNIA does not bypass anti-cheat or interact with GTA Online.

This is an independent fan project and is not affiliated with Rockstar Games or Take-Two Interactive.
