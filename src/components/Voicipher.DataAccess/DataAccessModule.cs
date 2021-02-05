using Autofac;

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
            builder.RegisterType<UnitOfWork>().AsImplementedInterfaces();
        }
    }
}
