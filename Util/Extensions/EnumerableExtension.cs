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
}