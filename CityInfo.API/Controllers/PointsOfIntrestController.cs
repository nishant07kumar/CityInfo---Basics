using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [Authorize("MustBeFromParis")]
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
            var cityName = User.Claims.FirstOrDefault(c => c.Type == "city")?.Value;

           if(!await _cityInfoRepository.CityNameMatchesCityId(cityName, cityId))
            {
                return Forbid();
            }

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


        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>> CreatePointOfInterest(int cityId,
            PointOfInterestForCreationDto pointOfInterest)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} does not exist.");
                return NotFound();
            }
                        
            var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, finalPointOfInterest);
            await _cityInfoRepository.SaveChangesAsync();

            var createdPointOfInterestToReturn = _mapper.Map<PointOfInterestDto>(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfIntrest",
                new
                {
                    cityId = cityId,
                    pointofinterestid = createdPointOfInterestToReturn.Id
                }, createdPointOfInterestToReturn
                );

        }


        [HttpPut("{pointofinterestid}")]
        public async Task<ActionResult> UpdatePointOfInterest(int cityId, int pointofinterestid,
            PointOfInterestForUpdateDto pointOfInterest)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} does not exist.");
                return NotFound();
            }
            var pointofinterestEntity = await _cityInfoRepository.GetPointOfInterestAsyc(cityId, pointofinterestid);
            if (pointofinterestEntity == null)
            {
                return NotFound();
            }
            _mapper.Map(pointOfInterest, pointofinterestEntity);

            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }


        [HttpPatch("{pointofinterestid}")]
        public async Task<ActionResult> PartiallyUpdatePointOfIntrest(int cityId, int pointofinterestid,
                JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} does not exist.");
                return NotFound();
            }
            var pointofinterestEntity = await _cityInfoRepository.GetPointOfInterestAsyc(cityId, pointofinterestid);
            if (pointofinterestEntity == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdateDto>(pointofinterestEntity);
            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(pointOfInterestToPatch, pointofinterestEntity);
            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("{pointofinterestid}")]
        public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointofinterestid)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation($"City with id {cityId} does not exist.");
                return NotFound();
            }
            var pointofinterestEntity = await _cityInfoRepository.GetPointOfInterestAsyc(cityId, pointofinterestid);
            if (pointofinterestEntity == null)
            {
                return NotFound();
            }

            _cityInfoRepository.DeletePointOfIntrest(pointofinterestEntity);
            await _cityInfoRepository.SaveChangesAsync();

            _mailService.Send("Point of interest deleted.",
                $"Point of interest {pointofinterestEntity.Name} with id {pointofinterestEntity.Id} was deleted.");
            return NoContent();
        }
    }
}
