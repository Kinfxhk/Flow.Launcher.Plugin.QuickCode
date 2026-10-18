using System.Text;
using Flow.Launcher.Plugin.QuickCode;

// Console harness for the "real check". NOT shipped in the plugin zip.
Console.OutputEncoding = Encoding.UTF8;

var table = CangjieTable.Load();
var indexer = new StartMenuIndexer();
var items = indexer.Enumerate();

Console.WriteLine($"Start Menu shortcuts indexed: {items.Count}");

var chineseNamed = items
    .Where(i => table.GetHanChars(i.Name).Count > 0)
    .OrderBy(i => i.Name, StringComparer.Ordinal)
    .ToList();

Console.WriteLine($"Chinese-named shortcuts: {chineseNamed.Count}");
foreach (var i in chineseNamed)
{
    var codes = table.GetHanCodes(i.Name);
    var quick = string.Join(" ", codes.Select(c => c.Quick));
    var cj = string.Join(" ", codes.Select(c => c.Cangjie));
    Console.WriteLine($"  {i.Name}\t速成={quick}\t倉頡={cj}\t{i.Path}");
}
Console.WriteLine();

var picks = chineseNamed.Take(3).ToList();

var queries = new List<string> { "yu jn dm", "yujndm" };
foreach (var p in picks)
{
    var codes = table.GetHanCodes(p.Name);
    queries.Add(string.Join(" ", codes.Select(c => c.Quick)));
    queries.Add(string.Concat(codes.Select(c => c.Quick)));
    queries.Add(string.Join(" ", codes.Select(c => c.Cangjie)));
}

foreach (var q in queries.Distinct())
{
    Console.WriteLine($"=== query: '{q}' ===");
    var matches = Matcher.Find(items, table, q).Take(5).ToList();
    foreach (var m in matches)
        Console.WriteLine($"  [{m.Score}] {m.Item.Name}  ({m.Item.Path})");
    if (matches.Count == 0)
        Console.WriteLine("  (no matches)");
    Console.WriteLine();
}
