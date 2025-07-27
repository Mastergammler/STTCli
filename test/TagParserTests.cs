public class TagParserTests
{
    public static Tag[] TAGS = {
        new() { Name = "#a", Bit = 0x1},
        new() { Name = "#b", Bit = 0x1 << 1},
        new() { Name = "#c", Bit = 0x1 << 2},
        new() { Name = "#d", Bit = 0x1 << 3}
    };

    [Test]
    public void TestTags_EmptyValue()
    {
        var filter = Parsing.ParseFiltering(TAGS, new string[] { "" }.AsMemory());
        Assert.IsEmpty(filter.Tags);
    }

    [Test]
    public void TestTags_UnknownTagName_IsIgnored()
    {
        var filter = Parsing.ParseFiltering(TAGS, new string[] { "#hello" }.AsMemory());
        AssertFirstTag(filter.Tags, 0, 0);
    }

    [Test]
    public void TestTags_ContainsSingle()
    {
        var filter = Parsing.ParseFiltering(TAGS, new string[] { "#b" }.AsMemory());
        AssertFirstTag(filter.Tags, 0x1 << 1, 0);
    }

    [Test]
    public void TestTags_NotContainsSingle()
    {
        var filter = Parsing.ParseFiltering(TAGS, new string[] { "~#b" }.AsMemory());
        AssertFirstTag(filter.Tags, 0, 0x1 << 1);
    }

    [Test]
    public void TestTags_ContainsTwo()
    {
        var filter = Parsing.ParseFiltering(TAGS, new string[] { "#b&#c" }.AsMemory());
        AssertFirstTag(filter.Tags, 0x1 << 1 | 0x1 << 2, 0);
    }

    [Test]
    public void TestTags_AndNot()
    {
        var filter = Parsing.ParseFiltering(TAGS, new string[] { "#b&~#c" }.AsMemory());
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