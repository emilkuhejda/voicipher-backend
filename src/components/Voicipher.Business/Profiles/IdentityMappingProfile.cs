using AutoMapper;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels;

namespace Voicipher.Business.Profiles
{
    public class IdentityMappingProfile : Profile
    {
        public IdentityMappingProfile()
        {
            CreateMap<User, IdentityOutputModel>()
                .ForMember(
                    i => i.Id,
                    opt => opt.MapFrom(u => u.Id))
                .ForMember(
                    i => i.Email,
                    opt => opt.MapFrom(u => u.Email))
                .ForMember(
                    i => i.GivenName,
                    opt => opt.MapFrom(u => u.GivenName))
                .ForMember(i => i.FamilyName,
                    opt => opt.MapFrom(u => u.FamilyName));
        }
    }
}
