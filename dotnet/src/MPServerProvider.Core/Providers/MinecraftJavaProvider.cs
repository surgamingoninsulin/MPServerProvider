using System.Text.Json;
using MPServerProvider.Core.Models;

namespace MPServerProvider.Core.Providers;

public sealed class MinecraftJavaProvider : IInstallProvider
{
    private const string MojangManifest = "https://piston-meta.mojang.com/mc/game/version_manifest_v2.json";
    private const string PaperApi = "https://fill.papermc.io/v3/projects/paper";
    private const string FabricMeta = "https://meta.fabricmc.net/v2";
    private const string PurpurApi = "https://api.purpurmc.org/v2/purpur";
    private const string VelocityApi = "https://fill.papermc.io/v3/projects/velocity";
    private const string ForgePromos = "https://files.minecraftforge.net/net/minecraftforge/forge/promotions_slim.json";
    private const string ForgeMavenMeta = "https://files.minecraftforge.net/maven/net/minecraftforge/forge/maven-metadata.xml";
    private const string BungeeCordUrl = "https://ci.md-5.net/job/BungeeCord/lastStableBuild/artifact/bootstrap/target/BungeeCord.jar";

    public string Id => "minecraft-java";

    internal static int GuessJavaMajor(string mcVersion)
    {
        var parts = mcVersion
            .Split('.', '-', ' ')
            .Select(p => int.TryParse(p, out var v) ? v : (int?)null)
            .Where(v => v is not null)
            .Select(v => v!.Value)
            .ToArray();

        // Mojang switched to year-based numbering (26.1, 26.2, ...) in 2026; 26.1+ needs Java 25.
        if (parts.Length >= 1 && parts[0] >= 26)
        {
            return 25;
        }

        if (parts.Length >= 3 && parts[0] == 1 && (parts[1] >= 21 || (parts[1] == 20 && parts[2] >= 5)))
        {
            return 21;
        }

        if (parts.Length >= 2 && parts[0] == 1 && parts[1] >= 18)
        {
            return 17;
        }

        if (parts.Length >= 2 && parts[0] == 1 && parts[1] == 17)
        {
            return 16;
        }

        return 8;
    }

    private static void WriteCommonFiles(InstallRequest request)
    {
        if (request.EulaAccepted)
        {
            var eulaPath = Path.Combine(request.TargetDir, "eula.txt");
            if (!File.Exists(eulaPath))
            {
                File.WriteAllText(
                    eulaPath,
                    "# Accepted via MP Server Provider install dialog (https://aka.ms/MinecraftEULA)\neula=true\n");
            }
        }

        var propsPath = Path.Combine(request.TargetDir, "server.properties");
        if (!File.Exists(propsPath))
        {
            File.WriteAllText(propsPath, $"server-port={request.Port}\nmotd={ConfigText.JavaPropertiesValue(request.ServerName)}\n");
        }
    }

    private static async Task<InstallResult> InstallVanillaAsync(InstallRequest request, IProgress<InstallProgress> progress, CancellationToken ct)
    {
        progress.Report(InstallProgress.Indeterminate("Resolving version from Mojang…"));

        using var manifest = JsonDocument.Parse(await DownloadHelper.GetStringAsync(MojangManifest, ct).ConfigureAwait(false));
        var requestedVersion = request.Version ?? string.Empty;

        JsonElement? versionEntry = null;
        if (manifest.RootElement.TryGetProperty("versions", out var versions) && versions.ValueKind == JsonValueKind.Array)
        {
            foreach (var v in versions.EnumerateArray())
            {
                if (v.TryGetProperty("id", out var idEl) && idEl.GetString() == requestedVersion)
                {
                    versionEntry = v;
                    break;
                }
            }
        }

        if (versionEntry is null)
        {
            throw new InvalidOperationException($"Minecraft version {requestedVersion} not found in Mojang's manifest.");
        }

        var versionUrl = versionEntry.Value.TryGetProperty("url", out var urlEl) ? urlEl.GetString() : null;
        if (versionUrl is null)
        {
            throw new InvalidOperationException("Mojang manifest entry has no url.");
        }

        using var versionDoc = JsonDocument.Parse(await DownloadHelper.GetStringAsync(versionUrl, ct).ConfigureAwait(false));
        if (!versionDoc.RootElement.TryGetProperty("downloads", out var downloads) || !downloads.TryGetProperty("server", out var serverDownload))
        {
            throw new InvalidOperationException($"Minecraft {requestedVersion} has no server jar.");
        }

        var jarUrl = serverDownload.TryGetProperty("url", out var jarUrlEl) ? jarUrlEl.GetString() : null;
        if (jarUrl is null)
        {
            throw new InvalidOperationException($"Minecraft {requestedVersion} server jar has no url.");
        }

        var javaMajor = ReadMojangJavaMajor(versionDoc.RootElement) ?? GuessJavaMajor(requestedVersion);

        // Mojang publishes the jar's SHA-1 alongside its URL; verify it.
        var sha1 = serverDownload.TryGetProperty("sha1", out var sha1El) && sha1El.ValueKind == JsonValueKind.String
            ? sha1El.GetString()
            : null;

        var jarPath = Path.Combine(request.TargetDir, "server.jar");
        progress.Report(InstallProgress.Indeterminate($"Downloading Minecraft {requestedVersion} server"));
        await DownloadHelper.DownloadFileAsync(jarUrl, jarPath, ct, expectedSha1: sha1).ConfigureAwait(false);

        return new InstallResult
        {
            ServerFile = "server.jar",
            Version = requestedVersion,
            StartCommand = null,
            JavaMajor = javaMajor,
        };
    }

