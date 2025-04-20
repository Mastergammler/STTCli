public static class Extensions
{
    public static IEnumerable<T> Where<T>(this Memory<T> input, Func<T, bool> predicate)
    {
        for (int i = 0; i < input.Length; i++)
        {
            if (predicate(input.Span[i]))
                yield return input.Span[i];
        }
    }
}