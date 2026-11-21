# OMNIA SA — MouseFix + in-game menu

This package contains the OMNIA in-game CLEO menu and the separate DirectInput recovery ASI module.

## Package contents

- `OMNIA_MouseFix.asi` — DirectInput device-loss recovery companion module.
- `cleo/OMNIA_MENU.cs` — OMNIA in-game control-center menu.

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
5. Start the game and press F10 to open or close the OMNIA menu.

The menu is drawn only while open; pressing F10 again disables its drawing immediately. The MouseFix entry identifies the companion ASI component; it is not a live runtime detector.

## Uninstall

Close the game, then remove `OMNIA_MouseFix.asi` from the game folder and `OMNIA_MENU.cs` from the `cleo` folder.

## Compatibility

MouseFix attempts to reacquire the DirectInput mouse device after `DIERR_INPUTLOST` or `DIERR_NOTACQUIRED`. It does not synthesize input or replace the game's controls. Runtime behavior still requires testing in the target game build.
