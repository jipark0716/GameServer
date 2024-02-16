using System.Reflection;
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

    public static void AddSingleton<T>(this IServiceCollection source, params Type[] classes)
        where T : class
        => source.AddSingleton<T>(o => o.ProxyBuild<T>([], classes));

    public static T ProxyBuild<T>(this IServiceProvider source, IEnumerable<object> arguments, IEnumerable<Type> classes)
        where T : class
    {
        return classes.Select(classType => classType.GetConstructors()[0] ?? throw new InvalidOperationException())
            .Aggregate<ConstructorInfo, T?>(null, (current, constructor) =>
            {
                return constructor.Invoke(constructor.GetParameters()
                    .Select(parameter =>
                    {
                        if (parameter.ParameterType == typeof(T)) return current;
                        foreach (var argument in arguments)
                        {
                            if (parameter.ParameterType.IsInstanceOfType(argument)) return argument;
                        }

                        return source.GetService(parameter.ParameterType);
                    })
                    .ToArray()) as T;
            }) ?? throw new InvalidOperationException();
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