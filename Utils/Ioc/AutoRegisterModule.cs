
using Autofac;
using System.Reflection;
using Utils.Enumerable;

namespace Utils.Ioc
{
    public class AutoRegisterModule : Autofac.Module
    {
        private readonly Assembly[] _assemblies;

        /// <summary>
        /// 初始化模块，指定要扫描的程序集
        /// </summary>
        /// <param name="assemblies">需要扫描的程序集</param>
        public AutoRegisterModule(params Assembly[] assemblies)
        {
            _assemblies = assemblies ?? throw new ArgumentNullException(
                nameof(assemblies), "必须指定至少一个要扫描的程序集");
        }
        protected override void Load(ContainerBuilder builder)
        {
            // 扫描所有指定程序集中带有AutoRegisterAttribute的非抽象类
            var targetTypes = _assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type =>
                    type.IsClass &&
                    !type.IsAbstract &&
                    type.GetCustomAttribute<RegisterAttribute>() != null)
                .ToList();

            if (!targetTypes.HasItem())
            {
                return;
            }

            foreach (var type in targetTypes)
            {
                var attribute = type.GetCustomAttribute<RegisterAttribute>()!;
                RegisterType(builder, type, attribute);
            }
        }

        /// <summary>
        /// 根据特性配置注册类型
        /// </summary>
        private void RegisterType(ContainerBuilder builder, Type implementationType, RegisterAttribute attribute)
        {
            // 确定服务类型（接口）和实现类型
            var serviceType = attribute.ServiceType ?? GetServiceType(implementationType);
            var registration = builder.RegisterType(implementationType).As(serviceType);

            // 设置生命周期
            switch (attribute.Lifetime)
            {
                case Lifetime.Singleton:
                    registration.SingleInstance();
                    break;
                case Lifetime.Scoped:
                    registration.InstancePerLifetimeScope();
                    break;
                case Lifetime.Transient:
                default:
                    registration.InstancePerDependency();
                    break;
            }
        }

        /// <summary>
        /// 自动推断服务类型（优先匹配"接口名=I+实现类名"的接口）
        /// </summary>
        private Type GetServiceType(Type implementationType)
        {
            var interfaces = implementationType.GetInterfaces();
            if (interfaces.Length == 0)
            {
                // 没有实现接口时，以自身作为服务类型
                return implementationType;
            }

            // 优先匹配 I+类名 的接口（如 UserService -> IUserService）
            var matchedInterface = interfaces.FirstOrDefault(
                i => i.Name.Equals($"I{implementationType.Name}", StringComparison.Ordinal));

            return matchedInterface ?? interfaces.First();
        }
    }
}
