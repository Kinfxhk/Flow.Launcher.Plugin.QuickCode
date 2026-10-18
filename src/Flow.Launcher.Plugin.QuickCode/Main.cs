using System.Diagnostics;
using Flow.Launcher.Plugin;

namespace Flow.Launcher.Plugin.QuickCode;

public class Main : IAsyncPlugin, IAsyncReloadable, IContextMenu
{
    private static readonly TimeSpan ReindexInterval = TimeSpan.FromMinutes(10);

    private CangjieTable _table = new(new Dictionary<string, string>());
    private readonly StartMenuIndexer _indexer = new();
    private IReadOnlyList<Shortcut> _shortcuts = Array.Empty<Shortcut>();
    private DateTime _lastIndexUtc = DateTime.MinValue;

    public Task InitAsync(PluginInitContext context)
    {
        _table = CangjieTable.Load();
        return ReloadDataAsync();
    }

    public async Task<List<Result>> QueryAsync(Query query, CancellationToken token)
    {
        if (DateTime.UtcNow - _lastIndexUtc > ReindexInterval)
            await ReloadDataAsync();

        var search = query.Search?.Trim() ?? string.Empty;
        if (search.Length == 0)
            return new List<Result>();

        var matches = Matcher.Find(_shortcuts, _table, search);
        var results = new List<Result>(matches.Count);
        foreach (var m in matches)
            results.Add(BuildResult(m.Item, _table, m.Score));
        return results;
    }

    public Task ReloadDataAsync()
    {
        _shortcuts = _indexer.Enumerate();
        _lastIndexUtc = DateTime.UtcNow;
        return Task.CompletedTask;
    }

    public List<Result> LoadContextMenus(Result selectedResult)
    {
        var path = selectedResult.ContextData as string;
        if (string.IsNullOrEmpty(path))
            return new List<Result>();

        return new List<Result>
        {
            new Result
            {
                Title = "開啟檔案位置",
                IcoPath = "Images\\icon.png",
                Action = _ => OpenFileLocation(path),
            },
        };
    }

    private static Result BuildResult(Shortcut item, CangjieTable table, int score)
    {
        var codes = table.GetHanCodes(item.Name);
        var subtitle = codes.Count > 0
            ? $"速成: {string.Join(" ", codes.Select(c => c.Quick))}  倉頡: {string.Join(" ", codes.Select(c => c.Cangjie))}  |  {item.Path}"
            : item.Path;

        return new Result
        {
            Title = item.Name,
            SubTitle = subtitle,
            IcoPath = "Images\\icon.png",
            Score = score,
            ContextData = item.Path,
            Action = _ => Launch(item.Path),
        };
    }

    private static bool Launch(string path)
    {
        try
        {
            Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static bool OpenFileLocation(string path)
    {
        try
        {
            Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{path}\"") { UseShellExecute = true });
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
