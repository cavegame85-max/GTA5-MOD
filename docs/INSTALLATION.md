# OMNIA GTA5 Mod Manager — installation

## Requirements

- Grand Theft Auto V **Legacy** on Windows.
- Story Mode.
- A compatible ScriptHookV installation.
- ScriptHookVDotNet 3 runtime and .NET Framework 4.8.
- Microsoft Visual C++ Redistributable (x64).

OMNIA's ZIP intentionally does not bundle ScriptHookV or ScriptHookVDotNet. Install those prerequisites from their official project pages and use matching files from one release set.

## Install OMNIA

1. Close GTA V.
2. Make a backup of the game folder or at least the files you already modified.
3. Download `OMNIA-GTA5-ModManager.zip` from the GitHub Actions artifact.
4. Extract the ZIP into the GTA V Legacy folder containing `GTA5.exe`, preserving folders.
5. Start Story Mode.
6. Press **F10** to open the OMNIA menu.

## Controls

- **F10** — open/close.
- **Up / Down** — select an entry.
- **Enter** — save a requested ON/OFF state.
- **Esc** — close.

## What the first build detects

- `.asi` files in the game root, excluding known loader components.
- `.dll`, `.cs`, and `.vb` files under `scripts`, excluding known scripting dependencies.
- The presence of the `mods` archive-modification folder.

The game does not provide a universal API for enumerating every mod. Mods embedded inside RPF archives cannot be individually enumerated by this first build.

## Important toggle limitation

The first build is a catalogue and state-request prototype. It records desired ON/OFF states in `scripts/OMNIA/mod-state.ini`, but it cannot safely unload arbitrary third-party plugins while the game is running. The menu marks these requests as restart/desktop-installer work. The next milestone will add the desktop component that can apply supported changes while GTA V is closed, with backups and rollback.

Do not interpret the displayed desired state as proof that a third-party plugin is currently loaded or disabled.

Use modifications in Story Mode only. Remove/disable third-party modifications before launching GTA Online.
