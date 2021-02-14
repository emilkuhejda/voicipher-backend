using AutoMapper;
using Voicipher.Domain.InputModels.Audio;
using Voicipher.Domain.Models;

namespace Voicipher.Business.Profiles
{
    public class DeletedAudioFileMappingProfile : Profile
    {
        public DeletedAudioFileMappingProfile()
        {
            CreateMap<DeletedAudioFileInputModel, DeletedAudioFile>()
                .ForMember(
                    d => d.Id,
                    opt => opt.MapFrom(x => x.Id))
                .ForMember(
                    d => d.DeletedDate,
                    opt => opt.MapFrom(x => x.DeletedDate));
        }
    }
}
