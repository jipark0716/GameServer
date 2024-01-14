namespace Util.Extensions;

public static class EnumerableExtension
{
    public static void Each<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
        {
            action(item);
        }
    }

    public static IEnumerable<T> DequeueLoop<T>(this Queue<T> source, int delay = 100)
        => DequeueLoop(source, TimeSpan.FromMilliseconds(delay));
    
    public static IEnumerable<T> DequeueLoop<T>(this Queue<T> source, TimeSpan delay)
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
}