namespace Utils.Ioc
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class RegisterAttribute : Attribute
    {
        public Lifetime Lifetime { get; set; } = Lifetime.Transient;
        public Type? ServiceType { get; set; }  // 可选：指定服务接口
    }

    public enum Lifetime
    {
        Singleton,
        Scoped,
        Transient
    }
}
