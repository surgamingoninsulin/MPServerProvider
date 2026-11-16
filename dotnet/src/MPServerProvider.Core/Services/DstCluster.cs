namespace MPServerProvider.Core.Services;

public static class DstCluster
{
    public const string GameId = "dont-starve-together";
    public const string KleiLoginUrl = "https://accounts.klei.com/login";

    public static string ClusterDir(string serverRoot)
        => Path.Combine(serverRoot, "data", "DoNotStarveTogether", "Cluster_1");

    public static string TokenPath(string serverRoot) => Path.Combine(ClusterDir(serverRoot), "cluster_token.txt");

    public static string? ReadToken(string serverRoot)
    {
        try
        {
            var path = TokenPath(serverRoot);
            var token = File.Exists(path) ? File.ReadAllText(path).Trim() : null;
            return string.IsNullOrEmpty(token) ? null : token;
        }
        catch
        {
            return null;
        }
    }

    public static bool LooksLikeToken(string? token)
        => token is not null
           && token.Trim().Length is >= 20 and <= 512
           && !token.Trim().Any(c => char.IsWhiteSpace(c) || char.IsControl(c));

    public static void SaveToken(string serverRoot, string token, string serverName, int port)
    {
        if (!LooksLikeToken(token))
        {
            throw new ArgumentException("That doesn't look like a Klei cluster token. Copy the whole token from the Game Servers page.", nameof(token));
        }

        var cluster = ClusterDir(serverRoot);
        Directory.CreateDirectory(Path.Combine(cluster, "Master"));
        File.WriteAllText(TokenPath(serverRoot), token.Trim());

        var clusterIni = Path.Combine(cluster, "cluster.ini");
        if (!File.Exists(clusterIni))
        {
            var name = new string(serverName.Where(c => !char.IsControl(c)).ToArray()).Trim();
            File.WriteAllText(clusterIni, string.Join(Environment.NewLine,
                "[GAMEPLAY]",
                "game_mode = survival",
                "max_players = 6",
                "pvp = false",
                "pause_when_empty = true",
                "",
                "[NETWORK]",
                $"cluster_name = {(name.Length == 0 ? "MP Server Provider" : name)}",
                "cluster_description = ",
                "cluster_password = ",
                "cluster_intention = cooperative",
                "",
                "[MISC]",
                "console_enabled = true",
                "",
                "[SHARD]",
                "shard_enabled = false",
                ""));
        }

        var serverIni = Path.Combine(cluster, "Master", "server.ini");
        if (!File.Exists(serverIni))
        {
            File.WriteAllText(serverIni, string.Join(Environment.NewLine,
                "[NETWORK]",
                $"server_port = {port}",
                "",
                "[SHARD]",
                "is_master = true",
                ""));
        }
    }
}
