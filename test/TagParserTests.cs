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
        Assert.IsEmpty(filter.Tags);
    }

    [Test]
    public void TestTags_UnknownTagName_IsIgnored()
    {
        var filter = Parsing.ParseOptions(TAGS, new string[] { "#hello" }.AsMemory());
        AssertFirstTag(filter.Tags, 0, 0);
    }

    [Test]
    public void TestTags_ContainsSingle()
    {
        var filter = Parsing.ParseOptions(TAGS, new string[] { "#bbb" }.AsMemory());
        AssertFirstTag(filter.Tags, 0x1 << 1, 0);
    }

    [Test]
    public void TestTags_NotContainsSingle()
    {
        var filter = Parsing.ParseOptions(TAGS, new string[] { "~#bbb" }.AsMemory());
        AssertFirstTag(filter.Tags, 0, 0x1 << 1);
    }

    [Test]
    public void TestTags_ContainsTwo()
    {
        var filter = Parsing.ParseOptions(TAGS, new string[] { "#bbb&#ccc" }.AsMemory());
        AssertFirstTag(filter.Tags, 0x1 << 1 | 0x1 << 2, 0);
    }

    [Test]
    public void TestTags_AndNot()
    {
        var filter = Parsing.ParseOptions(TAGS, new string[] { "#bbb&~#ccc" }.AsMemory());
        AssertFirstTag(filter.Tags, 0x1 << 1, 0x1 << 2);
    }

    private void AssertFirstTag(IEnumerable<TagSet> tags, long includes, long excludes)
    {
        var tag = tags.First();
        Assert.AreEqual(includes, tag.Included);
        Assert.AreEqual(excludes, tag.Excluded);
        Assert.AreEqual(includes | excludes, tag.Tags);
    }
}