using System;
using AutoMapper;
using Voicipher.Business.Extensions;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Models;
using Voicipher.Domain.Payloads.Job;

namespace Voicipher.Business.Profiles
{
    public class RecognitionFileMappingProfile : Profile
    {
        public RecognitionFileMappingProfile()
        {
            CreateMap<BackgroundJobPayload, RecognitionFile>()
                .ForMember(
                    r => r.UserId,
                    opt => opt.MapFrom(j => j.UserId))
                .ForMember(
                    r => r.AudioFileId,
                    opt => opt.MapFrom(j => j.AudioFileId))
                .ForMember(
                    r => r.JobId,
                    opt => opt.MapFrom(j => j.Id))
                .ForMember(
                    r => r.FileName,
                    opt => opt.MapFrom(j => j.GetParameter(BackgroundJobParameter.FileName, string.Empty)))
                .ForMember(
                    r => r.DateProcessedUtc,
                    opt => opt.MapFrom(j => j.GetParameter(BackgroundJobParameter.DateUtc, DateTime.MinValue)));
        }
    }
}
