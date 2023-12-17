namespace Common.Extensions;

public static class Enumerable
{
    public static void Each<T>(this IEnumerable<T> self, Action<T> action)
    {
        foreach (var item in self)
        {
            action(item);
        }
    }
}
