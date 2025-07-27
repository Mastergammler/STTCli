public static class Bitset
{
    public static bool AndMatch(this TagSet tagSet, long itemTags)
    {
        // means invalid entry, so no matches
        if (tagSet.Tags == 0) return false;

        return (tagSet.Included ^ (itemTags & tagSet.Tags)) == 0;
    }

    public static bool OrMatch(this TagSet tagSet, long itemTags)
    {
        // means invalid entry, so no matches
        if (tagSet.Tags == 0) return false;

        //TODO: can this be simplified?
        return ((tagSet.Included & itemTags) > 0 || !((tagSet.Excluded & itemTags) == tagSet.Excluded));
    }

    /// <summary>
    ///  Checks if one of the Include tags are there, but all of the exclude tags are not set
    ///  This allows for common checks like (A|B) & ~C to be executed
    ///  Since this is a union operation with exclusions, unions without exclusions can also 
    ///  be handled this way
    ///
    ///  This basically works like an OR for inclusion tags + and AND for exclusion tags
    ///  
    ///  Limitation: There is no precidence handling, and no preceise expressions are currently possible
    ///  e.G. (A & ~B) | (C & ~D) / A | (B & ~C)
    ///  All expressions will only match the positive ones, and reduce all the negative ones
    /// </summary
    public static bool ExclusionaryUnion(this TagSet filter, long itemTags)
    {
        if (filter.Tags == 0) return false;

        return ((filter.Included ^ (itemTags & filter.Tags)) & itemTags & filter.Tags) == 0;
    }
}