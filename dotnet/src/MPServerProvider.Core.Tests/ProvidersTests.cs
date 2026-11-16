using MPServerProvider.Core.Models;
using MPServerProvider.Core.Providers;
using Xunit;

namespace MPServerProvider.Core.Tests;

/// <summary>
/// Ports of the proven Rust port's pure-logic provider tests
/// (<c>crates/mpsp-core/src/providers/{vdf,steamcmd,cfx,beammp,java_runtime,manual}.rs</c>) to
/// C# equivalents against the real, restored provider types. These are the main correctness
/// signal for the <see cref="MPServerProvider.Core.Providers"/> port, since most provider
/// methods otherwise talk to real network endpoints.
/// </summary>
public class VdfTests
{
    private const string Sample = """

"4020"
{
    "common"
    {
        "name"              "Garry's Mod Dedicated Server"
        "parent"            "4000"
        "freetodownload"    "1"
    }
    "config"
    {
        "installdir"    "GarrysModDS"
        "launch"
        {
            "0"
            {
                "executable"    "srcds.exe"
                "workingdir"    "."
                "arguments"     "-game garrysmod -console"
            }
        }
    }
}

""";

    [Fact]
    public void Parses_nested_blocks_and_leaves()
    {
        var root = Vdf.Parse(Sample);
        Assert.True(root.Children.ContainsKey("4020"));
        var app = root.Children["4020"];
        var common = app.Find("common");
        Assert.NotNull(common);
        Assert.Equal("Garry's Mod Dedicated Server", common!.Find("name")?.Value);

        var config = app.Find("config");
        Assert.NotNull(config);
        Assert.Equal("GarrysModDS", config!.Find("installdir")?.Value);
    }

    [Fact]
    public void ToAppInfo_extracts_launch_entry()
    {
        var root = Vdf.Parse(Sample);
        var info = Vdf.ToAppInfo(root);
        Assert.NotNull(info);
        Assert.Equal("Garry's Mod Dedicated Server", info!.Name);
        Assert.Equal(4000, info.ParentAppId);
        Assert.Equal("GarrysModDS", info.InstallDir);
        Assert.Equal("srcds.exe", info.Executable);
        Assert.Equal(".", info.WorkingDir);
        Assert.Equal("-game garrysmod -console", info.Arguments);
        Assert.True(info.FreeToDownload);
    }

    [Fact]
    public void Handles_empty_document()
    {
        var root = Vdf.Parse(string.Empty);
        Assert.Empty(root.Children);
        Assert.Null(Vdf.ToAppInfo(root));
    }

    [Fact]
    public void Missing_optional_fields_default_sensibly()
    {
        var root = Vdf.Parse("\"440\" { \"common\" { \"name\" \"Team Fortress 2\" } }");
        var info = Vdf.ToAppInfo(root);
        Assert.NotNull(info);
        Assert.Equal("Team Fortress 2", info!.Name);
        Assert.Null(info.ParentAppId);
        Assert.Null(info.InstallDir);
        Assert.False(info.FreeToDownload);
    }
}

public class SteamCmdProviderTests
{
    [Fact]
    public void Login_arguments_anonymous_by_default()
    {
        var spec = new InstallSpec { Anonymous = true };
        Assert.Equal("+login anonymous", SteamCmdProvider.LoginArguments(spec));
    }

    [Fact]
    public void Login_arguments_anonymous_when_username_blank()
    {
        var spec = new InstallSpec();
        Assert.Equal("+login anonymous", SteamCmdProvider.LoginArguments(spec));
    }

    [Fact]
    public void Login_arguments_username_only_when_no_password()
    {
        var spec = new InstallSpec { Username = "steamuser" };
        Assert.Equal("+login \"steamuser\"", SteamCmdProvider.LoginArguments(spec));
    }

    [Fact]
    public void Definition_names_executable_checks_placeholder()
    {
        Assert.True(SteamCmdProvider.DefinitionNamesExecutable("{server_root}\\{server_file}"));
        Assert.False(SteamCmdProvider.DefinitionNamesExecutable("{server_root}\\srcds.exe"));
        Assert.False(SteamCmdProvider.DefinitionNamesExecutable(string.Empty));
    }

