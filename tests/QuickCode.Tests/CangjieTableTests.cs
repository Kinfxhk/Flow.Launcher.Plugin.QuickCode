using Flow.Launcher.Plugin.QuickCode;
using Xunit;

namespace QuickCode.Tests;

public class CangjieTableTests
{
    private static CangjieTable Table() => CangjieTable.FromLines(new[]
    {
        "記\tYRSU",
        "事\tJLLN",
        "本\tDM",
        "日\tA",
        "程\tHDRHG",
        "式\tIPM",
        "碼\tMRSQF",
    });

    [Theory]
    [InlineData("YRSU", "YU")]
    [InlineData("JLLN", "JN")]
    [InlineData("DM", "DM")]
    [InlineData("A", "A")]
    [InlineData("HDRHG", "HG")]
    [InlineData("IPM", "IM")]
    [InlineData("MRSQF", "MF")]
    public void ToQuick_ConvertsFirstAndLast(string cangjie, string expected)
        => Assert.Equal(expected, CangjieTable.ToQuick(cangjie));

    [Fact]
    public void Load_ReadsEmbeddedTable()
    {
        var t = CangjieTable.Load();
        Assert.True(t.TryGetCangjie("記", out var code));
        Assert.Equal("YRSU", code);
        Assert.True(t.TryGetCangjie("日", out var c2));
        Assert.Equal("A", c2);
    }

    [Fact]
    public void GetHanCodes_SkipsNonHanAndComputesQuick()
    {
        var t = Table();
        var codes = t.GetHanCodes("Visual Studio 程式碼");
        Assert.Equal(new[] { "程", "式", "碼" }, codes.Select(c => c.Character).ToArray());
        Assert.Equal(new[] { "HG", "IM", "MF" }, codes.Select(c => c.Quick).ToArray());
        Assert.Equal(new[] { "HDRHG", "IPM", "MRSQF" }, codes.Select(c => c.Cangjie).ToArray());
    }
}
