using System;
using AutoMapper;
using Voicipher.Domain.InputModels;
using Voicipher.Domain.Models;

namespace Voicipher.Business.Profiles
{
    public class SpeechResultMappingProfile : Profile
    {
        public SpeechResultMappingProfile()
        {
            CreateMap<CreateSpeechResultInputModel, SpeechResult>()
                .ForMember(
                    s => s.Id,
                    opt => opt.MapFrom(c => Guid.NewGuid()))
                .ForMember(
                    s => s.RecognizedAudioSampleId,
                    opt => opt.MapFrom(c => c.RecognizedAudioSampleId))
                .ForMember(
                    s => s.DisplayText,
                    opt => opt.MapFrom(c => c.DisplayText))
                .ForMember(
                    s => s.TotalTime,
                    opt => opt.MapFrom(c => TimeSpan.Zero));

            CreateMap<SpeechResultInputModel, SpeechResult>()
                .ForMember(
                    s => s.Id,
                    opt => opt.MapFrom(c => c.Id))
                .ForMember(
                    s => s.RecognizedAudioSampleId,
                    opt => opt.Ignore())
                .ForMember(
                    s => s.DisplayText,
                    opt => opt.Ignore())
                .ForMember(
                    s => s.TotalTime,
                    opt => opt.MapFrom(c => TimeSpan.FromTicks(c.Ticks)));
        }
    }
}
