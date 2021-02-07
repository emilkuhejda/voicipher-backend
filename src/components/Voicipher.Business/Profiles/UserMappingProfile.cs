using System;
using System.Collections.Generic;
using AutoMapper;
using Voicipher.Business.Extensions;
using Voicipher.Domain.Enums;
using Voicipher.Domain.InputModels.Authentication;
using Voicipher.Domain.Models;

namespace Voicipher.Business.Profiles
{
    public class UserMappingProfile : Profile
    {
        private const int FreeUserSubscriptionMinutes = 10;

        public UserMappingProfile()
        {
            CreateMap<UserRegistrationInputModel, User>()
                .ForMember(
                    u => u.Id,
                    opt => opt.MapFrom(x => x.Id))
                .ForMember(
                    u => u.Email,
                    opt => opt.MapFrom(x => x.Email))
                .ForMember(
                    u => u.GivenName,
                    opt => opt.MapFrom(x => x.GivenName))
                .ForMember(
                    u => u.FamilyName,
                    opt => opt.MapFrom(x => x.FamilyName))
                .ForMember(
                    u => u.DateRegisteredUtc,
                    opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(
                    u => u.CurrentUserSubscription,
                    opt => opt.Ignore())
                .ForMember(
                    u => u.UserSubscriptions,
                    opt => opt.Ignore())
                .ForMember(
                    u => u.UserDevices,
                    opt => opt.Ignore())
                .AfterMap((m, u) =>
                {
                    var userSubscription = CreateUserSubscription(u, m.ApplicationId);
                    u.UserSubscriptions = new List<UserSubscription> { userSubscription };
                    u.CurrentUserSubscription = CreateCurrentUserSubscription(userSubscription);

                    if (m.Device != null)
                    {
                        u.UserDevices = new List<UserDevice> { CreateUserDevice(u, m.Device) };
                    }
                });
        }

        private static UserSubscription CreateUserSubscription(User user, Guid applicationId)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                ApplicationId = applicationId,
                Time = TimeSpan.FromMinutes(FreeUserSubscriptionMinutes),
                Operation = SubscriptionOperation.Add,
                DateCreatedUtc = DateTime.UtcNow
            };
        }

        private static CurrentUserSubscription CreateCurrentUserSubscription(UserSubscription userSubscription)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                UserId = userSubscription.Id,
                Ticks = userSubscription.Time.Ticks,
                DateUpdatedUtc = DateTime.UtcNow
            };
        }

        private static UserDevice CreateUserDevice(User user, RegistrationDeviceInputModel registrationDeviceInputModel)
        {
            return new()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                InstallationId = registrationDeviceInputModel.InstallationId,
                RuntimePlatform = registrationDeviceInputModel.RuntimePlatform.ToRuntimePlatform(),
                InstalledVersionNumber = registrationDeviceInputModel.InstalledVersionNumber,
                Language = registrationDeviceInputModel.Language.ToLanguage(),
                DateRegisteredUtc = DateTime.UtcNow
            };
        }
    }
}
