using Flow.Launcher.Plugin.QuickCode;
using Xunit;

namespace QuickCode.Tests;

public class MatcherTests
{
    private static readonly CangjieTable Table = CangjieTable.FromLines(new[]
    {
        "記\tYRSU", "事\tJLLN", "本\tDM", "日\tA",
        "程\tHDRHG", "式\tIPM", "碼\tMRSQF",
    });

    [Fact]
    public void SpacedQuickCodes_MatchNotepad()
        => Assert.Equal(9, Matcher.TryMatch("記事本", Table, "yu jn dm"));

    [Fact]
    public void SingleTokenConcatenatedQuick_MatchNotepad()
        => Assert.Equal(9, Matcher.TryMatch("記事本", Table, "yujndm"));

    [Fact]
    public void FullCangjie_MatchNotepad()
        // 記=YRSU(Full=2) 事=JLLN(Full=2) 本=DM(quick==full==DM -> Quick=3)
        => Assert.Equal(7, Matcher.TryMatch("記事本", Table, "yrsu jlln dm"));

    [Fact]
    public void ScoreOrdering_QuickBeatsFullBeatsPrefix()
    {
        Assert.Equal(3, Matcher.TryMatch("記", Table, "yu"));     // exact 速成
        Assert.Equal(2, Matcher.TryMatch("記", Table, "yrsu"));   // full 倉頡
        Assert.Equal(1, Matcher.TryMatch("記", Table, "yr"));     // prefix of 倉頡
    }

    [Fact]
    public void MixedName_SkipsLatinWhenAligningHan()
        => Assert.Equal(9, Matcher.TryMatch("Visual Studio 程式碼", Table, "hg im mf"));

    [Fact]
    public void LatinWordPrefix_Matches()
        => Assert.NotNull(Matcher.TryMatch("Visual Studio Code", Table, "vis stu"));

    [Fact]
    public void NoMatch_Cases()
    {
        Assert.Null(Matcher.TryMatch("記事本", Table, "zz"));
        Assert.Null(Matcher.TryMatch("記事本", Table, "yu jn dmx"));
        Assert.Null(Matcher.TryMatch("記事本", Table, ""));
        Assert.Null(Matcher.TryMatch("記事本", Table, "   "));
        Assert.Null(Matcher.TryMatch("記事本", Table, "dm")); // 本 alone is not the leading Han char
    }

    [Fact]
    public void Find_ShorterNameRanksFirstOnTie()
    {
        var items = new[]
        {
            new Shortcut("記事本", @"C:\notepad.exe"),
            new Shortcut("記事", @"C:\a.exe"),
        };
        var r = Matcher.Find(items, Table, "yu jn");
        Assert.Equal(2, r.Count);
        Assert.Equal("記事", r[0].Item.Name);
        Assert.Equal("記事本", r[1].Item.Name);
    }
}
