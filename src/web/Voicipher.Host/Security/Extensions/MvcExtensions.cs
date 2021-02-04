using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Voicipher.Host.Security.Extensions
{
    public static class MvcExtensions
    {
        public static IMvcBuilder AddFilterProvider(this IMvcBuilder builder, Func<IServiceProvider, IFilterProvider> provideFilter)
        {
            builder.Services.Replace(ServiceDescriptor.Transient(provideFilter));

            return builder;
        }
    }
}
