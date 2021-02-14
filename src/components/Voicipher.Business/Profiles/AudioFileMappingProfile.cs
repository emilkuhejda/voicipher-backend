using AutoMapper;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels.Audio;

namespace Voicipher.Business.Profiles
{
    public class AudioFileMappingProfile : Profile
    {
        public AudioFileMappingProfile()
        {
            CreateMap<AudioFile, AudioFileOutputModel>()
                .ForMember(
                    a => a.Id,
                    opt => opt.MapFrom(x => x.Id))
                .ForMember(
                    a => a.Name,
                    opt => opt.MapFrom(x => x.Name))
                .ForMember(
                    a => a.FileName,
                    opt => opt.MapFrom(x => x.FileName))
                .ForMember(
                    a => a.Language,
                    opt => opt.MapFrom(x => x.Language))
                .ForMember(
                    a => a.RecognitionStateString,
                    opt => opt.MapFrom(x => nameof(x.RecognitionState)))
                .ForMember(
                    a => a.UploadStatus,
                    opt => opt.MapFrom(x => x.UploadStatus))
                .ForMember(
                    a => a.TotalTimeTicks,
                    opt => opt.MapFrom(x => x.TotalTime.Ticks))
                .ForMember(
                    a => a.TranscribedTimeTicks,
                    opt => opt.MapFrom(x => x.TranscribedTime.Ticks))
                .ForMember(
                    a => a.DateCreated,
                    opt => opt.MapFrom(x => x.DateCreated))
                .ForMember(
                    a => a.DateProcessedUtc,
                    opt => opt.MapFrom(x => x.DateProcessedUtc))
                .ForMember(
                    a => a.DateUpdatedUtc,
                    opt => opt.MapFrom(x => x.DateUpdatedUtc))
                .ForMember(
                    a => a.IsDeleted,
                    opt => opt.MapFrom(x => x.IsDeleted));

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
