using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfIntrestController : ControllerBase
    {

        private ILogger<PointsOfIntrestController> _logger;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public IMailService _mailService { get; }

        public PointsOfIntrestController(ILogger<PointsOfIntrestController> logger,
            IMailService mailService, ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)); ;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>> GetPointsOfIntrest(int cityId)
        {

            try
            {
                if (!await _cityInfoRepository.CityExistsAsync(cityId))
                {
                    _logger.LogInformation($"City with id {cityId} does not exist.");
                    return NotFound();
                }
                var pointofInterrestOfCity = await _cityInfoRepository.GetPointOfIntresetsOfCityAsync(cityId);
                return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointofInterrestOfCity));

            }
            catch (Exception ex)
            {

                _logger.LogCritical($"Exception occured in getting city with city id = {cityId}.", ex);
                return StatusCode(500, "A problem happened while handling your request.");
            }

        }

        [HttpGet("{pointofinterestid}", Name = "GetPointOfIntrest")]
        public async Task<ActionResult<PointOfInterestDto>> GetPointOfIntrest(int cityId, int pointofinterestid)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} does not exist.");
                return NotFound();
            }
            var pointofinterest = await _cityInfoRepository.GetPointOfInterestAsyc(cityId, pointofinterestid);
            if (pointofinterest == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<PointOfInterestDto>(pointofinterest));
        }


        //[HttpPost]
        //public ActionResult<PointOfInterestDto> CreatePointOfInterest(int cityId,
        //    PointOfInterestForCreationDto pointOfInterest)
        //{
        //    var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        //    if (city == null)
        //    {
        //        return NotFound();
        //    }

        //    var maxPointOfIntrestId = _citiesDataStore.Cities.SelectMany(
        //        c => c.PointsOfInterest).Max(p => p.Id);

        //    var finalPointOfInterest = new PointOfInterestDto()
        //    {
        //        Id = ++maxPointOfIntrestId,
        //        Name = pointOfInterest.Name,
        //        Description = pointOfInterest.Description
        //    };

        //    city.PointsOfInterest.Add(finalPointOfInterest);

        //    return CreatedAtRoute("GetPointOfIntrest",
        //        new
        //        {
        //            cityId = cityId,
        //            pointofinterestid = finalPointOfInterest.Id
        //        }, finalPointOfInterest
        //        );

        //}


        //[HttpPut("{pointofinterestid}")]
        //public ActionResult UpdatePointOfInterest(int cityId, int pointofinterestid,
        //    PointOfInterestForUpdateDto pointOfInterest)
        //{
        //    var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        //    if (city == null)
        //    {
        //        return NotFound();
        //    }
        //    var pointofinterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointofinterestid);
        //    if (pointofinterestFromStore == null)
        //    {
        //        return NotFound();
        //    }

        //    pointofinterestFromStore.Name = pointOfInterest.Name;
        //    pointofinterestFromStore.Description = pointOfInterest.Description;

        //    return NoContent();
        //}


        //[HttpPatch("{pointofinterestid}")]
        //public ActionResult PartiallyUpdatePointOfIntrest(int cityId, int pointofinterestid, JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        //{
        //    var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        //    if (city == null)
        //    {
        //        return NotFound();
        //    }
        //    var pointofinterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointofinterestid);
        //    if (pointofinterestFromStore == null)
        //    {
        //        return NotFound();
        //    }

        //    var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
        //    {
        //        Name = pointofinterestFromStore.Name,
        //        Description = pointofinterestFromStore.Description
        //    };


        //    patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (!TryValidateModel(pointOfInterestToPatch))
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    pointofinterestFromStore.Name = pointOfInterestToPatch.Name;
        //    pointofinterestFromStore.Description = pointOfInterestToPatch.Description;

        //    return NoContent();
        //}


        //[HttpDelete("{pointofinterestid}")]
        //public ActionResult DeletePointOfInterest(int cityId, int pointofinterestid)
        //{
        //    var city = _citiesDataStore.Cities.FirstOrDefault(c => c.Id == cityId);
        //    if (city == null)
        //    {
        //        return NotFound();
        //    }
        //    var pointofinterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointofinterestid);
        //    if (pointofinterestFromStore == null)
        //    {
        //        return NotFound();
        //    }

        //    city.PointsOfInterest.Remove(pointofinterestFromStore);

        //    _mailService.Send("Point of interest deleted.",
        //        $"Point of interest {pointofinterestFromStore.Name} with id {pointofinterestFromStore.Id} was deleted.");
        //    return NoContent();
        //}
    }
}