    private static async Task<InstallResult> InstallPaperAsync(InstallRequest request, IProgress<InstallProgress> progress, CancellationToken ct)
    {
        progress.Report(InstallProgress.Indeterminate("Resolving latest Paper build…"));
        var mcVersion = await ResolveFillVersionAsync(PaperApi, request.Version, stableOnly: true, ct).ConfigureAwait(false);

        using var build = JsonDocument.Parse(
            await DownloadHelper.GetStringAsync($"{PaperApi}/versions/{mcVersion}/builds/latest", ct).ConfigureAwait(false));
        var buildId = build.RootElement.TryGetProperty("id", out var idEl) ? idEl.GetInt64() : 0;

        if (!build.RootElement.TryGetProperty("downloads", out var downloads) || !downloads.TryGetProperty("server:default", out var download))
        {
            throw new InvalidOperationException("Paper build has no server:default download.");
        }

        var name = SafeJarName(download, "paper.jar");
        var url = download.TryGetProperty("url", out var urlEl) ? urlEl.GetString() : null;
        if (url is null)
        {
            throw new InvalidOperationException("Paper build download has no url.");
        }

        var jarPath = Path.Combine(request.TargetDir, name);
        progress.Report(InstallProgress.Indeterminate($"Downloading Paper {mcVersion} build {buildId}"));
        await DownloadHelper.DownloadFileAsync(url, jarPath, ct, expectedSha256: FillSha256(download)).ConfigureAwait(false);

        var javaMajor = GuessJavaMajor(mcVersion);
        try
        {
            using var versionDoc = JsonDocument.Parse(
                await DownloadHelper.GetStringAsync($"{PaperApi}/versions/{mcVersion}", ct).ConfigureAwait(false));
            if (versionDoc.RootElement.TryGetProperty("java", out var java)
                && java.TryGetProperty("version", out var javaVersion)
                && javaVersion.TryGetProperty("minimum", out var minimumEl))
            {
                javaMajor = minimumEl.GetInt32();
            }
        }
        catch
        {
            // Fall back to the heuristic on any shape/network failure
            // (tolerant `?? GuessJavaMajor(...)` fallback).
        }

        return new InstallResult
        {
            ServerFile = name,
            Version = $"{mcVersion} (build {buildId})",
            StartCommand = null,
            JavaMajor = javaMajor,
        };
    }

    private static async Task<InstallResult> InstallPurpurAsync(InstallRequest request, IProgress<InstallProgress> progress, CancellationToken ct)
    {
        var mcVersion = request.Version ?? string.Empty;
        progress.Report(InstallProgress.Indeterminate("Resolving latest Purpur build…"));

        using var doc = JsonDocument.Parse(
            await DownloadHelper.GetStringAsync($"{PurpurApi}/{mcVersion}/latest", ct).ConfigureAwait(false));
        var build = doc.RootElement.TryGetProperty("build", out var buildEl) ? buildEl.GetString() : null;
        if (build is null)
        {
            throw new InvalidOperationException($"Purpur returned no latest build for {mcVersion}.");
        }

        var url = $"{PurpurApi}/{mcVersion}/{build}/download";
        var jarPath = Path.Combine(request.TargetDir, "purpur_server.jar");

        progress.Report(InstallProgress.Indeterminate($"Downloading Purpur {mcVersion} build {build}"));
        await DownloadHelper.DownloadFileAsync(url, jarPath, ct).ConfigureAwait(false);

        return new InstallResult
        {
            ServerFile = "purpur_server.jar",
            Version = $"{mcVersion} (build {build})",
            StartCommand = null,
            // Purpur is a Paper fork for the same Minecraft version: same Java requirement.
            JavaMajor = await FetchVanillaJavaMajorAsync(mcVersion, ct).ConfigureAwait(false) ?? GuessJavaMajor(mcVersion),
        };
    }

