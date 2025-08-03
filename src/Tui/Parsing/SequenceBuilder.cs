using static CharType;

public enum CharType
{
    INVALID = 0b0,
    NUMBER = 0b1,
    LETTER = 0b10,
    DASH = 0b11,
    DOT = 0b100,
}

/// <summary>
/// NOTE: Enums values are represented as integer values
/// Which means the maximum pattern length is 32
/// Be aware, that the size is max(bitshift) + 3 [3 per pattern element]
/// </summary
public enum SequencePatterns
{
    ERR = 0b0,
    N = NUMBER,
    N_N = NUMBER << 6 | DASH << 3 | NUMBER,
    L = LETTER,
    NL = NUMBER << 3 | LETTER,
    _L = DASH << 3 | LETTER,
    _N = DASH << 3 | NUMBER,
    N_N_N = NUMBER << 12 | DASH << 9 | NUMBER << 6 | DASH << 3 | NUMBER,
    L_N = LETTER << 6 | DASH << 3 | NUMBER,
    LN = LETTER << 3 | NUMBER,
    N_LN = NUMBER << 9 | DASH << 6 | LETTER << 3 | NUMBER,
    _NL = DASH << 6 | NUMBER << 3 | LETTER,

    // Weekday 
    DL = DOT << 3 | LETTER,
    NDL = NUMBER << 6 | DOT << 3 | LETTER,
    _DL = DASH << 6 | DOT << 3 | LETTER,
    _NDL = DASH << 9 | NUMBER << 6 | DOT << 3 | LETTER,

    // Weekday + time
    DL_N = DL << 6 | _N,
    NDL_N = NDL << 9 | _N,
    _DL_N = _DL << 9 | _N,
    _NDL_N = _NDL << 12 | _N
}

public record CharSequence(CharType Type, int StartIndex, int initialCount = 1)
{
    public int CharCount { set; get; } = initialCount;

    // Start inclusive, End exclusive
    public int EndIndex => StartIndex + CharCount;

    /// <summary>
    ///  Lower invariant of the str sequence captured by this sequence
    /// </summary
    public string Str { set; get; } = string.Empty;
};

public class SequenceBuilder
{
    private List<CharSequence> _sequences = new List<CharSequence>(6);
    private int _seqIndex = -1;

    public string Expression { get; }
    public int SequenceCount => _sequences.Count();
    public int LastIdx => _sequences.Count() - 1;
    public int TotalCharCount { private set; get; } = 0;
    public int Pattern { get; }
    public bool IsSubsequence { get; private set; } = false;

    public SequenceBuilder(string expression)
    {
        Expression = expression.ToLowerInvariant();
        Build();
        Pattern = GetPattern();
        IsSubsequence = false;
    }

    private SequenceBuilder(List<CharSequence> subSequence)
    {
        IsSubsequence = true;
        _sequences = subSequence;
        Pattern = GetPattern();

        Expression = string.Join("", subSequence.Select(s => s.Str));
        TotalCharCount = subSequence.Sum(s => s.CharCount);
    }

    public (SequenceBuilder front, SequenceBuilder back) SplitLast(int num)
    {
        if (_sequences.Count() < num) throw new IndexOutOfRangeException($"Wanted to take last {num} elemest but seuqence length only was {_sequences.Count()}");

        var front = _sequences[..^num];
        var back = _sequences[^num..];

        return (new(front), new(back));
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
            prev.Str = Expression[prev.StartIndex..prev.EndIndex].ToLowerInvariant();
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
                '-' => CharType.DASH,
                '.' => CharType.DOT,
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

    // NOTE: Bitshift has to match the actual shift from the enum
    // else the returned pattern will be wrong
    private int GetPattern()
    {
        // Unhandeled symbols
        if (_sequences.Any(s => s.Type == CharType.INVALID)) return 0b0;

        int pattern = 0;

        for (int i = 0; i < _sequences.Count(); i++)
        {
            int typeId = (int)_sequences[i].Type;

            pattern = (pattern << 3) | typeId;
        }

        return pattern;
    }

    public CharSequence Get(int index)
    {
        if (index > SequenceCount - 1) throw new ArgumentOutOfRangeException(nameof(index), $"Index: {index} - Sequence: {SequenceCount}");
        return _sequences[index];
    }
}