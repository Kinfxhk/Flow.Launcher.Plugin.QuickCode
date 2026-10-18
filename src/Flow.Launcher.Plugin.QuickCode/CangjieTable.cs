using System.Reflection;

namespace Flow.Launcher.Plugin.QuickCode;

/// <summary>倉頡 code of one Han character (速成 = first + last letter).</summary>
public sealed record HanCode(string Character, string Quick, string Cangjie);

/// <summary>Pure in-memory 倉頡 lookup table (character -> full code).</summary>
public sealed class CangjieTable
{
    private readonly Dictionary<string, string> _cangjie;

    public CangjieTable(Dictionary<string, string> cangjie) => _cangjie = cangjie;

    /// <summary>Loads the embedded Unihan-derived table from the assembly resource.</summary>
    public static CangjieTable Load()
    {
        var asm = typeof(CangjieTable).Assembly;
        using var stream = asm.GetManifestResourceStream("QuickCode.cangjie.tsv")
            ?? throw new InvalidOperationException("Embedded QuickCode.cangjie.tsv not found.");
        using var reader = new StreamReader(stream);
        return FromLines(ReadAllLines(reader));
    }

    /// <summary>Builds a table from TSV lines of the form "char&lt;TAB&gt;CODE".</summary>
    public static CangjieTable FromLines(IEnumerable<string> lines)
    {
        var map = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var tab = line.IndexOf('\t');
            if (tab <= 0 || tab >= line.Length - 1) continue;
            var ch = line[..tab];
            var code = line[(tab + 1)..].Trim().ToUpperInvariant();
            if (ch.Length == 0 || code.Length == 0) continue;
            map[ch] = code;
        }
        return new CangjieTable(map);
    }

    public bool TryGetCangjie(string character, out string cangjie) =>
        _cangjie.TryGetValue(character, out cangjie!);

    /// <summary>速成 code = first + last letter (a 1-letter code stays 1 letter).</summary>
    public static string ToQuick(string cangjie)
    {
        if (string.IsNullOrEmpty(cangjie)) return string.Empty;
        return cangjie.Length == 1 ? cangjie : string.Concat(cangjie[0], cangjie[^1]);
    }

    /// <summary>Han characters (code points present in the table) of a name, in order.</summary>
    public IReadOnlyList<string> GetHanChars(string name)
    {
        var list = new List<string>();
        foreach (var cp in EnumerateCodePoints(name))
            if (_cangjie.ContainsKey(cp))
                list.Add(cp);
        return list;
    }

    /// <summary>Code info (速成 + 倉頡) for every Han character in the name, in order.</summary>
    public IReadOnlyList<HanCode> GetHanCodes(string name)
    {
        var list = new List<HanCode>();
        foreach (var cp in EnumerateCodePoints(name))
            if (_cangjie.TryGetValue(cp, out var code))
                list.Add(new HanCode(cp, ToQuick(code), code));
        return list;
    }

    private static IEnumerable<string> ReadAllLines(StreamReader reader)
    {
        while (reader.ReadLine() is { } line)
            yield return line;
    }

    private static IEnumerable<string> EnumerateCodePoints(string s)
    {
        for (var i = 0; i < s.Length; i++)
        {
            var c = s[i];
            if (char.IsHighSurrogate(c) && i + 1 < s.Length && char.IsLowSurrogate(s[i + 1]))
            {
                yield return s.Substring(i, 2);
                i++;
            }
            else
            {
                yield return c.ToString();
            }
        }
    }
}
