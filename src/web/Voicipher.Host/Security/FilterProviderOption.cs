using Microsoft.AspNetCore.Mvc.Authorization;

namespace Voicipher.Host.Security
{
    public class FilterProviderOption
    {
        public string RoutePrefix { get; set; }

        public AuthorizeFilter Filter { get; set; }
    }
}
