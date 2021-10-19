using SimpleInjector;
using System;
using System.Diagnostics;

namespace MrErsh.RadioRipper.Client.Services
{
    /// <summary>
    /// Фасад для работы с контейнером />.
    /// </summary>
    public class ServiceContainer
    {
        #region ContainerConfigurator
        public class ContainerConfigurator
        {
            public ContainerConfigurator Register<TConcrete>() where TConcrete : class
            {
                _container.Register<TConcrete>();
                return this;
            }

            public ContainerConfigurator RegisterSingleton<TService>(Func<TService> instanceCreator) where TService : class
            {
                _container.RegisterSingleton<TService>(instanceCreator);
                return this;
            }

            public ContainerConfigurator RegisterSingleton<TService, TImplementation>() 
                where TService : class
                where TImplementation : class, TService
            {
                _container.RegisterSingleton<TService, TImplementation>();
                return this;
            }

            public ContainerConfigurator RegisterSingleton<TService>() where TService : class
            {
                _container.Register<TService, TService>(Lifestyle.Singleton);
                return this;
            }

            public ContainerConfigurator Register<TService, TImplementation>()
                where TService : class
                where TImplementation : class, TService
            {
                _container.Register<TService, TImplementation>();
                return this;
            }

            public void Configure()
            {
                _isInitialized = _isInitialized
                    ? throw new Exception("ServiceContainer already configured")
                    : _isInitialized = true;
#if DEBUG || DEBUG_FAKEDATA
                 _container.Verify();
#endif
            }
        }
        #endregion

        private readonly static Container _container = new ();
        private static bool _isInitialized;

        static ServiceContainer()
        {
            _container.ResolveUnregisteredType += ContainerOnResolveUnregisteredType;
        }

        public static TService Get<TService>()
            where TService : class
        {
            return _isInitialized 
                ? _container.GetInstance<TService>()
                : throw new Exception("Service container should be configured first");
        }

        private static void ContainerOnResolveUnregisteredType(object sender, UnregisteredTypeEventArgs e)
            => Debug.Fail($"Type {e.UnregisteredServiceType} is not registered in container");
    }
}
