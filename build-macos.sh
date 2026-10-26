#!/bin/bash
set -euo pipefail
cd "$(dirname "$0")"
dotnet_bin="${DOTNET:-dotnet}"
"$dotnet_bin" publish NutStaller/NutStaller.csproj -p:PublishProfile=osx-arm64 --nologo
app="$PWD/NutStaller/bin/NutStaller.app"
mkdir -p "$app/Contents/MacOS" "$app/Contents/Resources"
cp -R NutStaller/bin/publish/osx-arm64/. "$app/Contents/MacOS/"
cat > "$app/Contents/Info.plist" <<'PLIST'
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0"><dict>
<key>CFBundleExecutable</key><string>NutStaller</string>
<key>CFBundleIdentifier</key><string>io.github.ricjo4w.nutstaller</string>
<key>CFBundleName</key><string>NutStaller</string>
<key>CFBundlePackageType</key><string>APPL</string>
<key>CFBundleVersion</key><string>1.0</string>
<key>CFBundleShortVersionString</key><string>1.0</string>
<key>LSMinimumSystemVersion</key><string>13.0</string>
<key>NSHighResolutionCapable</key><true/>
</dict></plist>
PLIST
# SDL is optional; include it when available for controller navigation.
if [[ -f /opt/homebrew/lib/libSDL2.dylib ]]; then
    # Homebrew libraries may be read-only; install also supports repackaging.
    install -m 755 /opt/homebrew/lib/libSDL2.dylib "$app/Contents/MacOS/libSDL2.dylib"
fi
codesign --force --deep --sign - "$app"
ditto -c -k --sequesterRsrc --keepParent "$app" NutStaller/bin/NutStaller-macos-arm64.zip
printf 'Built %s\n' "$app"
