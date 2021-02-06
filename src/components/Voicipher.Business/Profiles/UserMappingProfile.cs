using System;
using AutoMapper;
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
                .ForMember(u => u.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(u => u.Email, opt => opt.MapFrom(x => x.Email))
                .ForMember(u => u.GivenName, opt => opt.MapFrom(x => x.GivenName))
                .ForMember(u => u.FamilyName, opt => opt.MapFrom(x => x.FamilyName))
                .ForMember(u => u.DateRegisteredUtc, opt => opt.MapFrom(_ => DateTime.Now))
                .ForMember(u => u.CurrentUserSubscription, opt => opt.Ignore())
                .ForMember(u => u.UserSubscriptions, opt => opt.Ignore())
                .AfterMap((m, u) =>
                {
                    var userSubscription = CreateUserSubscription(u, m.ApplicationId);
                    u.UserSubscriptions.Add(userSubscription);
                    u.CurrentUserSubscription = CreateCurrentUserSubscription(userSubscription);
                });
        }

        private static UserSubscription CreateUserSubscription(User user, Guid applicationId)
        {
            return new UserSubscription
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
            return new CurrentUserSubscription
            {
                Id = Guid.NewGuid(),
                UserId = userSubscription.Id,
                Ticks = userSubscription.Time.Ticks,
                DateUpdatedUtc = DateTime.UtcNow
            };
        }
    }
}
