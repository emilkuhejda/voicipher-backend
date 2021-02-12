using Voicipher.Domain.Enums;

namespace Voicipher.Domain.OutputModels.Audio
{
    public record StorageConfigurationOutputModel
    {
        public StorageConfigurationOutputModel(StorageSetting storageSetting)
        {
            StorageSetting = storageSetting;
        }

        public StorageSetting StorageSetting { get; }
    }
}
