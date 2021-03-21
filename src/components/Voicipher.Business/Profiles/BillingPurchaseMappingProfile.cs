using System;
using System.Collections.Generic;
using AutoMapper;
using Voicipher.Domain.Models;
using Voicipher.Domain.Payloads;

namespace Voicipher.Business.Profiles
{
    public class BillingPurchaseMappingProfile : Profile
    {
        public BillingPurchaseMappingProfile()
        {
            CreateMap<CreateUserSubscriptionPayload, BillingPurchase>()
                .ForMember(
                    c => c.Id,
                    opt => opt.MapFrom(x => x.Id))
                .ForMember(
                    c => c.UserId,
                    opt => opt.MapFrom(x => x.UserId))
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
                    c => c.ConsumptionState,
                    opt => opt.MapFrom(x => x.ConsumptionState))
                .ForMember(
                    c => c.Platform,
                    opt => opt.MapFrom(x => x.Platform))
                .ForMember(
                    c => c.TransactionDateUtc,
                    opt => opt.MapFrom(x => x.TransactionDateUtc))
                .ForMember(
                    c => c.PurchaseStateTransactions,
                    opt => opt.MapFrom(x => new List<PurchaseStateTransaction>
                    {
                        new()
                        {
                            Id = Guid.NewGuid(),
                            BillingPurchaseId = x.Id,
                            PreviousPurchaseState = string.Empty,
                            PurchaseState = x.PurchaseState,
                            TransactionDateUtc = DateTime.UtcNow
                        }
                    }));
        }
    }
}
