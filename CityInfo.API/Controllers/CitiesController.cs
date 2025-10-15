using Asp.Versioning;
using AutoMapper;
using CityInfo.API.Migrations;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;


namespace CityInfo.API.Controllers
{
    [ApiController]
    //[Authorize]
    [Route("api/v{apiverson:apiversion}/cities")]
    [ApiVersion(1)]
    [ApiVersion(2)]
    public class CitiesController : ControllerBase
    {

        private readonly ICityInfoRepository cityInfoRepository;
        private readonly IMapper _mapper;
        const int maxCitiesPageSize = 20;

        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            this.cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityWithoutPointsofInterestDto>>> GetCities(string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {

            if (pageSize > maxCitiesPageSize)
            {
                pageSize = maxCitiesPageSize;
            }

            var (citiesEntity, paginationMetaData) = await cityInfoRepository.GetCitiesAsync(name, searchQuery, pageNumber, pageSize);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetaData));

            return Ok(_mapper.Map<IEnumerable<CityWithoutPointsofInterestDto>>(citiesEntity));
        }


        /// <summary>
        /// Get a City by id with option to include points of interest
        /// </summary>
        /// <param name="id">city of id to fetch the city</param>
        /// <param name="includePointsOfInterest">optional boolean parameter for including a point of intrest</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
