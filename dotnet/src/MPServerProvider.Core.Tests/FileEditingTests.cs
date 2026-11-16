using System.Text;
using MPServerProvider.Core.Services;
using Xunit;

namespace MPServerProvider.Core.Tests;

public class FileEditingTests : IDisposable
{
    private readonly string _temp;

    public FileEditingTests()
    {
        _temp = Path.Combine(Path.GetTempPath(), "mpsp-editing-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_temp);
    }

    public void Dispose()
    {
        try { Directory.Delete(_temp, recursive: true); } catch { /* best-effort */ }
    }

    // -- PathGuard ------------------------------------------------------------------------

    [Theory]
    [InlineData(@"C:\servers\mc", @"C:\servers\mc\server.properties", true)]
    [InlineData(@"C:\servers\mc", @"C:\SERVERS\MC\config\a.yml", true)]
    [InlineData(@"C:\servers\mc\", @"C:\servers\mc\world\level.dat", true)]
    [InlineData(@"C:\servers\mc", @"C:\servers\mc2\server.properties", false)]
    [InlineData(@"C:\servers\mc", @"C:\servers\mcx", false)]
    [InlineData(@"C:\servers\mc", @"C:\servers\mc\..\other\secret.txt", false)]
    [InlineData(@"C:\servers\mc", @"C:\servers\mc\config\..\..\mc\ok.txt", true)]
    [InlineData(@"C:\servers\mc", @"C:\Windows\System32\drivers\etc\hosts", false)]
    [InlineData(@"C:\servers\mc", @"..\escape.txt", false)]
    [InlineData(@"C:\servers\mc", @"plugins\config.yml", true)]
    [InlineData(@"C:\servers\mc", @"", false)]
    [InlineData(@"", @"C:\servers\mc\a.txt", false)]
    public void IsInsideRoot_matches_expected(string root, string candidate, bool expected)
    {
        Assert.Equal(expected, PathGuard.IsInsideRoot(root, candidate));
    }

    [Fact]
    public void Root_itself_is_only_allowed_when_requested()
    {
        Assert.False(PathGuard.IsInsideRoot(@"C:\servers\mc", @"C:\servers\mc\"));
        Assert.True(PathGuard.IsInsideRoot(@"C:\servers\mc", @"C:\servers\mc\", allowRootItself: true));
        Assert.True(PathGuard.IsInsideRoot(@"C:\servers\mc", @"c:\servers\MC", allowRootItself: true));
    }

    [Fact]
    public void TryResolveInsideRoot_accepts_real_nested_file_and_rejects_traversal()
    {
        var root = Path.Combine(_temp, "root");
        Directory.CreateDirectory(Path.Combine(root, "config"));
        var file = Path.Combine(root, "config", "a.txt");
        File.WriteAllText(file, "x");

        Assert.True(PathGuard.TryResolveInsideRoot(root, file, out var resolved));
        Assert.Equal(Path.GetFullPath(file), resolved, ignoreCase: true);

        Assert.False(PathGuard.TryResolveInsideRoot(root, Path.Combine(root, "..", "outside.txt"), out _));
        Assert.Throws<UnauthorizedAccessException>(() => PathGuard.EnsureInsideRoot(root, Path.Combine(_temp, "outside.txt")));
    }

    [Fact]
    public void TryResolveInsideRoot_rejects_symlink_escape()
    {
        var root = Path.Combine(_temp, "root");
        var outside = Path.Combine(_temp, "outside");
        Directory.CreateDirectory(root);
        Directory.CreateDirectory(outside);
        File.WriteAllText(Path.Combine(outside, "secret.txt"), "secret");

        var link = Path.Combine(root, "link");
        try
        {
            Directory.CreateSymbolicLink(link, outside);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or PlatformNotSupportedException)
        {
            // Creating symlinks needs Developer Mode / admin on Windows; nothing to verify here.
            return;
        }

        // String-wise it looks contained, but the on-disk check must catch the reparse point.
        Assert.True(PathGuard.IsInsideRoot(root, Path.Combine(link, "secret.txt")));
        Assert.False(PathGuard.TryResolveInsideRoot(root, Path.Combine(link, "secret.txt"), out _));
        Assert.False(PathGuard.TryResolveInsideRoot(root, link, out _));
    }

    // -- TextFileDetector --------------------------------------------------------------------

    [Theory]
    [InlineData("server.jar", true)]
    [InlineData("WORLD.ZIP", true)]
    [InlineData("bedrock_server.exe", true)]
    [InlineData("icon.png", true)]
    [InlineData("level.dat", true)]
    [InlineData("data.sqlite", true)]
    [InlineData("Content.pak", true)]
    [InlineData("server.properties", false)]
    [InlineData("server.cfg", false)]
    [InlineData("config.yml", false)]
    [InlineData("Dockerfile", false)]
    [InlineData("latest.log", false)]
    public void IsKnownBinaryExtension_matches_expected(string name, bool expected)
    {
        Assert.Equal(expected, TextFileDetector.IsKnownBinaryExtension(name));
    }

    [Fact]
    public void LooksBinary_detects_nul_bytes_but_not_utf16_text()
    {
        Assert.False(TextFileDetector.LooksBinary(Encoding.UTF8.GetBytes("motd=Hello\nmax-players=20\n")));
        Assert.False(TextFileDetector.LooksBinary(ReadOnlySpan<byte>.Empty));
        Assert.True(TextFileDetector.LooksBinary(new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x00, 0x00 }));

        var utf16 = Encoding.Unicode.GetPreamble().Concat(Encoding.Unicode.GetBytes("key=value")).ToArray();
        Assert.False(TextFileDetector.LooksBinary(utf16));
    }

    [Fact]
    public void Classify_covers_text_binary_content_extension_size_and_missing()
    {
        var text = Path.Combine(_temp, "server.properties");
        File.WriteAllText(text, "motd=hi\n");
        var sneaky = Path.Combine(_temp, "notes.txt");
        File.WriteAllBytes(sneaky, new byte[] { 0x41, 0x00, 0x42 });
        var jar = Path.Combine(_temp, "server.jar");
        File.WriteAllText(jar, "pretend text");
        var big = Path.Combine(_temp, "big.log");
        File.WriteAllBytes(big, Enumerable.Repeat((byte)'a', 2048).ToArray());

        Assert.Equal(TextEditability.Editable, TextFileDetector.Classify(text));
        Assert.Equal(TextEditability.BinaryContent, TextFileDetector.Classify(sneaky));
        Assert.Equal(TextEditability.BinaryExtension, TextFileDetector.Classify(jar));
        Assert.Equal(TextEditability.TooLarge, TextFileDetector.Classify(big, maxBytes: 1024));
        Assert.Equal(TextEditability.NotFound, TextFileDetector.Classify(Path.Combine(_temp, "nope.txt")));
    }

    [Theory]
    [InlineData("server.exe", true)]
    [InlineData("start.BAT", true)]
    [InlineData("install.ps1", true)]
    [InlineData("payload.vbs", true)]
    [InlineData("plugin.jar", true)]
    [InlineData("shortcut.lnk", true)]
    [InlineData("tweak.reg", true)]
    [InlineData("README", true)]
    [InlineData("icon.png", false)]
    [InlineData("world.zip", false)]
    [InlineData("latest.log", false)]
    public void ShellOpenPolicy_blocks_executables_and_scripts(string name, bool unsafeToOpen)
    {
        Assert.Equal(unsafeToOpen, ShellOpenPolicy.IsUnsafeToShellOpen(name));
    }

    // -- TextFileDocument -------------------------------------------------------------------

    [Fact]
    public void Utf8_bom_and_crlf_are_preserved_on_round_trip()
    {
        var original = new byte[] { 0xEF, 0xBB, 0xBF }.Concat(Encoding.UTF8.GetBytes("a=1\r\nb=ü\r\n")).ToArray();
        var doc = TextFileDocument.Parse(original);

        Assert.True(doc.HasBom);
        Assert.Equal("\r\n", doc.LineEnding);
        Assert.Equal("a=1\nb=ü\n", doc.Text);
        Assert.Equal(original, doc.Encode(doc.Text));
        Assert.Equal("UTF-8 BOM · CRLF", doc.Description);
    }

    [Fact]
    public void Lf_file_without_bom_stays_lf_even_if_editor_inserted_crlf()
    {
        var doc = TextFileDocument.Parse(Encoding.UTF8.GetBytes("x\ny\n"));
        Assert.False(doc.HasBom);
        Assert.Equal("\n", doc.LineEnding);

        var saved = doc.Encode("x\r\ny\r\nz\n");
        Assert.Equal(Encoding.UTF8.GetBytes("x\ny\nz\n"), saved);
    }

    [Fact]
    public void Invalid_utf8_falls_back_to_latin1_and_round_trips_bytes()
    {
        var original = new byte[] { 0x6E, 0x61, 0x6D, 0x65, 0x3D, 0xE9, 0x0D, 0x0A };
        var doc = TextFileDocument.Parse(original);
        Assert.Equal(original, doc.Encode(doc.Text));
    }

    [Fact]
    public void Save_writes_file_atomically_with_original_format()
    {
        var path = Path.Combine(_temp, "server.cfg");
        File.WriteAllBytes(path, new byte[] { 0xEF, 0xBB, 0xBF }.Concat(Encoding.UTF8.GetBytes("one\r\n")).ToArray());

        var doc = TextFileDocument.Load(path);
        doc.Save(path, doc.Text + "two\n");

        var bytes = File.ReadAllBytes(path);
        Assert.Equal(new byte[] { 0xEF, 0xBB, 0xBF }.Concat(Encoding.UTF8.GetBytes("one\r\ntwo\r\n")).ToArray(), bytes);
        Assert.False(File.Exists(path + ".mpsp-tmp"));
    }

    [Theory]
    [InlineData("a\r\nb\r\nc\n", "\r\n")]
    [InlineData("a\nb\nc\r\n", "\n")]
    [InlineData("a\rb\rc", "\r")]
    public void DetectLineEnding_picks_dominant_style(string text, string expected)
    {
        Assert.Equal(expected, TextFileDocument.DetectLineEnding(text));
    }
}
