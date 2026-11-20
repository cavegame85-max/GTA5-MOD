# GTA5-MOD — OMNIA Mod Manager

An in-game mod catalogue and control panel for **Grand Theft Auto V Legacy — Story Mode**.

## Current milestone: 0.1 foundation

This repository starts with the OMNIA in-game Mod Manager. The first build focuses on:
- an in-game overlay opened with **F10**;
- scanning common GTA V Legacy mod locations;
- showing detected ASI plugins and ScriptHookVDotNet scripts;
- identifying the OpenIV `mods` folder;
- recording requested states for future integration with the OMNIA desktop installer.

### Important limitation

GTA V and its plugin loaders do not expose one universal API that reports every installed or currently-running mod. The catalogue therefore reports files it can detect in known locations. It cannot reliably identify every mod embedded inside RPF archives or prove that an arbitrary plugin is currently active.

Changing an external ASI/.NET plugin while GTA V is already running is not generally safe. This first in-game build records the requested state and marks external changes as requiring a restart and a future OMNIA desktop/launcher component. It does not pretend to unload arbitrary third-party plugins in-place.

## Target

- GTA V **Legacy**
- Windows 10/11
- Story Mode only
- ScriptHookV + ScriptHookVDotNet 3 runtime installed separately

## Build

The GitHub Actions workflow builds the .NET Framework 4.8 plugin and creates `OMNIA-GTA5-ModManager.zip`.

Open **Actions → Build OMNIA Mod Manager → Run workflow**, or push a change to `main`.

The ZIP contains only OMNIA's own plugin and its configuration/readme. Third-party loaders are not bundled.

## Install

1. Install the compatible ScriptHookV and ScriptHookVDotNet runtime separately, following their official instructions.
2. Extract the OMNIA ZIP into the GTA V Legacy folder containing `GTA5.exe`.
3. Start Story Mode and press **F10**.
4. Use **Up/Down** to select a detected entry, **Enter** to request a state change, and **F10/Esc** to close.

See [docs/INSTALLATION.md](docs/INSTALLATION.md).

## Repository layout

- `src/OMNIA.GamePlugin` — in-game ScriptHookVDotNet plugin
- `docs` — installation and architecture notes
- `.github/workflows` — Windows CI and ZIP packaging

## Safety

Use modded files in Story Mode only. Disable/remove third-party modifications before entering GTA Online. Back up the game folder before installing mods. OMNIA does not bypass anti-cheat or interact with GTA Online.

This is an independent fan project and is not affiliated with Rockstar Games or Take-Two Interactive.