    private static async Task<InstallResult> InstallVelocityAsync(InstallRequest request, IProgress<InstallProgress> progress, CancellationToken ct)
    {
        progress.Report(InstallProgress.Indeterminate("Resolving latest Velocity build…"));
        var mcVersion = await ResolveFillVersionAsync(VelocityApi, request.Version, stableOnly: false, ct).ConfigureAwait(false);

        using var build = JsonDocument.Parse(
            await DownloadHelper.GetStringAsync($"{VelocityApi}/versions/{mcVersion}/builds/latest", ct).ConfigureAwait(false));
        if (!build.RootElement.TryGetProperty("downloads", out var downloads) || !downloads.TryGetProperty("server:default", out var download))
        {
            throw new InvalidOperationException("Velocity build has no server:default download.");
        }

        var name = SafeJarName(download, "velocity.jar");
        var url = download.TryGetProperty("url", out var urlEl) ? urlEl.GetString() : null;
        if (url is null)
        {
            throw new InvalidOperationException("Velocity build download has no url.");
        }

        var jarPath = Path.Combine(request.TargetDir, name);
        progress.Report(InstallProgress.Indeterminate($"Downloading Velocity {mcVersion}"));
        await DownloadHelper.DownloadFileAsync(url, jarPath, ct, expectedSha256: FillSha256(download)).ConfigureAwait(false);

        return new InstallResult
        {
            ServerFile = name,
            Version = mcVersion,
            StartCommand = null,
            // Velocity's own requirement, not Minecraft's: 4.x needs Java 21, 3.x needs Java 17.
            // (Left at 0 the server fell back to whatever default Java was found.)
            JavaMajor = mcVersion.StartsWith("3.", StringComparison.Ordinal) ? 17 : 21,
        };
    }

    private static async Task<InstallResult> InstallBungeeCordAsync(InstallRequest request, IProgress<InstallProgress> progress, CancellationToken ct)
    {
        progress.Report(InstallProgress.Indeterminate("Downloading BungeeCord…"));
        var jarPath = Path.Combine(request.TargetDir, "BungeeCord.jar");
        await DownloadHelper.DownloadFileAsync(BungeeCordUrl, jarPath, ct).ConfigureAwait(false);

        return new InstallResult
        {
            ServerFile = "BungeeCord.jar",
            Version = "latest",
            StartCommand = null,
            JavaMajor = 0,
        };
    }

