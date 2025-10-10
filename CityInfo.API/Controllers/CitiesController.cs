using AutoMapper;
using CityInfo.API.Migrations;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {

        private readonly ICityInfoRepository cityInfoRepository;
        private readonly IMapper _mapper;

        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            this.cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsofInterestDto>>> GetCities(string? name, string? searchQuery)
        {
            var citiesEntity = await cityInfoRepository.GetCitiesAsync(name, searchQuery);
            return Ok(_mapper.Map<IEnumerable<CityWithoutPointsofInterestDto>>(citiesEntity));
        }


        [HttpGet("{id}")]
        public async Task<ActionResult> GetCity(int id, bool includePointsOfInterest = false)
        {
            var cityEntity = await cityInfoRepository.GetCityAsync(id, includePointsOfInterest);
            if (cityEntity == null)
            {
                return NotFound();
            }

            if (includePointsOfInterest)
            {
                return Ok(_mapper.Map<CityDto>(cityEntity));
            }
            return Ok(_mapper.Map<CityWithoutPointsofInterestDto>(cityEntity));

        }
    }
}
