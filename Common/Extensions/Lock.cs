namespace Common.Extensions;

public static class Lock
{
    public static async void Enter(this SpinLock @lock, Func<Task> func)
    {
        var gotLock = false;
        try
        {
            @lock.Enter(ref gotLock);
            await func();
        }
        finally
        {
            // Only give up the lock if you actually acquired it
            if (gotLock)
            {
                @lock.Exit();
            }
        }
    }
}
