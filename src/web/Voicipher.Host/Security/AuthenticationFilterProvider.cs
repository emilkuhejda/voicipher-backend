using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Voicipher.Host.Security
{
    public class AuthenticationFilterProvider : DefaultFilterProvider
    {
        private readonly FilterProviderOption[] _options;

        public AuthenticationFilterProvider(params FilterProviderOption[] options)
        {
            _options = options;
        }

        public override void ProvideFilter(FilterProviderContext context, FilterItem filterItem)
        {
            var route = context.ActionContext.ActionDescriptor.AttributeRouteInfo.Template;

            var filter = _options.FirstOrDefault(option => route.StartsWith(option.RoutePrefix, StringComparison.OrdinalIgnoreCase))?.Filter;
            if (filter != null)
            {
                if (context.Results.All(r => r.Descriptor.Filter != filter))
                {
                    context.Results.Add(new FilterItem(new FilterDescriptor(filter, FilterScope.Controller)));
                }
            }

            base.ProvideFilter(context, filterItem);
        }
    }
}