    private static async Task<InstallResult> InstallForgeAsync(InstallRequest request, IProgress<InstallProgress> progress, CancellationToken ct)
    {
        var mcVersion = request.Version ?? string.Empty;
        progress.Report(InstallProgress.Indeterminate("Resolving Forge version…"));

        using var promos = JsonDocument.Parse(await DownloadHelper.GetStringAsync(ForgePromos, ct).ConfigureAwait(false));
        string? forgeVersion = null;
        if (promos.RootElement.TryGetProperty("promos", out var promosObj))
        {
            if (promosObj.TryGetProperty($"{mcVersion}-recommended", out var recommendedEl))
            {
                forgeVersion = recommendedEl.GetString();
            }
            else if (promosObj.TryGetProperty($"{mcVersion}-latest", out var latestEl))
            {
                forgeVersion = latestEl.GetString();
            }
        }

        if (forgeVersion is null)
        {
            var xml = await DownloadHelper.GetStringAsync(ForgeMavenMeta, ct).ConfigureAwait(false);
            var prefix = $"{mcVersion}-";
            var candidates = new List<string>();
            var rest = xml;
            while (true)
            {
                var start = rest.IndexOf("<version>", StringComparison.Ordinal);
                if (start < 0)
                {
                    break;
                }

                rest = rest[(start + "<version>".Length)..];
                var end = rest.IndexOf("</version>", StringComparison.Ordinal);
                if (end < 0)
                {
                    break;
                }

                var full = rest[..end].Trim();
                rest = rest[(end + "</version>".Length)..];
                if (full.StartsWith(prefix, StringComparison.Ordinal))
                {
                    candidates.Add(full[prefix.Length..]);
                }
            }

            if (candidates.Count == 0)
            {
                throw new InvalidOperationException($"No Forge version found for Minecraft {mcVersion}.");
            }

            forgeVersion = PickNewestVersion(candidates);
        }

        if (forgeVersion is null || !IsSafeVersionToken(mcVersion) || !IsSafeVersionToken(forgeVersion))
        {
            throw new InvalidOperationException($"Unexpected Forge version '{mcVersion}-{forgeVersion}'.");
        }

        var url = $"https://maven.minecraftforge.net/net/minecraftforge/forge/{mcVersion}-{forgeVersion}/forge-{mcVersion}-{forgeVersion}-installer.jar";
        var jarPath = Path.Combine(request.TargetDir, "forge-installer.jar");

        progress.Report(InstallProgress.Indeterminate($"Downloading Forge {mcVersion}-{forgeVersion} installer…"));
        await DownloadHelper.DownloadFileAsync(url, jarPath, ct).ConfigureAwait(false);

        // The installer jar is not a server: actually run it so it downloads the libraries and
        // generates the launch files (run.bat + win_args.txt on 1.17+, a forge-*.jar before).
        var javaMajor = await FetchVanillaJavaMajorAsync(mcVersion, ct).ConfigureAwait(false) ?? GuessJavaMajor(mcVersion);
        var java = await new JavaRuntimeProvider().ResolveAsync(javaMajor, progress, ct).ConfigureAwait(false);

        progress.Report(InstallProgress.Indeterminate($"Running the Forge {mcVersion}-{forgeVersion} installer (downloads libraries; this can take a few minutes)…"));
        var run = await ExternalProcess.RunAsync(
            java,
            new[] { "-jar", "forge-installer.jar", "--installServer" },
            request.TargetDir,
            line => progress.Report(InstallProgress.Indeterminate($"Forge: {Truncate(line, 120)}")),
            ForgeInstallerTimeout,
            ct).ConfigureAwait(false);
        if (run.ExitCode != 0)
        {
            var lastLine = run.Output.Split('\n').Select(l => l.Trim()).LastOrDefault(l => l.Length > 0);
            throw new InvalidOperationException(
                $"The Forge installer failed (exit code {run.ExitCode})" + (lastLine is null ? "." : $": {lastLine}"));
        }

        var (serverFile, startCommand) = DetectForgeLaunch(request.TargetDir, mcVersion, forgeVersion)
            ?? throw new InvalidOperationException(
                "The Forge installer finished but produced no runnable server (no win_args.txt or forge server jar was found).");

        DownloadHelper.TryDeleteFile(jarPath);

        return new InstallResult
        {
            ServerFile = serverFile,
            Version = $"{mcVersion}-{forgeVersion}",
            StartCommand = startCommand,
            JavaMajor = javaMajor,
        };
    }

    internal static readonly TimeSpan ForgeInstallerTimeout = TimeSpan.FromMinutes(30);

    internal static (string ServerFile, string? StartCommand)? DetectForgeLaunch(string targetDir, string mcVersion, string forgeVersion)
    {
        var argsRel = Path.Combine("libraries", "net", "minecraftforge", "forge", $"{mcVersion}-{forgeVersion}", "win_args.txt");
        if (File.Exists(Path.Combine(targetDir, argsRel)))
        {
            var jvmArgs = File.Exists(Path.Combine(targetDir, "user_jvm_args.txt")) ? "@user_jvm_args.txt " : string.Empty;
            var argsForward = argsRel.Replace('\\', '/');
            return (argsRel, $"-Xms{{ram_min_mb}}M -Xmx{{ram_max_mb}}M {jvmArgs}@{argsForward} nogui");
        }

        foreach (var candidate in new[]
        {
            $"forge-{mcVersion}-{forgeVersion}.jar",
            $"forge-{mcVersion}-{forgeVersion}-universal.jar",
            $"forge-{mcVersion}-{forgeVersion}-shim.jar",
        })
        {
            if (File.Exists(Path.Combine(targetDir, candidate)))
            {
                return (candidate, null);
            }
        }

        var fallback = Directory.Exists(targetDir)
            ? Directory.EnumerateFiles(targetDir, "forge-*.jar")
                .Select(Path.GetFileName)
                .FirstOrDefault(n => n is not null && !n.Contains("installer", StringComparison.OrdinalIgnoreCase))
            : null;
        return fallback is null ? null : (fallback, null);
    }

