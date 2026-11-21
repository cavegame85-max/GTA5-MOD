# OMNIA MouseFix

ASI plugin for classic GTA San Andreas PC 1.0 and 1.01 (32-bit).

## What this build changes

The plugin hooks DirectInput 8 initialization and the system mouse device. If the game asks for mouse state or buffered mouse events while DirectInput reports `DIERR_INPUTLOST` or `DIERR_NOTACQUIRED`, MouseFix attempts to reacquire the device and retries that read once.

This is an input-recovery patch. It does not replace the game's controls, synthesize mouse movement, or change keyboard bindings.

## Requirements

- Classic 32-bit GTA San Andreas PC.
- A compatible ASI Loader already installed.
- Windows x86/x64.

The archive does not include an ASI Loader or game files.

## Install

1. Close GTA San Andreas.
2. Back up the game folder.
3. Extract `OMNIA_MouseFix.asi` into the folder containing `gta_sa.exe` and/or `gta_sa_1.01.exe`.
4. Start the game normally.
5. To uninstall, close the game and remove `OMNIA_MouseFix.asi`.

## Compatibility note

This plugin addresses DirectInput device-lost recovery. It cannot guarantee compatibility with every third-party input wrapper or every mouse driver. Runtime testing on both game executables is required before calling a release fully verified.
