using System.Reflection;
using Autofac;
using Voicipher.Domain.Interfaces.Repositories;
using Module = Autofac.Module;

namespace Voicipher.DataAccess
{
    public class DataAccessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            RegisterServices(builder);
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assembly).Where(t => t.IsClosedTypeOf(typeof(IRepository<>)))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<UnitOfWork>().AsImplementedInterfaces();
        }
    }
}
