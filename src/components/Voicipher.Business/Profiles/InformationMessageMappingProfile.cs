using System.Linq;
using AutoMapper;
using Voicipher.Domain.Models;
using Voicipher.Domain.OutputModels;

namespace Voicipher.Business.Profiles
{
    public class InformationMessageMappingProfile : Profile
    {
        public InformationMessageMappingProfile()
        {
            CreateMap<InformationMessage, InformationMessageOutputModel>()
                .ForMember(
                    o => o.Id,
                    opt => opt.MapFrom(x => x.Id))
                .ForMember(
                    o => o.IsUserSpecific,
                    opt => opt.MapFrom(x => x.UserId.HasValue))
                .ForMember(
                    o => o.WasOpened,
                    opt => opt.MapFrom(x => x.WasOpened))
                .ForMember(
                    o => o.DateUpdatedUtc,
                    opt => opt.MapFrom(x => x.DateUpdatedUtc.GetValueOrDefault()))
                .ForMember(
                    o => o.DatePublishedUtc,
                    opt => opt.MapFrom(x => x.DatePublishedUtc.GetValueOrDefault()))
                .ForMember(
                    o => o.LanguageVersions,
                    opt => opt.Ignore())
                .AfterMap((i, o, c) =>
                {
                    var languageVersions = i.LanguageVersions.Select(x => c.Mapper.Map<LanguageVersionOutputModel>(x));
                    foreach (var languageVersion in languageVersions)
                    {
                        o.LanguageVersions.Add(languageVersion);
                    }
                });
        }
    }
}
