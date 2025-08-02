
public enum CharType
{
    INVALID = 0b0,
    NUMBER = 0b1,
    LETTER = 0b10,
    SYMBOL = 0b11
}

/// <summary>
///  NOTE: this doesn't work for compiling the pattern because of the limitations of c# pattern matichng
///  Patterns need to run against compile-time constants, and there is no way for the compiler to know
///  (or to tell the compiler) that these values basically are / neither can you pre-compile them in a way.
/// </summary>
public static class PatternUtil
{
    public static long Create(params CharType[] types)
    {
        long pattern = 0L;

        foreach (CharType type in types)
        {
            pattern = (pattern << 2) | (long)type;
        }

        return pattern;
    }
}

public record CharSequence(CharType Type, int StartIndex, int initialCount = 1)
{
    public int CharCount { set; get; } = initialCount;

    // Start inclusive, End exclusive
    public int EndIndex => StartIndex + CharCount;
    public string Str { set; get; } = string.Empty;
};

public class SequenceBuilder
{
    private IList<CharSequence> _sequences = new List<CharSequence>(5);
    private int _seqIndex = -1;

    public string Expression { get; }
    public int SequenceCount => _sequences.Count();
    public int LastIdx => _sequences.Count() - 1;
    public int TotalCharCount { private set; get; } = 0;
    public long Pattern { get; }

    public SequenceBuilder(string expression)
    {
        Expression = expression.ToLowerInvariant();
        Build();
        Pattern = GetPattern();
    }

    public IEnumerable<CharSequence> Find(CharType type) => _sequences.Where(s => s.Type == type);

    private void Add(CharType type)
    {
        if (_seqIndex >= 0)
        {
            var curEl = _sequences[_seqIndex];
            if (curEl.Type == type)
            {
                curEl.CharCount++;
                TotalCharCount++;
                return;
            }
        }

        TotalCharCount++;
        _seqIndex++;

        int nextStartIdx = 0;
        var prev = _sequences.LastOrDefault();
        if (prev is not null)
        {
            prev.Str = Expression[prev.StartIndex..prev.EndIndex];
            nextStartIdx = prev.EndIndex;
        }

        _sequences.Add(new(type, nextStartIdx));
    }

    private void Build()
    {
        for (int ci = 0; ci < Expression.Length; ci++)
        {
            char c = Expression[ci];
            var charType = c switch
            {
                >= '0' and <= '9' => CharType.NUMBER,
                >= 'a' and <= 'z' => CharType.LETTER,
                '-' => CharType.SYMBOL,
                _ => CharType.INVALID
            };

            Add(charType);
        }

        //close last sequence if it exists
        var seq = _sequences.LastOrDefault();
        if (seq is not null)
        {
            seq.Str = Expression[seq.StartIndex..];
        }
    }

    private long GetPattern()
    {
        // Unhandeled symbols
        if (_sequences.Any(s => s.Type == CharType.INVALID)) return 0b0;

        int pattern = 0;

        for (int i = 0; i < _sequences.Count(); i++)
        {
            int typeId = (int)_sequences[i].Type;

            pattern = (pattern << 2) | typeId;
        }

        return pattern;
    }

    public CharSequence Get(int index)
    {
        if (index > SequenceCount - 1) throw new ArgumentOutOfRangeException(nameof(index), $"Index: {index} - Sequence: {SequenceCount}");
        return _sequences[index];
    }
}