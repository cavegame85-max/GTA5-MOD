# OMNIA SA — Automatic Mod Manager + MouseFix

This package contains the OMNIA in-game CLEO menu and its ASI companion module.

## Automatic discovery

The menu does not contain a manually maintained list of mod names.

At startup, the OMNIA ASI companion scans:
- `*.asi` files in the GTA San Andreas game root.
- `cleo\*.cs` scripts in the game's CLEO folder.

It writes the detected filenames to `OMNIA_mods.txt`. When F10 opens the menu, the CLEO script reads the catalog and displays the discovered entries. The menu therefore reflects files actually present in those folders, rather than a hardcoded list.

This first discovery pass indexes ASI plugins and CLEO scripts. It does not yet identify every possible mod format (for example, loose replacement files, IMG archives, texture packs, or scripts stored in nonstandard folders).

## Package contents

- `OMNIA_MouseFix.asi` — ASI companion: mod discovery and DirectInput mouse-device recovery.
- `cleo/OMNIA_MENU.cs` — in-game menu.

## Requirements

- Classic 32-bit GTA San Andreas PC 1.0 or 1.01.
- CLEO 4 installed.
- A compatible ASI Loader installed.
- Windows x86/x64.

## Install

1. Close GTA San Andreas.
2. Back up the game folder.
3. Copy `OMNIA_MouseFix.asi` into the folder containing the game's EXE.
4. Copy `cleo/OMNIA_MENU.cs` into the game's `cleo` folder.
5. Start the game and press F10 to open or close OMNIA.

Pressing F10 a second time disables the menu drawing. The companion ASI must load for the automatic catalog file to be generated.

## Uninstall

Close the game, then remove `OMNIA_MouseFix.asi` from the game folder and `OMNIA_MENU.cs` from the `cleo` folder. You may also delete the generated `OMNIA_mods.txt` file.

## MouseFix compatibility

MouseFix attempts to reacquire the DirectInput mouse device after `DIERR_INPUTLOST` or `DIERR_NOTACQUIRED`. It does not synthesize input or replace the game's controls. Runtime behavior still requires testing in the target game build.
