using AutoMapper;
using Voicipher.Domain.InputModels.ControlPanel;
using Voicipher.Domain.Payloads.ControlPanel;

namespace Voicipher.Business.Profiles.ControlPanel
{
    public class ImportAudioFileMappingProfile : Profile
    {
        public ImportAudioFileMappingProfile()
        {
            CreateMap<ImportAudioFileInputModel, ImportAudioFilePayload>()
                .ForMember(
                    i => i.UsersJsonPath,
                    opt => opt.MapFrom(x => x.UsersJsonPath))
                .ForMember(
                    i => i.SubscriptionsJsonPath,
                    opt => opt.MapFrom(x => x.SubscriptionsJsonPath))
                .ForMember(
                    i => i.AlternativesJsonPath,
                    opt => opt.MapFrom(x => x.AlternativesJsonPath))
                .ForMember(
                    i => i.UploadsDirectoryPath,
                    opt => opt.MapFrom(x => x.UploadsDirectoryPath));
        }
    }
}
