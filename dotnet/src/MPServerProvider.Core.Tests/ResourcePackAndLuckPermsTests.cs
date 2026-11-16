using System.Text;
using MPServerProvider.Core.Models;
using MPServerProvider.Core.Services;
using Xunit;

namespace MPServerProvider.Core.Tests;

public class JavaPropertiesFileTests
{
    [Fact]
    public void Parses_separators_escapes_and_continuations()
    {
        var file = JavaPropertiesFile.Parse(
            "# comment\r\n" +
            "! bang comment\r\n" +
            "a=1\r\n" +
            "b : two words\r\n" +
            "c   three\r\n" +
            "url=https\\://example.com/p.zip?x\\=1\r\n" +
            "uni=caf\\u00e9\r\n" +
            "multi=one \\\r\n" +
            "    two\r\n" +
            "empty=\r\n" +
            "esc\\ key=v\r\n");

        Assert.Equal("1", file.Get("a"));
        Assert.Equal("two words", file.Get("b"));
        Assert.Equal("three", file.Get("c"));
        Assert.Equal("https://example.com/p.zip?x=1", file.Get("url"));
        Assert.Equal("café", file.Get("uni"));
        Assert.Equal("one two", file.Get("multi"));
        Assert.Equal(string.Empty, file.Get("empty"));
        Assert.Equal("v", file.Get("esc key"));
        Assert.Null(file.Get("missing"));
        Assert.Null(file.Get("# comment"));
    }

    [Fact]
    public void Unchanged_document_round_trips_byte_for_byte()
    {
        const string text = "#Minecraft server properties\n#Sat Nov 15\nmotd=A \\u00A7aMinecraft Server\n\nmulti=a\\\n  b\nlast=no-newline";
        Assert.Equal(text, JavaPropertiesFile.Parse(text).ToString());
    }

    [Fact]
    public void Set_rewrites_only_that_line_and_keeps_order_comments_and_line_endings()
    {
        var file = JavaPropertiesFile.Parse("#c\r\nmotd=hi\r\nresource-pack=old\r\nlevel-name=world\r\n");
        file.Set("resource-pack", "https://x.dev/a.zip");
        file.Set("require-resource-pack", "true");

        Assert.Equal(
            "#c\r\nmotd=hi\r\nresource-pack=https\\://x.dev/a.zip\r\nlevel-name=world\r\nrequire-resource-pack=true\r\n",
            file.ToString());
    }

    [Fact]
    public void Set_replaces_every_physical_line_of_a_continued_value_and_all_duplicates()
    {
        var file = JavaPropertiesFile.Parse("k=a\\\n b\nother=1\nk=dup");
        file.Set("k", "new");
        Assert.Equal("k=new\nother=1\nk=new", file.ToString());
        Assert.Equal("new", file.Get("k"));
    }

    [Fact]
    public void Appending_to_a_file_without_trailing_newline_adds_one_first()
    {
        var file = JavaPropertiesFile.Parse("a=1\nb=2");
        file.Set("c", "3");
        Assert.Equal("a=1\nb=2\nc=3\n", file.ToString());
    }

    [Theory]
    [InlineData("plain", "plain")]
    [InlineData("a=b:c", "a\\=b\\:c")]
    [InlineData("back\\slash", "back\\\\slash")]
    [InlineData(" lead", "\\ lead")]
    [InlineData("line\nbreak", "line\\nbreak")]
    [InlineData("é", "\\u00E9")]
    public void Escapes_values(string value, string expected)
    {
        Assert.Equal(expected, JavaPropertiesFile.EscapeValue(value));
        Assert.Equal(value, JavaPropertiesFile.Unescape(expected));
    }

