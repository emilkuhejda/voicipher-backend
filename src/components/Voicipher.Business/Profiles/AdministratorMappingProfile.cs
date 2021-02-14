using AutoMapper;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels.MetaData;

namespace Voicipher.Business.Profiles
{
    public class AdministratorMappingProfile : Profile
    {
        public AdministratorMappingProfile()
        {
            CreateMap<Administrator, AdministratorOutputModel>()
                .ForMember(
                    a => a.Id,
                    opt => opt.MapFrom(x => x.Id))
                .ForMember(
                    a => a.Username,
                    opt => opt.MapFrom(x => x.Username))
                .ForMember(
                    a => a.FirstName,
                    opt => opt.MapFrom(x => x.FirstName))
                .ForMember(
                    a => a.LastName,
                    opt => opt.MapFrom(x => x.LastName))
                .ForMember(
                    a => a.Token,
                    opt => opt.Ignore());
        }
    }
}
