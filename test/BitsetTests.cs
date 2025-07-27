public class BitsetTests
{
    [TestCase(0b110, 0b111, false)]
    [TestCase(0b110, 0b101, false)]
    [TestCase(0b110, 0b011, false)]
    [TestCase(0b110, 0b110, true)]
    [TestCase(0b110, 0b100, true)]
    [TestCase(0b110, 0b010, true)]
    [TestCase(0b1100, 0b1111, false)]
    [TestCase(0b1100, 0b1011, false)]
    [TestCase(0b1100, 0b0111, false)]
    [TestCase(0b1100, 0b1110, false)]
    [TestCase(0b1100, 0b1010, false)]
    [TestCase(0b1100, 0b0110, false)]
    // These don't work for this case -> this is not complete
    //[TestCase(0b1100, 0b1101, true)]
    //[TestCase(0b1100, 0b1001, true)]
    //[TestCase(0b1100, 0b0101, true)]
    [TestCase(0b1100, 0b1100, true)]
    [TestCase(0b1100, 0b1000, true)]
    [TestCase(0b1100, 0b0100, true)]
    public void TestItemMatch(long filter, long item, bool expectedSuccess)
    {
        var result = ((filter ^ item) & item) == 0;
        Assert.AreEqual(expectedSuccess, result);
    }

    [TestCase(0b110, 0b111, 0b111, false)]
    [TestCase(0b110, 0b101, 0b111, false)]
    [TestCase(0b110, 0b011, 0b111, false)]
    [TestCase(0b110, 0b110, 0b111, true)]
    [TestCase(0b110, 0b100, 0b111, true)]
    [TestCase(0b110, 0b010, 0b111, true)]
    [TestCase(0b1100, 0b1111, 0b1110, false)]
    [TestCase(0b1100, 0b1011, 0b1110, false)]
    [TestCase(0b1100, 0b0111, 0b1110, false)]
    [TestCase(0b1100, 0b1110, 0b1110, false)]
    [TestCase(0b1100, 0b1010, 0b1110, false)]
    [TestCase(0b1100, 0b0110, 0b1110, false)]
    [TestCase(0b1100, 0b1101, 0b1110, true)]
    [TestCase(0b1100, 0b1001, 0b1110, true)]
    [TestCase(0b1100, 0b0101, 0b1110, true)]
    [TestCase(0b1100, 0b1100, 0b1110, true)]
    [TestCase(0b1100, 0b1000, 0b1110, true)]
    [TestCase(0b1100, 0b0100, 0b1110, true)]
    [TestCase(0b110011, 0b010011, 0b1111111, true)]
    [TestCase(0b110011, 0b010111, 0b1111111, false)]
    [TestCase(0b110011, 0b011111, 0b1111111, false)]
    public void TestItemMatchWithUsed(long filter, long item, long used, bool expectedSuccess)
    {
        TagSet tagSet = new() { Included = filter, Excluded = filter ^ used };
        Assert.AreEqual(expectedSuccess, Bitset.ExclusionaryUnion(tagSet, item));
    }

    [TestCase(0b000, 0b000, 0b111, false)]
    [TestCase(0b100, 0b000, 0b111, true)]
    [TestCase(0b110, 0b000, 0b111, true)]
    [TestCase(0b110, 0b000, 0b100, true)]
    [TestCase(0b110, 0b000, 0b010, true)]
    [TestCase(0b110, 0b000, 0b001, false)]
    [TestCase(0b110, 0b001, 0b100, true)]
    [TestCase(0b110, 0b001, 0b010, true)]
    [TestCase(0b110, 0b001, 0b110, true)]
    [TestCase(0b110, 0b001, 0b111, true)]
    [TestCase(0b000, 0b001, 0b111, false)]
    [TestCase(0b000, 0b001, 0b110, true)]
    [TestCase(0b1100, 0b0011, 0b1000, true)]
    [TestCase(0b1100, 0b0011, 0b0100, true)]
    [TestCase(0b1100, 0b0011, 0b0010, true)]
    [TestCase(0b1100, 0b0011, 0b0001, true)]
    [TestCase(0b1100, 0b0011, 0b0011, false)]
    [TestCase(0b1100, 0b0011, 0b1011, true)]
    [TestCase(0b1100, 0b0010, 0b1001, true)]
    [TestCase(0b1100, 0b0010, 0b0011, false)]
    public void TestOrMatch(long include, long exclude, long item, bool expectSuccess)
    {
        TagSet ts = new() { Included = include, Excluded = exclude };
        Assert.AreEqual(expectSuccess, Bitset.OrMatch(ts, item));
    }
}