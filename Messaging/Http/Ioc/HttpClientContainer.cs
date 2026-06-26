using Messaging.Http.Configurations;
using Messaging.Http.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Utils.Configuration;

namespace Messaging.Http.Ioc
{
    public sealed class HttpClientContainer
    {
        private static readonly Lazy<HttpClientContainer> _instance = new(() => new HttpClientContainer(), LazyThreadSafetyMode.ExecutionAndPublication);
        public static HttpClientContainer Instance => _instance.Value;
        
        private HttpClientContainer()
        {
            _serviceCollection = new ServiceCollection();
            _serviceProvider = new Lazy<IServiceProvider>(BuildProvider, LazyThreadSafetyMode.ExecutionAndPublication);

            SetupConfig();
        }
        
        private readonly IServiceCollection _serviceCollection;
        private readonly Lazy<IServiceProvider> _serviceProvider;
        public IServiceProvider ServiceProvider => _serviceProvider.Value;
        public IServiceCollection Services => _serviceCollection;
        
        private IServiceProvider BuildProvider()
        {
            // validate dependencies under development environment, identify dependency injection errors in advance
            return _serviceCollection.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });
        }

        private void SetupConfig()
        {
            try
            {
                IConfiguration httpConfig = AppConfig.Configuration!.GetSection("ApiSettings:Http");
                _serviceCollection.Configure<HttpApiClientConfig>(httpConfig);
            }
            catch (Exception e)
            {
                throw new HttpException(e, "Failed to read ApiSettings:Http node in appsettings.json");
            }
        }
        
        public HttpClientContainer RegisterService(Action<IServiceCollection> registerAction)
        {
            if (_serviceProvider.IsValueCreated)
                throw new InvalidOperationException("ServiceProvider has been built, not able to register new services");
            
            registerAction.Invoke(_serviceCollection);
            return this;
        }
    }
}
