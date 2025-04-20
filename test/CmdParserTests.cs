public class CmdParserTests
{
    [Test]
    public void TestingDefaultCase()
    {
        var parsed = Parsing.ParseCmdInput("my default commands");
        Assert.AreEqual(parsed, new string[] { "my", "default", "commands" });
    }

    [Test]
    public void ParseInput_SpaceEnding()
    {
        var parsed = Parsing.ParseCmdInput("my default ");
        Assert.AreEqual(parsed, new string[] { "my", "default" });
    }

    [Test]
    public void ParseInput_IgnoreMultiSpaces()
    {
        var parsed = Parsing.ParseCmdInput("my    default");
        Assert.AreEqual(parsed, new string[] { "my", "default" });
    }

    [Test]
    public void ParseInput_WithQuoteStart()
    {
        var parsed = Parsing.ParseCmdInput("\"multiple params\" here");
        Assert.AreEqual(parsed, new string[] { "multiple params", "here" });
    }

    [Test]
    public void ParseInput_WithQuoteEnd()
    {
        var parsed = Parsing.ParseCmdInput("starting normally \"ending crazy\"");
        Assert.AreEqual(parsed, new string[] { "starting", "normally", "ending crazy" });
    }

    [Test]
    public void ParseInput_SpaceMissingForQuote()
    {
        var parsed = Parsing.ParseCmdInput("hello\"there whats happening\"");
        Assert.AreEqual(new string[] { "hello", "there whats happening" }, parsed);
    }

    [Test]
    public void ParseInput_OpenQuote()
    {
        var parsed = Parsing.ParseCmdInput("hello\"there is no end");
        Assert.AreEqual(new string[] { "hello", "there is no end" }, parsed);
    }

    [Test]
    public void ParseInput_OpenClosing()
    {
        var parsed = Parsing.ParseCmdInput("hello \"");
        Assert.AreEqual(new string[] { "hello" }, parsed);
    }

    [Test]
    public void ParseInput_OpenClosingDirect()
    {
        var parsed = Parsing.ParseCmdInput("hello\"");
        Assert.AreEqual(new string[] { "hello" }, parsed);
    }
}