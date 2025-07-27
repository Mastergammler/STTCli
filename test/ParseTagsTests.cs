public class ParseTagsTests
{
    private static readonly IDictionary<string, Tag> TAGS = TagParserTests.TAGS.ToDictionary(t => t.Name);

    //NOTE: tags are inverted a = 0001, b = 0010
    [TestCase("#xyz", 0b0, 0b0, 0b0)]
    [TestCase("#a", 0b1, 0b0, 0b1)]
    [TestCase("~#a", 0b0, 0b1, 0b1)]
    [TestCase("#a&#b", 0b11, 0b0, 0b11)]
    [TestCase("#a&~#b", 0b01, 0b10, 0b11)]
    [TestCase("~#a&#b", 0b10, 0b01, 0b11)]
    [TestCase("#a&#b&#c&#d", 0b1111, 0b0, 0b1111)]
    [TestCase("#a&#b|#c", 0b0111, 0, 0b0111, true)]
    [TestCase("#a|#b", 0b0011, 0b0, 0b0011, false)]
    [TestCase("#a|#b|#c", 0b0111, 0b0, 0b0111, false)]
    [TestCase("#a|~#b", 0b0001, 0b0010, 0b0011, false)]
    [TestCase("~#a|~#b", 0b0000, 0b0011, 0b0011, false)]
    public void SingleTag(string expr, long include, long exclude, long tags, bool isAnd = true)
    {
        var ts = Parsing.ParseTags(expr, TAGS);

        Assert.AreEqual(include, ts.Included);
        Assert.AreEqual(exclude, ts.Excluded);
        Assert.AreEqual(tags, ts.Tags);
        Assert.AreEqual(isAnd, ts.AndExpr);
    }
}


