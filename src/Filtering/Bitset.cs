
public static class Bitset
{
    public static bool Matches(this TagSet tagSet, long itemTags)
    {
        // means invalid entry, so no matches
        if (tagSet.Included == 0 && tagSet.Excluded == 0) return false;

        return (itemTags & tagSet.Included) == tagSet.Included;
    }
}