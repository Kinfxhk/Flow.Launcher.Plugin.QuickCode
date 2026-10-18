namespace Flow.Launcher.Plugin.QuickCode;

/// <summary>Enumerates .lnk and .url shortcuts from the two Start Menu "Programs" folders.</summary>
public sealed class StartMenuIndexer
{
    public IReadOnlyList<Shortcut> Enumerate()
    {
        var results = new List<Shortcut>();
        var roots = new[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "Programs"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs"),
        };

        foreach (var root in roots)
            if (Directory.Exists(root))
                EnumerateDirectory(root, results);

        return results;
    }

    private static void EnumerateDirectory(string dir, List<Shortcut> results)
    {
        try
        {
            foreach (var file in Directory.EnumerateFiles(dir))
            {
                var ext = Path.GetExtension(file);
                if (string.Equals(ext, ".lnk", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(ext, ".url", StringComparison.OrdinalIgnoreCase))
                {
                    results.Add(new Shortcut(Path.GetFileNameWithoutExtension(file), file));
                }
            }
        }
        catch (Exception)
        {
            // Ignore unreadable folders; keep indexing the rest.
        }

        IEnumerable<string> subdirs;
        try
        {
            subdirs = Directory.EnumerateDirectories(dir);
        }
        catch (Exception)
        {
            return;
        }

        foreach (var sub in subdirs)
            EnumerateDirectory(sub, results);
    }
}