    [Fact]
    public void Parse_progress_extracts_fields_from_steamcmd_line()
    {
        const string line = "Update state (0x61) downloading, progress: 42.50 (123456 / 7890123)";
        var parsed = SteamCmdProvider.ParseProgress(line);
        Assert.NotNull(parsed);
        Assert.Equal("downloading", parsed!.State);
        Assert.Equal(42.5, parsed.Percent);
        Assert.Equal(123456, parsed.BytesDone);
        Assert.Equal(7890123, parsed.BytesTotal);
    }

    [Fact]
    public void Parse_progress_returns_none_for_unrelated_line()
    {
        Assert.Null(SteamCmdProvider.ParseProgress("Logging in user 'anonymous'"));
    }

    [Fact]
    public void Megabytes_formats_as_whole_number()
    {
        Assert.Equal("10", SteamCmdProvider.Megabytes(10_485_760));
    }

    [Fact]
    public void Exe_blocklist_matches_known_noise_binaries_via_autodetect()
    {
        var dir = Path.Combine(Path.GetTempPath(), "mpsp-steamcmd-test-" + Guid.NewGuid().ToString("N"));
        var binDir = Path.Combine(dir, "bin");
        Directory.CreateDirectory(binDir);
        try
        {
            File.WriteAllBytes(Path.Combine(binDir, "steamerrorreporter.exe"), Array.Empty<byte>());
            File.WriteAllBytes(Path.Combine(binDir, "vcredist_x64.exe"), Array.Empty<byte>());
            File.WriteAllBytes(Path.Combine(binDir, "my_server.exe"), new byte[10]);

            var found = SteamCmdProvider.AutoDetectServerExe(dir);
            Assert.NotNull(found);
            Assert.Contains("my_server.exe", found);
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }
}

public class CfxProviderTests
{
    [Fact]
    public void Parses_artifact_index_from_directory_listing()
    {
        const string html = """
            <html><body>
            <a href="../">../</a>
            <a href="15999-ab12cd34ef56ab78/">15999-ab12cd34ef56ab78/</a>
            <a href="16000-0123456789abcdef/">16000-0123456789abcdef/</a>
            </body></html>
        """;

        var index = CfxProvider.ParseArtifactIndex(html);
        Assert.Equal(2, index.Count);
        Assert.Equal("16000-0123456789abcdef", index["16000"]);
        Assert.Equal("15999-ab12cd34ef56ab78", index["15999"]);
    }

    [Fact]
    public void Empty_listing_yields_empty_index()
    {
        Assert.Empty(CfxProvider.ParseArtifactIndex("<html></html>"));
    }

    [Fact]
    public void Parses_builds_from_download_page_links()
    {
        const string page = """
            {"downloadURL":"https://runtime.fivem.net/artifacts/fivem/build_server_windows/master/38621-5412ed86f9eff1b81d2afb10cff85153b043d3b8/server.7z"},
            {"downloadURL":"https://runtime.fivem.net/artifacts/fivem/build_proot_linux/master/38435-5c3065c261fb3ba82032fee6803f247198fec221/fx.tar.xz"}
        """;

        var index = CfxProvider.ParseArtifactIndex(page);
        Assert.Single(index);
        Assert.Equal("38621-5412ed86f9eff1b81d2afb10cff85153b043d3b8", index["38621"]);
    }

    [Fact]
    public void Channels_come_first_and_shared_builds_merge_labels()
    {
        const string json = """
            {"critical":"7290","recommended":"38435","optional":"7290","latest":"38621",
             "critical_download":"https://runtime.fivem.net/artifacts/fivem/build_server_windows/master/7290-a654bcc2/server.zip",
             "recommended_download":"https://runtime.fivem.net/artifacts/fivem/build_server_windows/master/38435-5c3065c2/server.zip",
             "optional_download":"https://runtime.fivem.net/artifacts/fivem/build_server_windows/master/7290-a654bcc2/server.zip",
             "latest_download":"https://runtime.fivem.net/artifacts/fivem/build_server_windows/master/38621-5412ed86/server.zip"}
        """;

        var channels = CfxProvider.ParseChannels(json);
        var index = new SortedDictionary<string, string>(StringComparer.Ordinal) { ["38500"] = "38500-abc" };
        foreach (var c in channels)
        {
            index.TryAdd(c.Build, c.ArtifactDir!);
        }

        var versions = CfxProvider.BuildVersionList(index, channels);
        Assert.Equal(new[]
        {
            "build 38435 (recommended)",
            "build 38621 (latest)",
            "build 7290 (optional, critical)",
            "build 38500",
        }, versions);
    }
}

public class BeamMpProviderTests
{
    private static InstallRequest Request(string targetDir) => new()
    {
        Game = new GameDefinition
        {
            ManifestVersion = 1,
            Id = "beammp",
            Name = "BeamMP",
            Description = string.Empty,
            IsBuiltIn = true,
        },
        ServerName = "My Server",
        TargetDir = targetDir,
        Port = 30814,
        Description = "A test server",
        EulaAccepted = true,
    };

