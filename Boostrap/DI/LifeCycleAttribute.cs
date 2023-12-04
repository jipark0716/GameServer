namespace Boostrap.DI
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Interface)]
    public class LifeCycleAttribute : Attribute
    {
        public readonly LifeCycle LifeCycle;
        public readonly Type? Abstract;
        public LifeCycleAttribute(
            LifeCycle lifeCycle = LifeCycle.Scoped,
            Type? @abstract = null) {
            LifeCycle = lifeCycle;
            Abstract = @abstract;
        }
    }

    public enum LifeCycle
    {
        Singleton,
        Scoped,
        Transient,
    }
}
