using System.Linq;
using AutoMapper;
using Voicipher.Business.Extensions;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels.Audio;

namespace Voicipher.Business.Profiles
{
    public class TranscribeItemMappingProfile : Profile
    {
        public TranscribeItemMappingProfile()
        {
            CreateMap<TranscribeItem, TranscribeItemOutputModel>()
                .ForMember(
                    t => t.Id,
                    opt => opt.MapFrom(x => x.Id))
                .ForMember(
                    t => t.FileItemId,
                    opt => opt.MapFrom(x => x.AudioFileId))
                .ForMember(
                    t => t.Alternatives,
                    opt => opt.Ignore())
                .ForMember(
                    t => t.UserTranscript,
                    opt => opt.MapFrom(x => x.UserTranscript))
                .ForMember(
                    t => t.StartTimeTicks,
                    opt => opt.MapFrom(x => x.StartTime.Ticks))
                .ForMember(
                    t => t.EndTimeTicks,
                    opt => opt.MapFrom(x => x.EndTime.Ticks))
                .ForMember(
                    t => t.TotalTimeTicks,
                    opt => opt.MapFrom(x => x.TotalTime.Ticks))
                .ForMember(
                    t => t.IsIncomplete,
                    opt => opt.MapFrom(x => x.IsIncomplete))
                .ForMember(
                    t => t.DateCreatedUtc,
                    opt => opt.MapFrom(x => x.DateCreatedUtc))
                .ForMember(
                    t => t.DateCreatedUtc,
                    opt => opt.MapFrom(x => x.DateUpdatedUtc))
                .AfterMap((t, o, c) =>
                {
                    var outputModels = t.GetAlternatives().Select(x => c.Mapper.Map<RecognitionAlternativeOutputModel>(x));
                    foreach (var model in outputModels)
                    {
                        o.Alternatives.Add(model);
                    }
                });
        }
    }
}
