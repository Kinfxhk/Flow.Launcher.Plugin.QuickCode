using System.Text;

namespace Flow.Launcher.Plugin.QuickCode;

public enum TokenMatchKind
{
    None = 0,
    Prefix = 1,
    Full = 2,
    Quick = 3,
}

public sealed record Shortcut(string Name, string Path);

public sealed record MatchedShortcut(Shortcut Item, int Score);

/// <summary>Pure query matcher: maps 速成/倉頡 tokens to a program display name.</summary>
public static class Matcher
{
    private const int ScoreFactor = 10_000;

    /// <summary>Returns match quality (sum of per-token weights) or null when no match.</summary>
    public static int? TryMatch(string name, CangjieTable table, string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return null;

        var han = table.GetHanChars(name);
        var latin = GetLatinWords(name);

        if (query.Any(char.IsWhiteSpace))
        {
            var tokens = SplitTokens(query);
            var h = MatchHan(han, table, tokens);
            var l = MatchLatin(latin, tokens);
            return MaxScore(h, l);
        }

        // Single token (no spaces): match it as quick/full/prefix against the first
        // Han character, OR as concatenated 速成 codes (try all 1/2-letter splits),
        // OR as a leading Latin word prefix.
        int? best = null;

        var single = MatchHan(han, table, new[] { query });
        if (single is int sv && (best is null || sv > best))
            best = sv;

        foreach (var segs in SplitQuickCodes(query))
        {
            var s = MatchHanQuick(han, table, segs);
            if (s is int v && (best is null || v > best))
                best = v;
        }

        var latinMatch = MatchLatin(latin, new[] { query });
        if (latinMatch is int lv && (best is null || lv > best))
            best = lv;

        return best;
    }

    /// <summary>Finds and ranks matching shortcuts; higher Score first, shorter names break ties.</summary>
    public static IReadOnlyList<MatchedShortcut> Find(
        IReadOnlyList<Shortcut> items, CangjieTable table, string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return Array.Empty<MatchedShortcut>();

        var results = new List<MatchedShortcut>();
        foreach (var item in items)
        {
            var quality = TryMatch(item.Name, table, query);
            if (quality is int q)
                results.Add(new MatchedShortcut(item, FinalScore(q, item.Name.Length)));
        }

        return results
            .OrderByDescending(r => r.Score)
            .ThenBy(r => r.Item.Name, StringComparer.Ordinal)
            .ToList();
    }

    private static int FinalScore(int quality, int nameLength) =>
        quality * ScoreFactor + Math.Max(0, ScoreFactor - nameLength);

    private static IReadOnlyList<string> SplitTokens(string query) =>
        query.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

    private static int? MatchHan(IReadOnlyList<string> han, CangjieTable table, IReadOnlyList<string> tokens)
    {
        if (tokens.Count > han.Count) return null;
        var quality = 0;
        for (var i = 0; i < tokens.Count; i++)
        {
            if (!table.TryGetCangjie(han[i], out var code)) return null;
            var kind = MatchTokenToChar(tokens[i], code);
            if (kind == TokenMatchKind.None) return null;
            quality += (int)kind;
        }
        return quality;
    }

    private static int? MatchHanQuick(IReadOnlyList<string> han, CangjieTable table, IReadOnlyList<string> quickCodes)
    {
        if (quickCodes.Count > han.Count) return null;
        var quality = 0;
        for (var i = 0; i < quickCodes.Count; i++)
        {
            if (!table.TryGetCangjie(han[i], out var code)) return null;
            if (!string.Equals(quickCodes[i], CangjieTable.ToQuick(code), StringComparison.OrdinalIgnoreCase))
                return null;
            quality += (int)TokenMatchKind.Quick;
        }
        return quality;
    }

    private static TokenMatchKind MatchTokenToChar(string token, string cangjie)
    {
        var quick = CangjieTable.ToQuick(cangjie);
        if (string.Equals(token, quick, StringComparison.OrdinalIgnoreCase)) return TokenMatchKind.Quick;
        if (string.Equals(token, cangjie, StringComparison.OrdinalIgnoreCase)) return TokenMatchKind.Full;
        if (cangjie.StartsWith(token, StringComparison.OrdinalIgnoreCase)) return TokenMatchKind.Prefix;
        return TokenMatchKind.None;
    }

    private static int? MatchLatin(IReadOnlyList<string> latinWords, IReadOnlyList<string> tokens)
    {
        if (latinWords.Count == 0 || tokens.Count == 0 || tokens.Count > latinWords.Count) return null;
        var quality = 0;
        for (var i = 0; i < tokens.Count; i++)
        {
            if (!latinWords[i].StartsWith(tokens[i], StringComparison.OrdinalIgnoreCase)) return null;
            quality += (int)TokenMatchKind.Prefix;
        }
        return quality;
    }

    private static IReadOnlyList<string> GetLatinWords(string name)
    {
        var words = new List<string>();
        var sb = new StringBuilder();
        foreach (var c in name)
        {
            if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
            {
                sb.Append(c);
            }
            else if (sb.Length > 0)
            {
                words.Add(sb.ToString());
                sb.Clear();
            }
        }
        if (sb.Length > 0) words.Add(sb.ToString());
        return words;
    }

    private static IEnumerable<IReadOnlyList<string>> SplitQuickCodes(string s)
    {
        if (s.Length == 0)
        {
            yield return Array.Empty<string>();
            yield break;
        }
        if (s.Length >= 1)
            foreach (var rest in SplitQuickCodes(s[1..]))
                yield return Prepend(s[..1], rest);
        if (s.Length >= 2)
            foreach (var rest in SplitQuickCodes(s[2..]))
                yield return Prepend(s[..2], rest);
    }

    private static IReadOnlyList<string> Prepend(string head, IReadOnlyList<string> tail)
    {
        var result = new List<string>(tail.Count + 1) { head };
        result.AddRange(tail);
        return result;
    }

    private static int? MaxScore(int? a, int? b)
    {
        if (a is null) return b;
        if (b is null) return a;
        return a > b ? a : b;
    }
}
