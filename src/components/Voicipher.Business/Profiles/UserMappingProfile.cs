using System;
using AutoMapper;
using Voicipher.Domain.InputModels.Authentication;
using Voicipher.Domain.Models;

namespace Voicipher.Business.Profiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<UserRegistrationInputModel, User>()
                .ForMember(u => u.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(u => u.Email, opt => opt.MapFrom(x => x.Email))
                .ForMember(u => u.GivenName, opt => opt.MapFrom(x => x.GivenName))
                .ForMember(u => u.FamilyName, opt => opt.MapFrom(x => x.FamilyName))
                .ForMember(u => u.DateRegisteredUtc, opt => opt.MapFrom(_ => DateTime.Now));
        }
    }
}
