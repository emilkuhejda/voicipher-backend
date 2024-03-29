﻿using AutoMapper;
using Voicipher.Domain.InputModels;
using Voicipher.Domain.Payloads;

namespace Voicipher.Business.Profiles
{
    public class CreateUserSubscriptionMappingProfile : Profile
    {
        public CreateUserSubscriptionMappingProfile()
        {
            CreateMap<CreateUserSubscriptionInputModel, CreateUserSubscriptionPayload>()
                .ForMember(
                    c => c.Id,
                    opt => opt.MapFrom(x => x.Id))
                .ForMember(
                    c => c.UserId,
                    opt => opt.MapFrom(x => x.UserId))
                .ForMember(
                    c => c.ApplicationId,
                    opt => opt.Ignore())
                .ForMember(
                    c => c.PurchaseId,
                    opt => opt.MapFrom(x => x.PurchaseId))
                .ForMember(
                    c => c.ProductId,
                    opt => opt.MapFrom(x => x.ProductId))
                .ForMember(
                    c => c.AutoRenewing,
                    opt => opt.MapFrom(x => x.AutoRenewing))
                .ForMember(
                    c => c.PurchaseToken,
                    opt => opt.MapFrom(x => x.PurchaseToken))
                .ForMember(
                    c => c.PurchaseState,
                    opt => opt.MapFrom(x => x.PurchaseState))
                .ForMember(
                    c => c.ConsumptionState,
                    opt => opt.MapFrom(x => x.ConsumptionState))
                .ForMember(
                    c => c.Platform,
                    opt => opt.MapFrom(x => x.Platform))
                .ForMember(
                    c => c.TransactionDateUtc,
                    opt => opt.MapFrom(x => x.TransactionDateUtc));
        }
    }
}
