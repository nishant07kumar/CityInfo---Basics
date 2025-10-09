using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Entities;

namespace CityInfo.API.Profiles
{
    public class CityProfile : Profile
    {

        public CityProfile()
        {
            CreateMap<City, CityWithoutPointsofInterestDto>();
        }
    }
}
