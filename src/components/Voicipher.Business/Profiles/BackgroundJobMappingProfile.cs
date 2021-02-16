using System;
using System.Collections.Generic;
using AutoMapper;
using Newtonsoft.Json;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Models;
using Voicipher.Domain.Payloads.Job;

namespace Voicipher.Business.Profiles
{
    public class BackgroundJobMappingProfile : Profile
    {
        public BackgroundJobMappingProfile()
        {
            CreateMap<CreateBackgroundJobPayload, BackgroundJob>()
                .ForMember(
                    j => j.Id,
                    opt => opt.MapFrom(c => Guid.NewGuid()))
                .ForMember(
                    j => j.UserId,
                    opt => opt.MapFrom(c => c.UserId))
                .ForMember(
                    j => j.AudioFileId,
                    opt => opt.MapFrom(c => c.AudioFileId))
                .ForMember(
                    j => j.JobState,
                    opt => opt.MapFrom(c => JobState.Idle))
                .ForMember(
                    j => j.Attempt,
                    opt => opt.MapFrom(c => 0))
                .ForMember(
                    j => j.Parameters,
                    opt => opt.MapFrom(c => JsonConvert.SerializeObject(c.Parameters)))
                .ForMember(
                    j => j.DateCreatedUtc,
                    opt => opt.MapFrom(c => DateTime.UtcNow));

            CreateMap<BackgroundJob, BackgroundJobPayload>()
                .ForMember(
                    p => p.Id,
                    opt => opt.MapFrom(j => j.Id))
                .ForMember(
                    p => p.UserId,
                    opt => opt.MapFrom(j => j.UserId))
                .ForMember(
                    p => p.AudioFileId,
                    opt => opt.MapFrom(j => j.AudioFileId))
                .ForMember(
                    p => p.JobState,
                    opt => opt.MapFrom(j => j.JobState))
                .ForMember(
                    p => p.Attempt,
                    opt => opt.MapFrom(j => j.Attempt))
                .ForMember(
                    p => p.Parameters,
                    opt => opt.Ignore())
                .ForMember(
                    p => p.DateCreatedUtc,
                    opt => opt.MapFrom(j => j.DateCreatedUtc))
                .AfterMap((j, p) =>
                {
                    var dictionary = JsonConvert.DeserializeObject<Dictionary<BackgroundJobParameter, object>>(j.Parameters);
                    foreach (var (key, value) in dictionary)
                    {
                        p.Parameters.Add(key, value);
                    }
                });
        }
    }
}
