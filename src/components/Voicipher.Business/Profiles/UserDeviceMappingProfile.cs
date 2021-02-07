using System;
using AutoMapper;
using Voicipher.Business.Extensions;
using Voicipher.Domain.InputModels.Authentication;
using Voicipher.Domain.Models;

namespace Voicipher.Business.Profiles
{
    public class UserDeviceMappingProfile : Profile
    {
        public UserDeviceMappingProfile()
        {
            CreateMap<UserRegistrationInputModel, UserDevice>()
                .ForMember(
                    u => u.Id,
                    opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(
                    u => u.UserId,
                    opt => opt.MapFrom(x => x.Id))
                .ForMember(
                    u => u.InstallationId,
                    opt => opt.MapFrom(x => x.Device.InstallationId))
                .ForMember(
                    u => u.RuntimePlatform,
                    opt => opt.MapFrom(x => x.Device.RuntimePlatform.ToRuntimePlatform()))
                .ForMember(
                    u => u.InstalledVersionNumber,
                    opt => opt.MapFrom(x => x.Device.InstalledVersionNumber))
                .ForMember(
                    u => u.Language,
                    opt => opt.MapFrom(x => x.Device.Language.ToLanguage()))
                .ForMember(u => u.DateRegisteredUtc,
                    opt => opt.MapFrom(_ => DateTime.UtcNow));
        }
    }
}
