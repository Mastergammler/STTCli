public static class Parsing
{
    public static string[] ParseCmdInput(string input)
    {
        List<string> parts = [];

        bool withinQuotes = false;
        int lastIndex = 0;
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == ' ' && !withinQuotes)
            {
                parts.Add(input[lastIndex..i]);
                // we want to be exclusive
                lastIndex = i + 1;
            }
            else if (input[i] == '"')
            {
                if (withinQuotes || (i > 0 && input[i - 1] != ' '))
                {
                    parts.Add(input[lastIndex..i]);
                }

                lastIndex = i + 1;
                withinQuotes = !withinQuotes;
            }
        }

        if (lastIndex < input.Length) parts.Add(input[lastIndex..]);

        return parts.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
    }

    //TODO: refactor to some more generalized parser strategies
    private static Func<string, bool> IsTag = s => s.StartsWith("#") || s.StartsWith("~#");

    public static FilterOptions ParseOptions(Tag[] tags, Memory<string> args)
    {
        var opt = new FilterOptions();
        var dict = tags.ToDictionary(t => t.Name);
        foreach (string arg in args.Span)
        {
            if (IsTag(arg))
            {
                var tagSet = ParseTags(arg, dict);
                opt.Tags.Add(tagSet);
            }

            // TODO: handle other cases
        }

        return opt;
    }

    private static TagSet ParseTags(string tagString, IDictionary<string, Tag> tags)
    {
        TagSet ts = new();

        string[] ands = tagString.Split('&');

        foreach (string op in ands)
        {
            string tagName = op;
            bool isComplement = false;
            if (op.StartsWith('~'))
            {
                tagName = op[1..];
                isComplement = true;
            }

            if (tags.ContainsKey(tagName))
            {
                long tagValue = tags[tagName].Bit;

                if (isComplement)
                {
                    if (ts.Excluded == 0) ts.Excluded = tagValue;
                    ts.Excluded |= tagValue; // punch out new zeros
                }
                else
                {
                    if (ts.Included == 0) ts.Included = tagValue;
                    else ts.Included |= tagValue; // add new 1s together
                }
            }

            //NOTE: for the purpose of parsing we just ignore invalid values
            // - else we create a crash for every invalid user input?
            // -> Or would this be the correct handling? Because i would want to know that something is wrong?
        }

        return ts;
    }
}

public class FilterOptions
{
    public List<TagSet> Tags { get; } = [];
}

public class TagSet
{
    public long Included { set; get; } = 0;
    public long Excluded { set; get; } = 0;
    public long Tags => Included | Excluded;
}