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

    public static string Truncate(this string str, int maxLength, bool withIndicator = true)
    {
        if (str.Length > maxLength)
        {
            if (withIndicator)
            {
                return str[0..(maxLength - 2)] + "..";
            }
            return str[0..maxLength];
        }
        return str;
    }

    public static string With(this string str, params object[] args) => string.Format(str, args);


    public static Result<IEnumerable<T>> FindArg<T>(this Memory<T> input, Func<T, bool> predicate)
    {
        return new Success<IEnumerable<T>>(input.Where(predicate));
    }
}