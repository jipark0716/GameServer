using System.Reflection;

namespace Boostrap.DI
{
    public static class AssamblyExtendsion
    {
        public static IEnumerable<Type> GetAllClasses<T>()
            where T : Attribute
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var result in assembly.Classes<T>())
                {
                    yield return result;
                }
            }
        }

        public static IEnumerable<MethodInfo> GetMethods<T>(this Type source)
            where T : Attribute
        {
            return source.GetMethods().Where(o => o.GetCustomAttribute<T>() is not null);
        }

        public static IEnumerable<Type> Classes<T>(this Assembly assembly)
            where T : Attribute
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetCustomAttributes(typeof(T), true).Length > 0)
                {
                    yield return type;
                }
            }
        }
    }
}
