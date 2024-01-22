using Microsoft.Extensions.DependencyInjection;

namespace Util.Extensions;

public static class HostService
{
    public static void AddConfig<T>(this IServiceCollection source, string[] args)
        where T : class
    {
        var config = new ConfigLoader().Get<T>(args[0], args[1], args[2]);
        source.AddSingleton<T>(o => config);
    }
}