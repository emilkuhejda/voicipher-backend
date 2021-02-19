using System;
using AutoMapper;
using Voicipher.Domain.InputModels.ControlPanel;
using Voicipher.Domain.Payloads;

namespace Voicipher.Business.Profiles.ControlPanel
{
    public class ModifySubscriptionTimeMappingProfile : Profile
    {
        public ModifySubscriptionTimeMappingProfile()
        {
            CreateMap<CreateSubscriptionInputModel, ModifySubscriptionTimePayload>()
                .ForMember(
                    m => m.UserId,
                    opt => opt.MapFrom(x => x.UserId))
                .ForMember(
                    m => m.ApplicationId,
                    opt => opt.MapFrom(x => x.ApplicationId))
                .ForMember(
                    m => m.Time,
                    opt => opt.MapFrom(x => TimeSpan.FromSeconds(x.Seconds)))
                .ForMember(
                    m => m.Operation,
                    opt => opt.MapFrom(x => x.Operation));
        }
    }
}
