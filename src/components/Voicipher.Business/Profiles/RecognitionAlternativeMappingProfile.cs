using System.Linq;
using AutoMapper;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels.Audio;

namespace Voicipher.Business.Profiles
{
    public class RecognitionAlternativeMappingProfile : Profile
    {
        public RecognitionAlternativeMappingProfile()
        {
            CreateMap<RecognitionAlternative, RecognitionAlternativeOutputModel>()
                .ForMember(
                    r => r.ResultNumber,
                    opt => opt.MapFrom(x => x.ResultNumber))
                .ForMember(
                    r => r.Transcript,
                    opt => opt.MapFrom(x => x.Transcript))
                .ForMember(
                    r => r.Confidence,
                    opt => opt.MapFrom(x => x.Confidence))
                .ForMember(
                    r => r.Words,
                    opt => opt.Ignore())
                .AfterMap((r, o, c) =>
                {
                    var outputModel = r.Words.Select(x => c.Mapper.Map<RecognitionWordInfoOutputModel>(x));
                    foreach (var model in outputModel)
                    {
                        o.Words.Add(model);
                    }
                });
        }
    }
}
