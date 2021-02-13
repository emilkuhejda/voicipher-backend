using AutoMapper;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels;

namespace Voicipher.Business.Profiles
{
    public class LanguageVersionMappingProfile : Profile
    {
        public LanguageVersionMappingProfile()
        {
            CreateMap<LanguageVersion, LanguageVersionOutputModel>()
                .ForMember(
                    o => o.Id,
                    opt => opt.MapFrom(x => x.Id))
                .ForMember(
                    o => o.InformationMessageId,
                    opt => opt.MapFrom(x => x.InformationMessageId))
                .ForMember(
                    o => o.Title,
                    opt => opt.MapFrom(x => x.Title))
                .ForMember(
                    o => o.Message,
                    opt => opt.MapFrom(x => x.Message))
                .ForMember(
                    o => o.Description,
                    opt => opt.MapFrom(x => x.Description))
                .ForMember(
                    o => o.LanguageString,
                    opt => opt.MapFrom(x => x.Language.ToString()))
                .ForMember(
                    o => o.SentOnOsx,
                    opt => opt.MapFrom(x => x.SentOnOsx))
                .ForMember(
                    o => o.SentOnAndroid,
                    opt => opt.MapFrom(x => x.SentOnAndroid));
        }
    }
}
