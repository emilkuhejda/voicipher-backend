using AutoMapper;
using Voicipher.Domain.Infrastructure;
using Voicipher.Domain.OutputModels;

namespace Voicipher.Business.Profiles
{
    public class ErrorResultMappingProfile : Profile
    {
        public ErrorResultMappingProfile()
        {
            CreateMap<QueryResultBase, ErrorResultOutputModel>()
                .ForMember(
                    e => e.Error,
                    opt => opt.MapFrom(q => q.Error))
                .ForMember(
                    e => e.ValidationErrors,
                    opt => opt.MapFrom(q => q.ValidationErrors));

            CreateMap<CommandResultBase, ErrorResultOutputModel>()
                .ForMember(
                    e => e.Error,
                    opt => opt.MapFrom(c => c.Error))
                .ForMember(
                    e => e.ValidationErrors,
                    opt => opt.MapFrom(c => c.ValidationErrors));
        }
    }
}
