using System.Runtime.InteropServices;
using NutStaller.Core;

static void Check(bool condition, string name)
{
    if (!condition) throw new Exception(name);
}

foreach (string name in new[] { "renut-macos-arm64.zip", "reNut-osx-aarch64.zip" })
    Check(ReleasePlatform.Rank(name, "macos", Architecture.Arm64) == 2, name);
foreach (string name in new[] { "renut-win-x64.zip", "renut-x86_64.AppImage", "renut-macos-x64.zip", "renut-macos.zip", "source.zip" })
    Check(ReleasePlatform.Rank(name, "macos", Architecture.Arm64) == -1, name);
Check(ReleasePlatform.Rank("renut-macos-universal.zip", "macos", Architecture.Arm64) == 1, "universal");
Check(ReleasePlatform.Rank("renut-macos-arm64.zip", "windows", Architecture.X64) == -1, "Windows rejects Mac");
Check(ReleasePlatform.Rank("renut-win-x64.zip", "windows", Architecture.X64) == 1, "Windows zip");
Check(ReleasePlatform.Rank("renut-x86_64.AppImage", "linux", Architecture.X64) == 0, "Linux AppImage");
string temp = Path.Combine(Path.GetTempPath(), "nutstaller-check-" + Guid.NewGuid());
try
{
    string executable = Path.Combine(temp, "reNut.app", "Contents", "MacOS", "renut");
    Directory.CreateDirectory(Path.GetDirectoryName(executable)!);
    File.WriteAllText(executable, "test");
    GithubDownloader.FlattenSingleFolder(temp);
    Check(File.Exists(executable), "Do not flatten a sole app bundle");
    Directory.Delete(temp, true);
    Directory.CreateDirectory(Path.Combine(temp, "release"));
    File.WriteAllText(Path.Combine(temp, "release", "renut.exe"), "test");
    GithubDownloader.FlattenSingleFolder(temp);
    Check(File.Exists(Path.Combine(temp, "renut.exe")), "Keep flattening Windows wrapper folders");
}
finally { if (Directory.Exists(temp)) Directory.Delete(temp, true); }
Console.WriteLine("Platform release selection and app bundle extraction checks passed.");

// Optional integration check against an actual signed macOS release archive.
if (args.Length == 1 && OperatingSystem.IsMacOS())
{
    string extracted = Path.Combine(Path.GetTempPath(), "nutstaller-release-" + Guid.NewGuid());
    try
    {
        GithubDownloader.ExtractZip(Path.GetFullPath(args[0]), extracted);
        GithubDownloader.FlattenSingleFolder(extracted);
        string app = Path.Combine(extracted, "reNut.app");
        Check(Directory.Exists(app), "Release retains app bundle");
        foreach (string name in new[] { "renut", "renut_controller_check" })
        {
            string binary = Path.Combine(app, "Contents", "MacOS", name);
            Check((File.GetUnixFileMode(binary) & UnixFileMode.UserExecute) != 0,
                name + " retains executable permission");
        }
        using var signature = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(
            "/usr/bin/codesign") { ArgumentList = { "--verify", "--deep", "--strict", app } })!;
        signature.WaitForExit();
        Check(signature.ExitCode == 0, "Extracted app signature verifies");
        Console.WriteLine("Packaged release extraction, permissions and signature passed.");
    }
    finally { if (Directory.Exists(extracted)) Directory.Delete(extracted, true); }
}
