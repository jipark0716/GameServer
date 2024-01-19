namespace Util.Extensions;

public static class EnumerableExtension
{
    private static Random? _random;
    
    public static void Each<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
        {
            action(item);
        }
    }
    
    public static void Each<T>(this IEnumerable<T> source, Action<T, int> action)
        => source
            .Select((row, i) => (row, i))
            .Each(o => action(o.row, o.i));

    public static IEnumerable<T> DequeueLoop<T>(this Queue<T> source, int delay = 100)
        => DequeueLoop(source, TimeSpan.FromMilliseconds(delay));

    private static IEnumerable<T> DequeueLoop<T>(this Queue<T> source, TimeSpan delay)
    {
        while (true)
        {
            // worker 스레드
            if (source.TryDequeue(out var message))
            {
                yield return message;
            }
            else
            {
                Thread.Sleep(delay);
            }
        } 
    }

    public static T[] Merge<T>(this T[] source, params T[][]targets)
    {
        var result = new T[source.Length + targets.Sum(o => o.Length)];
        Buffer.BlockCopy(source, 0, result, 0, source.Length);

        var startIndex = source.Length;
        foreach (var target in targets)
        {
            Buffer.BlockCopy(target, 0, result, startIndex, target.Length);
            startIndex += target.Length;
        }
        
        return result;
    }

    public static IEnumerable<(T, int)> WithIndex<T>(this IEnumerable<T> source)
        => source.Select((o, i) => (o, i));
    
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random? random = null)
    {
        random ??= _random ??= new();
        
        var result = source.ToArray();
        random.Shuffle(result);
        return result;
    }

    public static void Clear<T>(this T?[] source)
        where T : class
    {
        for (var i = 0; i < source.Length; i ++)
        {
            source[i] = null;
        }
    }
}