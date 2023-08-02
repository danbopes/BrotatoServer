namespace BrotatoServer.Utilities;

public static class EnumerableExtensions
{
    public static IEnumerable<KeyValuePair<int, T>> WithIdx<T>(this IEnumerable<T> source)
    {
        var idx = 0;
        foreach (var item in source)
        {
            yield return new KeyValuePair<int, T>(idx++, item);
        }
    }
}
