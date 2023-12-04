using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Boostrap.DI
{
    public static class DIExtension
    {
        public static void DI(this IServiceCollection service)
        {
            foreach (var classType in AssamblyExtendsion.GetAllClasses<LifeCycleAttribute>())
            {
                var attribute = classType.GetCustomAttribute<LifeCycleAttribute>();
                if (attribute is null)
                {
                    continue;
                }

                switch (attribute.LifeCycle)
                {
                    case LifeCycle.Singleton:
                        service.AddSingleton(classType, attribute.Abstract ?? classType);
                        break;
                    case LifeCycle.Scoped:
                        service.AddScoped(classType, attribute.Abstract ?? classType);
                        break;
                    case LifeCycle.Transient:
                        service.AddTransient(classType, attribute.Abstract ?? classType);
                        break;
                }
            }
        }

    }
}
