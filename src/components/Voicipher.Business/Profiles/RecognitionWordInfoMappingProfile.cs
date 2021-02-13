using AutoMapper;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels.Audio;

namespace Voicipher.Business.Profiles
{
    public class RecognitionWordInfoMappingProfile : Profile
    {
        public RecognitionWordInfoMappingProfile()
        {
            CreateMap<RecognitionWordInfo, RecognitionWordInfoOutputModel>()
                .ForMember(
                    r => r.Word,
                    opt => opt.MapFrom(x => x.Word))
                .ForMember(
                    r => r.StartTimeTicks,
                    opt => opt.MapFrom(x => x.StartTime.Ticks))
                .ForMember(
                    r => r.EndTimeTicks,
                    opt => opt.MapFrom(x => x.EndTime.Ticks))
                .ForMember(
                    r => r.SpeakerTag,
                    opt => opt.MapFrom(x => x.SpeakerTag));
        }
    }
}
