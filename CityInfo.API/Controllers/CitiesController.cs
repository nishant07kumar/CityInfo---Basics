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
        public async Task<ActionResult<IEnumerable<CityWithoutPointsofInterestDto>>> GetCities()
        {
            var citiesEntity = await cityInfoRepository.GetCitiesAsync();
            return Ok(_mapper.Map<IEnumerable<CityWithoutPointsofInterestDto>>(citiesEntity));
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<CityWithoutPointsofInterestDto>> GetCity(int id)
        {
            var cityEntity = await cityInfoRepository.GetCityAsync(id, false);           

            return Ok(_mapper.Map<CityWithoutPointsofInterestDto>(cityEntity));

        }
    }
}
