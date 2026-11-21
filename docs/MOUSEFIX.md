# OMNIA MouseFix

OMNIA MouseFix is a separate ASI module for classic GTA San Andreas PC. The first CI milestone is a **diagnostic bootstrap**, not yet a mouse-input patch.

## Supported targets

- GTA San Andreas PC 1.0
- GTA San Andreas PC 1.01
- Windows 10 x64, with a 32-bit ASI loader compatible with the game

The plugin itself is built as x86 because both game executables are 32-bit. An ASI loader is required; this package does not include one.

## Current milestone: diagnostic bootstrap 0.1.0

When loaded, the plugin writes:

`%USERPROFILE%\Documents\OMNIA_MouseFix.log`

It records the foreground window, cursor position, and the state of the left, right, and middle mouse buttons once per second. It does not hook DirectInput, change game settings, or claim to fix the mouse yet. This is intentional: the log lets us distinguish Windows-level mouse input from input lost inside the game before selecting a safe patch.

## Install for diagnosis

1. Make a backup of the game folder.
2. Ensure a compatible ASI loader is already installed.
3. Copy `OMNIA_MouseFix.asi` from the archive into the GTA San Andreas game folder, next to the game executable.
4. Start only one game executable at a time.
5. Close the game and inspect `Documents\OMNIA_MouseFix.log`.

To uninstall, remove only `OMNIA_MouseFix.asi`. The log can be deleted separately.

## Verification plan

GitHub Actions performs two independent checks:
1. Build and verify that the produced plugin is a valid 32-bit PE DLL.
2. Inspect the ZIP and verify that the expected ASI and installation notes are present.

These CI checks verify the package, not actual mouse operation in GTA. Runtime compatibility and the eventual input fix must still be tested in-game on both 1.0 and 1.01.
