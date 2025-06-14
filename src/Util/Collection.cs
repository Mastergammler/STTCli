public static class Collection
{
    public static IEnumerable<T> Of<T>(params T[] items) => Enumerable.Empty<T>().Concat(items);
}