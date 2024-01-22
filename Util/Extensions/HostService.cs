using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Util.Config;

namespace Util.Extensions;

public static class HostService
{
    public static T AddConfig<T>(this IServiceCollection source, string[] args)
        where T : class
    {
        ObjectDisposedException.ThrowIf(
            args.Length < 3,
            new InvalidOperationException("fail config load"));
        
        var config = new ConfigLoader().Get<T>(args[0], args[1], args[2]);
        source.AddSingleton<T>(o => config);
        return config;
    }

    public static void AddDbContextPool<T>(this IServiceCollection source, DatabaseConfig config)
        where T : DbContext
    {
        source.AddDbContextPool<T>(
            o => o.UseMySql(
                    config.ConnectionString,
                    new MySqlServerVersion(config.VersionString)),
                config.ConnectionPoolSize);
    }
}