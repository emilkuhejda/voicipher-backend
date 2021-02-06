using System.Collections.Generic;
using Autofac;
using AutofacSerilogIntegration;
using AutoMapper;
using Voicipher.Business;
using Voicipher.DataAccess;
using Voicipher.Host.Filters;

namespace Voicipher.Host.Configuration
{
    public class ApplicationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            RegisterModules(builder);
            RegisterServices(builder);
            ConfigureFilters(builder);
            ConfigureMappings(builder);
        }

        private void RegisterModules(ContainerBuilder builder)
        {
            builder.RegisterModule<BusinessModule>();
            builder.RegisterModule<DataAccessModule>();
        }

        public static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterLogger();
        }

        private static void ConfigureFilters(ContainerBuilder builder)
        {
            builder.RegisterType<ApiExceptionFilter>().AsSelf();
        }

        private static void ConfigureMappings(ContainerBuilder builder)
        {
            builder.Register(context =>
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        foreach (var profile in context.Resolve<IEnumerable<Profile>>())
                        {
                            cfg.AddProfile(profile);
                        }
                    });

                    config.AssertConfigurationIsValid();
                    return config.CreateMapper();
                })
                .As<IMapper>()
                .SingleInstance()
                .AutoActivate();
        }
    }
}
