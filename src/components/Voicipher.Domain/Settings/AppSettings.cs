using System;

namespace Voicipher.Domain.Settings
{
    public class AppSettings
    {
        public Guid ApplicationId { get; set; }

        public string SecretKey { get; set; }

        public string ConnectionString { get; set; }

        public Authentication Authentication { get; set; }

        public AzureStorageAccountSettings AzureStorageAccount { get; set; }
    }
}