    internal static string? PickNewestVersion(IEnumerable<string> versions)
    {
        string? best = null;
        foreach (var v in versions)
        {
            if (best is null || CompareVersions(v, best) > 0)
            {
                best = v;
            }
        }

        return best;
    }

    internal static int CompareVersions(string a, string b)
    {
        var pa = a.Split('.', '-');
        var pb = b.Split('.', '-');
        for (var i = 0; i < Math.Max(pa.Length, pb.Length); i++)
        {
            var sa = i < pa.Length ? pa[i] : "0";
            var sb = i < pb.Length ? pb[i] : "0";
            int cmp;
            if (long.TryParse(sa, out var na) && long.TryParse(sb, out var nb))
            {
                cmp = na.CompareTo(nb);
            }
            else
            {
                cmp = string.CompareOrdinal(sa, sb);
            }

            if (cmp != 0)
            {
                return cmp;
            }
        }

        return 0;
    }

    private static bool IsSafeVersionToken(string value)
        => value.Length is > 0 and <= 64 && value.All(c => char.IsAsciiLetterOrDigit(c) || c is '.' or '-' or '_' or '+');

    private static string Truncate(string value, int max) => value.Length <= max ? value : value[..max] + "…";

    private static string SafeJarName(JsonElement download, string fallback)
    {
        var raw = download.TryGetProperty("name", out var nameEl) && nameEl.ValueKind == JsonValueKind.String ? nameEl.GetString() : null;
        var name = DownloadHelper.SafeFileName(raw, fallback);
        return name.EndsWith(".jar", StringComparison.OrdinalIgnoreCase) ? name : fallback;
    }

    private static string? FillSha256(JsonElement download)
        => download.TryGetProperty("checksums", out var sums) && sums.ValueKind == JsonValueKind.Object
            && sums.TryGetProperty("sha256", out var sha) && sha.ValueKind == JsonValueKind.String
            ? sha.GetString()
            : null;

    private static async Task<InstallResult> InstallFabricAsync(InstallRequest request, IProgress<InstallProgress> progress, CancellationToken ct)
    {
        var mcVersion = request.Version ?? string.Empty;
        progress.Report(InstallProgress.Indeterminate("Resolving Fabric loader…"));

        using var loaders = JsonDocument.Parse(
            await DownloadHelper.GetStringAsync($"{FabricMeta}/versions/loader", ct).ConfigureAwait(false));
        var loaderVersion = loaders.RootElement.ValueKind == JsonValueKind.Array && loaders.RootElement.GetArrayLength() > 0
            && loaders.RootElement[0].TryGetProperty("version", out var loaderVersionEl)
            ? loaderVersionEl.GetString()
            : null;
        if (loaderVersion is null)
        {
            throw new InvalidOperationException("Fabric meta returned no loader versions.");
        }

        using var installers = JsonDocument.Parse(
            await DownloadHelper.GetStringAsync($"{FabricMeta}/versions/installer", ct).ConfigureAwait(false));
        var installerVersion = installers.RootElement.ValueKind == JsonValueKind.Array && installers.RootElement.GetArrayLength() > 0
            && installers.RootElement[0].TryGetProperty("version", out var installerVersionEl)
            ? installerVersionEl.GetString()
            : null;
        if (installerVersion is null)
        {
            throw new InvalidOperationException("Fabric meta returned no installer versions.");
        }

        var url = $"{FabricMeta}/versions/loader/{mcVersion}/{loaderVersion}/{installerVersion}/server/jar";
        var jarPath = Path.Combine(request.TargetDir, "fabric_server.jar");

        progress.Report(InstallProgress.Indeterminate($"Downloading Fabric {loaderVersion} server launcher"));
        await DownloadHelper.DownloadFileAsync(url, jarPath, ct).ConfigureAwait(false);

        Directory.CreateDirectory(Path.Combine(request.TargetDir, "mods"));

        // Fabric runs on top of vanilla's Java requirement for the same Minecraft version, so
        // reuse Mojang's per-version manifest (same as InstallVanillaAsync) when reachable,
        // falling back to the heuristic.
        var javaMajor = await FetchVanillaJavaMajorAsync(mcVersion, ct).ConfigureAwait(false) ?? GuessJavaMajor(mcVersion);

        return new InstallResult
        {
            ServerFile = "fabric_server.jar",
            Version = $"{mcVersion} (loader {loaderVersion})",
            StartCommand = null,
            JavaMajor = javaMajor,
        };
    }