    [Fact]
    public void Write_config_produces_expected_toml()
    {
        var dir = Path.Combine(Path.GetTempPath(), "mpsp-beammp-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            var req = Request(dir);

            BeamMpProvider.WriteConfig(req);
            var configPath = Path.Combine(dir, "ServerConfig.toml");
            var contents = File.ReadAllText(configPath);
            Assert.Contains("Name = \"My Server\"", contents);
            Assert.Contains("Port = 30814", contents);
            Assert.Contains("Description = \"A test server\"", contents);
            Assert.Contains("Private = true", contents);

            // Second call must not overwrite an existing config.
            File.WriteAllText(configPath, "custom");
            BeamMpProvider.WriteConfig(req);
            Assert.Equal("custom", File.ReadAllText(configPath));
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }
}

public class ManualProviderTests
{
    [Fact]
    public void Finds_known_server_exe_by_priority()
    {
        var dir = Path.Combine(Path.GetTempPath(), "mpsp-manual-test-" + Guid.NewGuid().ToString("N"));
        var binDir = Path.Combine(dir, "bin");
        Directory.CreateDirectory(binDir);
        try
        {
            File.WriteAllBytes(Path.Combine(binDir, "RustDedicated.exe"), Array.Empty<byte>());
            File.WriteAllBytes(Path.Combine(dir, "notes.txt"), Array.Empty<byte>());

            var found = ManualProvider.FindKnownServerExe(dir);
            Assert.NotNull(found);
            Assert.EndsWith("RustDedicated.exe", found);
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    [Fact]
    public void Returns_none_when_no_known_exe_present()
    {
        var dir = Path.Combine(Path.GetTempPath(), "mpsp-manual-test-empty-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            File.WriteAllBytes(Path.Combine(dir, "readme.md"), Array.Empty<byte>());
            Assert.Null(ManualProvider.FindKnownServerExe(dir));
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }
}

public class JavaRuntimeProviderTests
{
    [Fact]
    public void Walk_for_java_exe_finds_nested_binary()
    {
        var dir = Path.Combine(Path.GetTempPath(), "mpsp-java-test-" + Guid.NewGuid().ToString("N"));
        var nested = Path.Combine(dir, "jdk-17.0.1+12-jre", "bin");
        Directory.CreateDirectory(nested);
        try
        {
            File.WriteAllBytes(Path.Combine(nested, "java.exe"), Array.Empty<byte>());

            var found = JavaRuntimeProvider.WalkForJavaExe(dir);
            Assert.NotNull(found);
            Assert.EndsWith("java.exe", found);
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }

    [Fact]
    public void Walk_for_java_exe_returns_none_when_absent()
    {
        var dir = Path.Combine(Path.GetTempPath(), "mpsp-java-test-empty-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            Assert.Null(JavaRuntimeProvider.WalkForJavaExe(dir));
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }
}

public class MinecraftJavaProviderTests
{
    [Theory]
    [InlineData("1.21.1", 21)]
    [InlineData("1.20.5", 21)]
    [InlineData("1.20.4", 17)]
    [InlineData("1.18.2", 17)]
    [InlineData("1.17.1", 16)]
    [InlineData("1.12.2", 8)]
    [InlineData("not-a-version", 8)]
    [InlineData("26.1", 25)]
    [InlineData("26.3", 25)]
    [InlineData("26.4-snapshot-8", 25)]
    [InlineData("26.1.2 (build 74)", 25)]
    [InlineData("1.21.11", 21)]
    public void Guesses_java_major_by_known_version_bands(string mcVersion, int expected)
    {
        Assert.Equal(expected, MinecraftJavaProvider.GuessJavaMajor(mcVersion));
    }
}
