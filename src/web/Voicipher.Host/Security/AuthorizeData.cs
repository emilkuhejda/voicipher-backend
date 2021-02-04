using Microsoft.AspNetCore.Authorization;

namespace Voicipher.Host.Security
{
    public class AuthorizeData : IAuthorizeData
    {
        public AuthorizeData()
        { }

        public AuthorizeData(string policy)
        {
            Policy = policy;
        }

        public string Policy { get; set; }

        public string Roles { get; set; }

        public string AuthenticationSchemes { get; set; }
    }
}