    private static async Task<int?> FetchVanillaJavaMajorAsync(string mcVersion, CancellationToken ct)
    {
        try
        {
            using var manifest = JsonDocument.Parse(await DownloadHelper.GetStringAsync(MojangManifest, ct).ConfigureAwait(false));
            if (!manifest.RootElement.TryGetProperty("versions", out var versions))
            {
                return null;
            }

            string? versionUrl = null;
            foreach (var v in versions.EnumerateArray())
            {
                if (v.TryGetProperty("id", out var idEl) && idEl.GetString() == mcVersion)
                {
                    versionUrl = v.TryGetProperty("url", out var urlEl) ? urlEl.GetString() : null;
                    break;
                }
            }

            if (versionUrl is null)
            {
                return null;
            }

            using var versionDoc = JsonDocument.Parse(await DownloadHelper.GetStringAsync(versionUrl, ct).ConfigureAwait(false));
            return ReadMojangJavaMajor(versionDoc.RootElement);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            return null;
        }
    }

    internal static int? ReadMojangJavaMajor(JsonElement versionRoot)
    {
        static int? Read(JsonElement owner)
            => owner.ValueKind == JsonValueKind.Object
               && owner.TryGetProperty("javaVersion", out var javaVersion)
               && javaVersion.ValueKind == JsonValueKind.Object
               && javaVersion.TryGetProperty("majorVersion", out var majorEl)
               && majorEl.ValueKind == JsonValueKind.Number
               && majorEl.TryGetInt32(out var major)
               && major > 0
                ? major
                : null;

        if (Read(versionRoot) is { } rootMajor)
        {
            return rootMajor;
        }

        // Tolerate the old (never-observed) nested shape too.
        return versionRoot.ValueKind == JsonValueKind.Object
               && versionRoot.TryGetProperty("downloads", out var downloads)
               && downloads.ValueKind == JsonValueKind.Object
               && downloads.TryGetProperty("server", out var server)
            ? Read(server)
            : null;
    }

    public async Task<List<string>> GetVersionsAsync(GameDefinition game, string? variant, CancellationToken ct)
    {
        switch (variant)
        {
            case "paper":
            {
                using var doc = JsonDocument.Parse(await DownloadHelper.GetStringAsync(PaperApi, ct).ConfigureAwait(false));
                return FillVersions(doc.RootElement, stableOnly: true);
            }

            case "fabric":
            {
                using var doc = JsonDocument.Parse(
                    await DownloadHelper.GetStringAsync($"{FabricMeta}/versions/game", ct).ConfigureAwait(false));
                var result = new List<string>();
                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var v in doc.RootElement.EnumerateArray())
                    {
                        // Snapshots used to fill the top of the list, so the default pick was
                        // an unstable snapshot instead of the current release.
                        if (v.TryGetProperty("stable", out var stableEl) && stableEl.ValueKind == JsonValueKind.False)
                        {
                            continue;
                        }

                        if (v.TryGetProperty("version", out var versionEl) && versionEl.GetString() is { } s)
                        {
                            result.Add(s);
                        }
                    }
                }

                return result;
            }

            case "purpur":
            {
                using var doc = JsonDocument.Parse(await DownloadHelper.GetStringAsync(PurpurApi, ct).ConfigureAwait(false));
                // Purpur lists oldest first; every other variant shows newest first.
                var result = ExtractStringArray(doc.RootElement, "versions");
                result.Sort(CompareVersions);
                result.Reverse();
                return result;
            }

            case "velocity":
            {
                // Velocity's current releases are all -SNAPSHOT builds, so keep them.
                using var doc = JsonDocument.Parse(await DownloadHelper.GetStringAsync(VelocityApi, ct).ConfigureAwait(false));
                var result = FillVersions(doc.RootElement, stableOnly: false);
                return result.Count > 0 ? result : new List<string> { "latest" };
            }

            case "bungeecord":
                return new List<string> { "latest" };

