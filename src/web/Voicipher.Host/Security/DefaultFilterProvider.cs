using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Voicipher.Host.Security
{
    public class DefaultFilterProvider : IFilterProvider
    {
        public int Order => -1000;

        public void OnProvidersExecuting(FilterProviderContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            for (var index = 0; index < context.Results.Count; ++index)
            {
                ProvideFilter(context, context.Results[index]);
            }
        }

        public void OnProvidersExecuted(FilterProviderContext context)
        {
        }

        public virtual void ProvideFilter(FilterProviderContext context, FilterItem filterItem)
        {
            var filter = filterItem.Descriptor.Filter;
            var filterFactory = filter as IFilterFactory;

            if (filterFactory == null)
            {
                filterItem.Filter = filter;
                filterItem.IsReusable = true;
            }
            else
            {
                var requestServices = context.ActionContext.HttpContext.RequestServices;
                filterItem.Filter = filterFactory.CreateInstance(requestServices);
                filterItem.IsReusable = filterFactory.IsReusable;
                if (filterItem.Filter == null)
                    throw new InvalidOperationException(nameof(IFilterFactory));

                ApplyFilterToContainer(filterItem.Filter, filterFactory);
            }
        }

        private void ApplyFilterToContainer(object actualFilter, IFilterMetadata filterMetadata)
        {
            var filterContainer = actualFilter as IFilterContainer;
            if (filterContainer == null)
                return;

            filterContainer.FilterDefinition = filterMetadata;
        }
    }
}
