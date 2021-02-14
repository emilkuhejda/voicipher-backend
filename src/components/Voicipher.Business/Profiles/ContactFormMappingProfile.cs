using System;
using AutoMapper;
using Voicipher.Domain.InputModels;
using Voicipher.Domain.Models;

namespace Voicipher.Business.Profiles
{
    public class ContactFormMappingProfile : Profile
    {
        public ContactFormMappingProfile()
        {
            CreateMap<ContactFormInputModel, ContactForm>()
                .ForMember(
                    c => c.Id,
                    opt => opt.MapFrom(x => Guid.NewGuid()))
                .ForMember(
                    c => c.Name,
                    opt => opt.MapFrom(x => x.Name))
                .ForMember(
                    c => c.Email,
                    opt => opt.MapFrom(x => x.Email))
                .ForMember(
                    c => c.Message,
                    opt => opt.MapFrom(x => x.Message))
                .ForMember(
                    c => c.DateCreatedUtc,
                    opt => opt.MapFrom(x => DateTime.UtcNow));
        }
    }
}