            case "forge":
            {
                using var doc = JsonDocument.Parse(await DownloadHelper.GetStringAsync(ForgePromos, ct).ConfigureAwait(false));
                var versions = new HashSet<string>();
                if (doc.RootElement.TryGetProperty("promos", out var promos) && promos.ValueKind == JsonValueKind.Object)
                {
                    foreach (var prop in promos.EnumerateObject())
                    {
                        var mcVersion = prop.Name.Split('-')[0];
                        versions.Add(mcVersion);
                    }
                }

                var sorted = versions.ToList();
                sorted.Sort(CompareVersions);
                sorted.Reverse();
                return sorted;
            }

            default:
            {
                using var doc = JsonDocument.Parse(await DownloadHelper.GetStringAsync(MojangManifest, ct).ConfigureAwait(false));
                var result = new List<string>();
                if (doc.RootElement.TryGetProperty("versions", out var versions) && versions.ValueKind == JsonValueKind.Array)
                {
                    foreach (var v in versions.EnumerateArray())
                    {
                        if (v.TryGetProperty("type", out var typeEl) && typeEl.GetString() == "release"
                            && v.TryGetProperty("id", out var idEl) && idEl.GetString() is { } id)
                        {
                            result.Add(id);
                        }
                    }
                }

                return result;
            }
        }
    }

    internal static List<string> FillVersions(JsonElement root, bool stableOnly)
    {
        var result = new List<string>();
        if (!root.TryGetProperty("versions", out var versions))
        {
            return result;
        }

        IEnumerable<JsonElement> entries = versions.ValueKind switch
        {
            JsonValueKind.Object => versions.EnumerateObject().SelectMany(group =>
                group.Value.ValueKind == JsonValueKind.Array ? group.Value.EnumerateArray().ToArray() : Array.Empty<JsonElement>()),
            JsonValueKind.Array => versions.EnumerateArray().Reverse().ToArray(),
            _ => Array.Empty<JsonElement>(),
        };

        foreach (var entry in entries)
        {
            if (entry.ValueKind != JsonValueKind.String || entry.GetString() is not { Length: > 0 } version)
            {
                continue;
            }

            if (stableOnly && (version.Contains("-pre", StringComparison.OrdinalIgnoreCase)
                || version.Contains("-rc", StringComparison.OrdinalIgnoreCase)
                || version.Contains("snapshot", StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            if (!result.Contains(version))
            {
                result.Add(version);
            }
        }

        return result;
    }

    private static async Task<string> ResolveFillVersionAsync(string api, string? requested, bool stableOnly, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(requested) && !requested.Equals("latest", StringComparison.OrdinalIgnoreCase))
        {
            return requested.Trim();
        }

        using var doc = JsonDocument.Parse(await DownloadHelper.GetStringAsync(api, ct).ConfigureAwait(false));
        return FillVersions(doc.RootElement, stableOnly).FirstOrDefault()
            ?? throw new InvalidOperationException("PaperMC returned no versions for this project.");
    }

    private static List<string> ExtractStringArray(JsonElement root, string propertyName)
    {
        var result = new List<string>();
        if (root.TryGetProperty(propertyName, out var arr) && arr.ValueKind == JsonValueKind.Array)
        {
            foreach (var v in arr.EnumerateArray())
            {
                if (v.ValueKind == JsonValueKind.String)
                {
                    result.Add(v.GetString()!);
                }
            }
        }

        return result;
    }

    public async Task<InstallResult> InstallAsync(InstallRequest request, IProgress<InstallProgress> progress, CancellationToken ct)
    {
        Directory.CreateDirectory(request.TargetDir);

        var result = request.Variant switch
        {
            "paper" => await InstallPaperAsync(request, progress, ct).ConfigureAwait(false),
            "fabric" => await InstallFabricAsync(request, progress, ct).ConfigureAwait(false),
            "purpur" => await InstallPurpurAsync(request, progress, ct).ConfigureAwait(false),
            "velocity" => await InstallVelocityAsync(request, progress, ct).ConfigureAwait(false),
            "bungeecord" => await InstallBungeeCordAsync(request, progress, ct).ConfigureAwait(false),
            "forge" => await InstallForgeAsync(request, progress, ct).ConfigureAwait(false),
            _ => await InstallVanillaAsync(request, progress, ct).ConfigureAwait(false),
        };

        WriteCommonFiles(request);
        return result;
    }
}
