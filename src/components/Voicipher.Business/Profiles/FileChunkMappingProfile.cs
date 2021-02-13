using System;
using AutoMapper;
using Voicipher.Domain.Models;
using Voicipher.Domain.Payloads.Audio;

namespace Voicipher.Business.Profiles
{
    public class FileChunkMappingProfile : Profile
    {
        public FileChunkMappingProfile()
        {
            CreateMap<UploadChunkFilePayload, FileChunk>()
                .ForMember(
                    f => f.Id,
                    opt => opt.MapFrom(u => Guid.NewGuid()))
                .ForMember(
                    f => f.FileItemId,
                    opt => opt.MapFrom(u => u.FileItemId))
                .ForMember(
                    f => f.ApplicationId,
                    opt => opt.MapFrom(u => u.ApplicationId))
                .ForMember(
                    f => f.Order,
                    opt => opt.MapFrom(u => u.Order))
                .ForMember(
                    f => f.Path,
                    opt => opt.Ignore())
                .ForMember(
                    f => f.DateCreatedUtc,
                    opt => opt.MapFrom(u => DateTime.UtcNow));
        }
    }
}
