# NutStaller

A launcher and installer for [reNut](https://github.com/masterspike52/reNut), the PC recompilation of
Banjo-Kazooie: Nuts & Bolts.

![NutStaller](NutStaller/Assets/bg.png)

## What it does

- Downloads the latest reNut release and the latest extract-xiso build
- Extracts your Banjo-Kazooie: Nuts & Bolts (US) ISO with live progress
- Writes `renut.cfg` so the in-game path wizard never shows up
- Edits keybinds in `renut.toml`
- Checks for reNut updates
- Workshop: browse community vehicle blueprints in a grid, download them into your save or remove them again, and share your own (through [nutstaller-workshop](https://github.com/etonedemid/nutstaller-workshop))
- Works with mouse or a gamepad (dpad moves, A selects, LB/RB switch page, Start plays)
- Runs on Windows, Linux and Apple Silicon macOS (Linux uses the reNut AppImage; Mac uses a native app bundle)

## Usage

1. Run `NutStaller.exe` (Windows) or `NutStaller-x86_64.AppImage` (Linux)
2. Pick an install folder
3. Hit "Do everything" and select your ISO when asked
4. Hit "Play"

Drop an ISO into the install folder beforehand and step 3 picks it up automatically.

## Building

```
dotnet publish -p:PublishProfile=win-x64
dotnet publish -p:PublishProfile=linux-x64
```

Each produces a single self-contained executable under `bin/publish/<rid>/`. Requires the .NET 8 SDK
or newer. The UI is Avalonia, so either build can be made from any OS.

For Linux, `./build-appimage.sh` packages it as `NutStaller/bin/NutStaller-x86_64.AppImage`.
On Linux the launcher keeps `nutstaller.ini` and its default `reNut` folder next to the AppImage.

### Apple Silicon macOS

Run `./build-macos.sh` with an ARM64 .NET 8 SDK to create
`NutStaller/bin/NutStaller.app` and `NutStaller-macos-arm64.zip`. The app is
self-contained and includes SDL2 for controller navigation when Homebrew SDL2
is available at build time. `DOTNET` can select a project-local SDK executable.

The launcher stores its state and default installation in
`~/Library/Application Support/NutStaller`, and reNut config/saves in
`~/Library/Application Support/renut`. Its Mac release feed defaults to
`ricjo4w/reNut`; set `NUTSTALLER_RENUT_REPO=owner/repo` to use another fork.
Only Mac ARM64 or universal ZIP assets are accepted on Apple Silicon. Packages
must contain `reNut.app/Contents/MacOS/renut`. The fork must publish a compatible
release before download/update can install it; the launcher does not make an
unported game release Mac-compatible.

Run `dotnet run --project tests/PlatformChecks -c Release` for release-selection
and app-bundle extraction checks. To also check a real signed release ZIP,
pass its absolute path after `--`; this verifies extraction, executable modes
and the extracted app's signature on macOS. The app uses an ad-hoc development signature;
public distribution requires Developer ID signing and notarization.

### Linux compatibility

The launcher itself needs glibc 2.17 or newer (no .NET, ICU or SDL needed on the host; SDL2 only
enables gamepad support). Tested on Ubuntu 20.04 and 24.04. What it downloads has its own floor:
the extract-xiso Linux build needs glibc 2.34 and the reNut AppImage needs glibc 2.38
(Ubuntu 24.04, Debian 13, Fedora 39 or newer). If reNut quits right after Play, the launcher says
so and shows the system's glibc version.

## Credits

reNut by masterspike52 and contributors. extract-xiso by the XboxDev team.
Banjo-Kazooie: Nuts & Bolts by Rare. See the Credits page in the app for the full list.
