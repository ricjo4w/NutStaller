using System.Runtime.InteropServices;

namespace NutStaller.Core;

internal static class ReleasePlatform
{
    // Apple builds live in this fork until upstream publishes compatible assets.
    public static string RenutRepository => Environment.GetEnvironmentVariable("NUTSTALLER_RENUT_REPO")
        ?? (OperatingSystem.IsMacOS() ? "ricjo4w/reNut" : "masterspike52/reNut");

    public static int RenutAssetRank(string name) => Rank(name,
        OperatingSystem.IsWindows() ? "windows" : OperatingSystem.IsMacOS() ? "macos" : "linux",
        RuntimeInformation.OSArchitecture);

    internal static int Rank(string name, string platform, Architecture architecture)
    {
        string n = name.ToLowerInvariant();
        if (platform == "macos")
        {
            if (!n.EndsWith(".zip") || !(n.Contains("mac") || n.Contains("osx"))) return -1;
            bool arm = n.Contains("arm64") || n.Contains("aarch64");
            bool intel = n.Contains("x64") || n.Contains("x86_64") || n.Contains("amd64");
            if (n.Contains("universal")) return 1;
            return architecture == Architecture.Arm64 ? (arm && !intel ? 2 : -1)
                : architecture == Architecture.X64 && intel && !arm ? 2 : -1;
        }
        if (platform == "linux") return n.EndsWith(".appimage") ? 0 : -1;
        if (!n.EndsWith(".zip") || n.Contains("linux") || n.Contains("mac") || n.Contains("osx")) return -1;
        return n.Contains("win") ? 1 : 0;
    }
}
