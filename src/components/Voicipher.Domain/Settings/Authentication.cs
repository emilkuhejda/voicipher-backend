namespace Voicipher.Domain.Settings
{
    public class Authentication
    {
        public string Tenant { get; set; }

        public string ClientId { get; set; }

        public string PolicySignUpSignIn { get; set; }

        public string AuthorityBase => $"https://login.microsoftonline.com/tfp/{Tenant}/";

        public string AuthoritySignUpSignIn => $"{AuthorityBase}{PolicySignUpSignIn}";
    }
}
