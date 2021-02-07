using AutoMapper;
using Voicipher.Domain.InputModels.MetaData;
using Voicipher.Domain.Models;
using Voicipher.Domain.Payloads;

namespace Voicipher.Business.Profiles
{
    public class GenerateTokenMappingProfile : Profile
    {
        public GenerateTokenMappingProfile()
        {
            CreateMap<Administrator, GenerateTokenPayload>()
                .ForMember(
                    g => g.UserId,
                    opt => opt.Ignore())
                .ForMember(
                    g => g.Username,
                    opt => opt.MapFrom(a => a.Username))
                .ForMember(
                    g => g.Password,
                    opt => opt.Ignore())
                .ForMember(
                    g => g.PasswordHash,
                    opt => opt.MapFrom(a => a.PasswordHash))
                .ForMember(
                    g => g.PasswordSalt,
                    opt => opt.MapFrom(a => a.PasswordSalt));

            CreateMap<CreateTokenInputModel, GenerateTokenPayload>()
                .ForMember(
                    g => g.UserId,
                    opt => opt.MapFrom(a => a.UserId))
                .ForMember(
                    g => g.Username,
                    opt => opt.Ignore())
                .ForMember(
                    g => g.Password,
                    opt => opt.MapFrom(a => a.Password))
                .ForMember(
                    g => g.PasswordHash,
                    opt => opt.Ignore())
                .ForMember(
                    g => g.PasswordSalt,
                    opt => opt.Ignore());
        }
    }
}