    [Fact]
    public void Save_keeps_bom_and_latin1_encodings()
    {
        var dir = Directory.CreateTempSubdirectory("mpsp-props-enc").FullName;
        try
        {
            var bomPath = Path.Combine(dir, "bom.properties");
            File.WriteAllBytes(bomPath, new UTF8Encoding(true).GetPreamble().Concat(Encoding.UTF8.GetBytes("motd=Grüße\n")).ToArray());
            var bom = JavaPropertiesFile.Load(bomPath);
            Assert.Equal("Grüße", bom.Get("motd"));
            bom.Set("x", "1");
            bom.Save(bomPath);
            var bytes = File.ReadAllBytes(bomPath);
            Assert.Equal(new byte[] { 0xEF, 0xBB, 0xBF }, bytes[..3]);
            Assert.Equal("motd=Grüße\nx=1\n", Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3));

            var latinPath = Path.Combine(dir, "latin.properties");
            File.WriteAllBytes(latinPath, Encoding.Latin1.GetBytes("motd=caf\u00e9\n"));
            var latin = JavaPropertiesFile.Load(latinPath);
            Assert.Equal("café", latin.Get("motd"));
            latin.Set("x", "1");
            latin.Save(latinPath);
            Assert.Equal(Encoding.Latin1.GetBytes("motd=caf\u00e9\nx=1\n"), File.ReadAllBytes(latinPath));
            Assert.False(File.Exists(latinPath + ".mpsp-tmp"));
        }
        finally
        {
            Directory.Delete(dir, true);
        }
    }
}

public class ResourcePackPropertiesTests
{
    private static readonly string Sha = new('a', 40);

