using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Security;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;

namespace YATDL
{
    public static class DependencyRegistrator
    {
        public static void RegisterDependencies()
        {
            var cBuilder = new ContainerBuilder();

            // Все зависимости скидываем сюда
            cBuilder.RegisterType<YATDLLogger>().As<Logger>().SingleInstance();
            cBuilder.RegisterType<EfUnitOfWork>().As<IUnitOfWork>().InstancePerRequest();
            cBuilder.RegisterType<YATDLContext>().As<IDbContext>().InstancePerRequest();
            cBuilder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).InstancePerRequest();

            cBuilder.RegisterModelBinders();
            cBuilder.RegisterModule(new AutofacWebTypesModule());
            cBuilder.RegisterControllers(Assembly.GetExecutingAssembly());

            var container = cBuilder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(container));
        }
    }



    /// <summary>
    /// Autofac implementation of the Microsoft CommonServiceLocator.
    /// </summary>
    public class AutofacServiceLocator : ServiceLocatorImplBase
    {
        /// <summary>
        /// The <see cref="Autofac.IComponentContext"/> from which services
        /// should be located.
        /// </summary>
        private readonly IComponentContext _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacServiceLocator" /> class.
        /// </summary>
        [SecuritySafeCritical]
        protected AutofacServiceLocator()
        {
            // This constructor needs to be here for SecAnnotate/CoreCLR security purposes
            // but doesn't get used in standard situations.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacServiceLocator" /> class.
        /// </summary>
        /// <param name="container">
        /// The <see cref="Autofac.IComponentContext"/> from which services
        /// should be located.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if <paramref name="container" /> is <see langword="null" />.
        /// </exception>
        [SecuritySafeCritical]
        public AutofacServiceLocator(IComponentContext container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }
            _container = container;
        }



        /// <summary>
        /// Resolves the requested service instance.
        /// </summary>
        /// <param name="serviceType">Type of instance requested.</param>
        /// <param name="key">Name of registered service you want. May be <see langword="null" />.</param>
        /// <returns>The requested service instance.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if <paramref name="serviceType" /> is <see langword="null" />.
        /// </exception>
        protected override object DoGetInstance(Type serviceType, string key)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }
            return key != null ? _container.ResolveNamed(key, serviceType) : _container.Resolve(serviceType);
        }



        /// <summary>
        /// Resolves all requested service instances.
        /// </summary>
        /// <param name="serviceType">Type of instance requested.</param>
        /// <returns>Sequence of service instance objects.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown if <paramref name="serviceType" /> is <see langword="null" />.
        /// </exception>
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(serviceType);

            object instance = _container.Resolve(enumerableType);
            return ((IEnumerable)instance).Cast<object>();

        }
    }

}