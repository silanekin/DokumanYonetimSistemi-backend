using AutoMapper;
using DokumanYonetimSistemi.DataModels;
using DokumanYonetimSistemi.DomainModels;
using DataModels = DokumanYonetimSistemi.DataModels;
using DomainModels = DokumanYonetimSistemi.DomainModels;

namespace DokumanYonetimSistemi.Profiles
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<DataModels.User, DomainModels.User>().ReverseMap();
        }
    }
}
