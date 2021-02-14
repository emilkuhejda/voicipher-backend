using AutoMapper;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels.Audio;

namespace Voicipher.Business.Profiles
{
    public class AudioFileMappingProfile : Profile
    {
        public AudioFileMappingProfile()
        {
            CreateMap<AudioFile, FileItemOutputModel>()
                .ForMember(
                    f => f.Id,
                    opt => opt.MapFrom(a => a.Id))
                .ForMember(
                    f => f.Name,
                    opt => opt.MapFrom(a => a.Name))
                .ForMember(
                    f => f.FileName,
                    opt => opt.MapFrom(a => a.FileName))
                .ForMember(
                    f => f.Language,
                    opt => opt.MapFrom(a => a.Language))
                .ForMember(
                    f => f.RecognitionStateString,
                    opt => opt.MapFrom(a => a.RecognitionState.ToString()))
                .ForMember(
                    f => f.UploadStatus,
                    opt => opt.MapFrom(a => a.UploadStatus))
                .ForMember(
                    f => f.TotalTimeTicks,
                    opt => opt.MapFrom(a => a.TotalTime.Ticks))
                .ForMember(
                    f => f.TranscribedTimeTicks,
                    opt => opt.MapFrom(a => a.TranscribedTime.Ticks))
                .ForMember(
                    f => f.DateCreated,
                    opt => opt.MapFrom(a => a.DateCreated))
                .ForMember(
                    f => f.DateProcessedUtc,
                    opt => opt.MapFrom(a => a.DateProcessedUtc))
                .ForMember(
                    f => f.DateUpdatedUtc,
                    opt => opt.MapFrom(a => a.DateUpdatedUtc))
                .ForMember(
                    f => f.IsDeleted,
                    opt => opt.MapFrom(a => a.IsDeleted));
        }
    }
}
