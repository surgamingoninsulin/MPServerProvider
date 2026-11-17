using MPServerProvider.Core.Services;
using Xunit;

namespace MPServerProvider.Core.Tests;

public class OpenRct2SetupTests
{
    [Fact]
    public void EnsureGroupsFile_writes_once_and_never_overwrites()
    {
        var root = Path.Combine(Path.GetTempPath(), "mpsp-openrct2-" + Guid.NewGuid());
        try
        {
            OpenRct2Setup.EnsureGroupsFile(root);
            var path = OpenRct2Setup.GroupsFile(root);
            Assert.True(File.Exists(path));
            Assert.Contains("\"name\": \"Admin\"", File.ReadAllText(path));

            File.WriteAllText(path, "custom content");
            OpenRct2Setup.EnsureGroupsFile(root);
            Assert.Equal("custom content", File.ReadAllText(path));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void SetGameDataPaths_writes_and_reads_back_both_paths()
    {
        var root = Path.Combine(Path.GetTempPath(), "mpsp-openrct2-" + Guid.NewGuid());
        try
        {
            OpenRct2Setup.SetGameDataPaths(root, @"C:\Games\RCT2", @"C:\Games\RCT1 Deluxe");

            var (rct2, rct1) = OpenRct2Setup.ReadGameDataPaths(root);
            Assert.Equal(@"C:\Games\RCT2", rct2);
            Assert.Equal(@"C:\Games\RCT1 Deluxe", rct1);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void SetGameDataPaths_updates_an_existing_key_in_place_without_duplicating_it()
    {
        var root = Path.Combine(Path.GetTempPath(), "mpsp-openrct2-" + Guid.NewGuid());
        try
        {
            OpenRct2Setup.SetGameDataPaths(root, @"C:\Games\RCT2", null);
            OpenRct2Setup.SetGameDataPaths(root, @"D:\Steam\RCT2", null);

            var lines = File.ReadAllLines(OpenRct2Setup.ConfigFile(root));
            Assert.Single(lines, l => l.Contains("game_path"));

            var (rct2, rct1) = OpenRct2Setup.ReadGameDataPaths(root);
            Assert.Equal(@"D:\Steam\RCT2", rct2);
            Assert.Null(rct1);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
