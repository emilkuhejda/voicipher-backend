using System;
using AutoMapper;
using Voicipher.Domain.Models;
using Voicipher.Domain.Payloads;

namespace Voicipher.Business.Profiles
{
    public class UserSubscriptionMappingProfile : Profile
    {
        public UserSubscriptionMappingProfile()
        {
            CreateMap<ModifySubscriptionTimePayload, UserSubscription>()
                .ForMember(
                    u => u.Id,
                    opt => opt.MapFrom(m => Guid.NewGuid()))
                .ForMember(
                    u => u.UserId,
                    opt => opt.Ignore())
                .ForMember(
                    u => u.ApplicationId,
                    opt => opt.MapFrom(m => m.ApplicationId))
                .ForMember(
                    u => u.Time,
                    opt => opt.MapFrom(m => m.Time))
                .ForMember(
                    u => u.Operation,
                    opt => opt.MapFrom(m => m.Operation))
                .ForMember(
                    u => u.DateCreatedUtc,
                    opt => opt.MapFrom(m => DateTime.UtcNow));
        }
    }
}
