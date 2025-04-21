public class TagParserTests
{
    private static Tag[] TAGS = {
        new() { Name = "#aaa", Bit = 0x1},
        new() { Name = "#bbb", Bit = 0x1 << 1},
        new() { Name = "#ccc", Bit = 0x1 << 2},
    };

    [Test]
    public void TestTags_EmptyValue()
    {
        var filter = Parsing.ParseOptions(TAGS, new string[] { "" }.AsMemory());
        Assert.IsEmpty(filter.AndTags);
    }

    [Test]
    public void TestTags_UnknownTagName_IsIgnored()
    {
        //FIXME: this case seems quite invalid ...
        var filter = Parsing.ParseOptions(TAGS, new string[] { "#hello" }.AsMemory());
        Assert.Contains(0, filter.AndTags);
    }

    [Test]
    public void TestTags_ContainsSingle()
    {
        var filter = Parsing.ParseOptions(TAGS, new string[] { "#bbb" }.AsMemory());
        Assert.Contains(0x1 << 1, filter.AndTags);
    }

    [Test]
    public void TestTags_NotContainsSingle()
    {
        var filter = Parsing.ParseOptions(TAGS, new string[] { "~#bbb" }.AsMemory());
        Assert.Contains(~(0x1 << 1), filter.AndTags);
    }

    [Test]
    public void TestTags_ContainsTwo()
    {
        var filter = Parsing.ParseOptions(TAGS, new string[] { "#bbb&#ccc" }.AsMemory());
        Assert.Contains(0x1 << 1 | 0x1 << 2, filter.AndTags);
    }

    [Test]
    //FIXME: this doesn't make sense currently
    public void TestTags_AndNot()
    {
        var filter = Parsing.ParseOptions(TAGS, new string[] { "#bbb&~#ccc" }.AsMemory());
        Assert.Contains(0x1 << 1, filter.AndTags);
    }
}