    [Fact]
    public void Apply_writes_the_four_keys_and_preserves_everything_else()
    {
        var root = Directory.CreateTempSubdirectory("mpsp-rp").FullName;
        try
        {
            var path = Path.Combine(root, "server.properties");
            File.WriteAllText(path, "#Minecraft server properties\nmotd=Hello\\: world\nresource-pack=\nresource-pack-sha1=\nserver-port=25565\n");

            ResourcePackProperties.Apply(root, new ResourcePackSettings(
                " https://mcpacks.dev/pack/abc/download ", Sha.ToUpperInvariant(), true,
                ResourcePackProperties.PromptJsonFromText("Please \"accept\" \\ ok")));

            Assert.Equal(
                "#Minecraft server properties\n" +
                "motd=Hello\\: world\n" +
                "resource-pack=https\\://mcpacks.dev/pack/abc/download\n" +
                $"resource-pack-sha1={Sha}\n" +
                "server-port=25565\n" +
                "require-resource-pack=true\n" +
                "resource-pack-prompt={\"text\"\\:\"Please \\\\\"accept\\\\\" \\\\\\\\ ok\"}\n",
                File.ReadAllText(path));

            var read = ResourcePackProperties.Read(root);
            Assert.Equal("https://mcpacks.dev/pack/abc/download", read.Url);
            Assert.Equal(Sha, read.Sha1);
            Assert.True(read.Required);
            Assert.Equal("Please \"accept\" \\ ok", ResourcePackProperties.PromptTextFromJson(read.PromptJson));

            ResourcePackProperties.Remove(root);
            var removed = ResourcePackProperties.Read(root);
            Assert.Equal(ResourcePackSettings.None, removed);
            Assert.Contains("motd=Hello\\: world", File.ReadAllText(path));
            Assert.Contains("require-resource-pack=false", File.ReadAllText(path));
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void Apply_creates_the_file_when_missing()
    {
        var root = Directory.CreateTempSubdirectory("mpsp-rp-new").FullName;
        try
        {
            ResourcePackProperties.Apply(root, new ResourcePackSettings("https://e.com/p.zip", Sha, false, string.Empty));
            Assert.Equal("https://e.com/p.zip", ResourcePackProperties.Read(root).Url);
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [Theory]
    [InlineData("", "", false, "", true)]
    [InlineData("https://e.com/p.zip", "", false, "", true)]
    [InlineData("https://e.com/p.zip", "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", true, "{\"text\":\"hi\"}", true)]
    [InlineData("ftp://e.com/p.zip", "", false, "", false)]
    [InlineData("not a url", "", false, "", false)]
    [InlineData("https://e.com/p.zip", "abc", false, "", false)]
    [InlineData("", "", true, "", false)]
    [InlineData("https://e.com/p.zip", "", false, "{broken", false)]
    public void Validates_settings(string url, string sha1, bool required, string prompt, bool ok)
        => Assert.Equal(ok, ResourcePackProperties.Validate(new ResourcePackSettings(url, sha1, required, prompt)) is null);

    [Fact]
    public void Apply_rejects_invalid_settings()
    {
        var root = Directory.CreateTempSubdirectory("mpsp-rp-bad").FullName;
        try
        {
            Assert.Throws<ArgumentException>(() => ResourcePackProperties.Apply(root, new ResourcePackSettings("https://e.com/p.zip", "zz", false, "")));
            Assert.False(File.Exists(Path.Combine(root, "server.properties")));
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [Fact]
    public void Computes_lowercase_sha1()
    {
        var file = Path.GetTempFileName();
        try
        {
            File.WriteAllText(file, "abc");
            Assert.Equal("a9993e364706816aba3e25717850c26c9cd0d89d", ResourcePackProperties.ComputeSha1(file));
        }
        finally
        {
            File.Delete(file);
        }
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("{\"text\":\"Hi\"}", "Hi")]
    [InlineData("\"plain json string\"", "plain json string")]
    [InlineData("{\"text\":\"A\",\"extra\":[{\"text\":\"B\",\"color\":\"red\"},\"C\"]}", "ABC")]
    [InlineData("[{\"text\":\"x\"},{\"text\":\"y\"}]", "xy")]
    [InlineData("not json", "not json")]
    public void Reads_prompt_text(string json, string expected)
        => Assert.Equal(expected, ResourcePackProperties.PromptTextFromJson(json));

    [Fact]
    public void Prompt_json_is_escaped_and_detects_styling()
    {
        Assert.Equal(string.Empty, ResourcePackProperties.PromptJsonFromText("  "));
        Assert.Equal("{\"text\":\"Line1\\nLine2 \\\"q\\\"\"}", ResourcePackProperties.PromptJsonFromText("Line1\nLine2 \"q\""));
        Assert.False(ResourcePackProperties.IsStyledPrompt("{\"text\":\"x\"}"));
        Assert.True(ResourcePackProperties.IsStyledPrompt("{\"text\":\"x\",\"color\":\"gold\"}"));
    }

    [Theory]
    [InlineData("1.16.5", false)]
    [InlineData("1.17", true)]
    [InlineData("1.21.4", true)]
    [InlineData("26.3", true)]
    public void Knows_which_versions_support_required_packs(string version, bool expected)
        => Assert.Equal(expected, ResourcePackProperties.SupportsRequiredPrompt(version));

    [Fact]
    public void Unknown_versions_are_not_guessed()
        => Assert.Null(ResourcePackProperties.SupportsRequiredPrompt("24w14a"));

    [Fact]
    public void Parses_mcpacks_inertia_pages()
    {
        var json = "{\"component\":\"UploadSuccess\",\"props\":{\"pack\":{\"original_filename\":\"My Pack.zip\"," +
                   "\"sha1_hash\":\"" + Sha.ToUpperInvariant() + "\",\"download_url\":\"https://mcpacks.dev/pack/550e8400-e29b-41d4-a716-446655440000/download\"}}}";
        var pack = ResourcePackProperties.TryParseMcPacksPage(json);
        Assert.NotNull(pack);
        Assert.Equal("https://mcpacks.dev/pack/550e8400-e29b-41d4-a716-446655440000/download", pack!.DownloadUrl);
        Assert.Equal(Sha, pack.Sha1);
        Assert.Equal("My Pack.zip", pack.FileName);

        Assert.NotNull(ResourcePackProperties.TryParseMcPacksPage("{\"page\":" + json + "}"));
        Assert.Null(ResourcePackProperties.TryParseMcPacksPage("{\"component\":\"Home\",\"props\":{\"stats\":{}}}"));
        Assert.Null(ResourcePackProperties.TryParseMcPacksPage("<html>"));
        Assert.Null(ResourcePackProperties.TryParseMcPacksPage(json.Replace(Sha.ToUpperInvariant(), "nothex")));
        Assert.Null(ResourcePackProperties.TryCreateHostedPack("javascript:alert(1)", Sha, ""));
    }
}

public class LuckPermsSupportTests
{
    [Fact]
    public void Finds_luckperms_jar_in_plugins_mods_or_custom_folder()
    {
        var root = Directory.CreateTempSubdirectory("mpsp-lp").FullName;
        try
        {
            var server = new ServerInstance { ServerName = "s", ServerRoot = root };
            Assert.Null(LuckPermsSupport.FindJar(server));

            Directory.CreateDirectory(Path.Combine(root, "plugins"));
            File.WriteAllText(Path.Combine(root, "plugins", "EssentialsX.jar"), "");
            File.WriteAllText(Path.Combine(root, "plugins", "LuckPerms-Bukkit-5.4.141.jar.disabled"), "");
            Assert.Null(LuckPermsSupport.FindJar(server));

            File.WriteAllText(Path.Combine(root, "plugins", "LuckPerms-Bukkit-5.4.141.jar"), "");
            Assert.EndsWith("LuckPerms-Bukkit-5.4.141.jar", LuckPermsSupport.FindJar(server));

            var other = Directory.CreateTempSubdirectory("mpsp-lp2").FullName;
            try
            {
                var hytale = new ServerInstance { ServerName = "h", ServerRoot = other };
                var game = new GameDefinition { Mods = new ModsSpec { Supported = true, Folder = "UserData/Mods" } };
                Directory.CreateDirectory(Path.Combine(other, "UserData", "Mods"));
                File.WriteAllText(Path.Combine(other, "UserData", "Mods", "luckperms-hytale.jar"), "");
                Assert.Null(LuckPermsSupport.FindJar(hytale));
                Assert.NotNull(LuckPermsSupport.FindJar(hytale, game));
            }
            finally
            {
                Directory.Delete(other, true);
            }
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [Theory]
    [InlineData("[12:00:01 INFO]: [LP] Preparing a new editor session, please wait...", LuckPermsEventKind.Preparing, "")]
    [InlineData("[12:00:02 INFO]: https://luckperms.net/editor/AbC123xyz", LuckPermsEventKind.EditorUrl, "https://luckperms.net/editor/AbC123xyz")]
    [InlineData("[12:00:02 INFO]: [LP] Click the link below to open the editor:", LuckPermsEventKind.None, "")]
    [InlineData("[12:00:03 INFO]: [LP] Editor window connected successfully.", LuckPermsEventKind.SocketConnected, "")]
    [InlineData("[LP] If it was you, run /lp trusteditor k3yZ to trust the session!", LuckPermsEventKind.TrustRequired, "k3yZ")]
    [InlineData("[LP] An editor window has connected, but it is not yet trusted.", LuckPermsEventKind.TrustRequired, "")]
    [InlineData("[LP] The editor session has been marked as trusted.", LuckPermsEventKind.Trusted, "")]
    [InlineData("[LP] Web editor data was applied to group admin successfully.", LuckPermsEventKind.ChangesApplied, "")]
    [InlineData("[LP] Changes have been received from the connected web editor session.", LuckPermsEventKind.ChangesApplied, "")]
    [InlineData("[LP] No changes were applied from the web editor, the returned data didn't contain any edits.", LuckPermsEventKind.NoChanges, "")]
    [InlineData("> lp editor", LuckPermsEventKind.None, "")]
    [InlineData("[Server thread/INFO]: Done (3.2s)!", LuckPermsEventKind.None, "")]
    public void Parses_console_lines(string line, LuckPermsEventKind kind, string value)
    {
        var parsed = LuckPermsSupport.Parse(line);
        Assert.Equal(kind, parsed.Kind);
        Assert.Equal(value, parsed.Value);
    }

    [Fact]
    public void Error_lines_are_reported()
        => Assert.Equal(LuckPermsEventKind.Error, LuckPermsSupport.Parse("[LP] Unable to communicate with the editor. (response code 403)").Kind);

    [Fact]
    public void Finds_applyedits_and_builds_safe_commands()
    {
        Assert.Equal(("lp", "Xy12ab"), LuckPermsSupport.FindApplyEdits("  /lp applyedits Xy12ab  "));
        Assert.Equal(("lpv", "q9"), LuckPermsSupport.FindApplyEdits("/lpv applyedits q9"));
        Assert.Null(LuckPermsSupport.FindApplyEdits("/lp editor"));
        Assert.Equal(("lp", "n0nce"), LuckPermsSupport.FindTrustEditor("/lp trusteditor n0nce"));

        Assert.Equal("lp applyedits Xy12ab", LuckPermsSupport.BuildCommand("lp", "applyedits", "Xy12ab"));
        Assert.Equal("lp trusteditor n0nce", LuckPermsSupport.BuildCommand("LP", "trusteditor", "n0nce"));
        Assert.Null(LuckPermsSupport.BuildCommand("op", "applyedits", "x"));
        Assert.Null(LuckPermsSupport.BuildCommand("lp", "applyedits", "x; stop"));
        Assert.Null(LuckPermsSupport.BuildCommand("lp", "deleteall", "x"));
    }
}
