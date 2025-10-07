using CityInfo.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/{cityId}/pointsofinterest")]
    [ApiController]
    public class PointsOfIntrestController : ControllerBase
    {

        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfIntrest(int cityId)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            return Ok(city.PointsOfInterest);
        }

        [HttpGet("{pointofinterestid}", Name = "GetPointOfIntrest")]
        public ActionResult<PointOfInterestDto> GetPointOfIntrest(int cityId, int pointofinterestid)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointofinterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointofinterestid);
            if (pointofinterest == null)
            {
                return NotFound();
            }
            return Ok(pointofinterest);
        }


        [HttpPost]
        public ActionResult<PointOfInterestDto> CreatePointOfInterest(int cityId,
            PointOfInterestForCreationDto pointOfInterest)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }

            var maxPointOfIntrestId = CitiesDataStore.Current.Cities.SelectMany(
                c => c.PointsOfInterest).Max(p => p.Id);

            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++maxPointOfIntrestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            city.PointsOfInterest.Add(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfIntrest",
                new
                {
                    cityId = cityId,
                    pointofinterestid = finalPointOfInterest.Id
                }, finalPointOfInterest
                );

        }


        [HttpPut("{pointofinterestid}")]
        public ActionResult UpdatePointOfInterest(int cityId, int pointofinterestid,
            PointOfInterestForUpdateDto pointOfInterest)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            var pointofinterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointofinterestid);
            if (pointofinterestFromStore == null)
            {
                return NotFound();
            }

            pointofinterestFromStore.Name = pointOfInterest.Name;
            pointofinterestFromStore.Description = pointOfInterest.Description;

            return NoContent();
        }


        [HttpPatch("{pointofinterestid}")]
        public ActionResult PartiallyUpdatePointOfIntrest(int cityId, int pointofinterestid, JsonPatchDocument<PointOfInterestForUpdateDto> patchDocument)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            var pointofinterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointofinterestid);
            if (pointofinterestFromStore == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
            {
                Name = pointofinterestFromStore.Name,
                Description = pointofinterestFromStore.Description
            };


            patchDocument.ApplyTo(pointOfInterestToPatch,ModelState);

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            pointofinterestFromStore.Name = pointOfInterestToPatch.Name;
            pointofinterestFromStore.Description = pointOfInterestToPatch.Description;

            return NoContent();
        }


        [HttpDelete("{pointofinterestid}")]
        public ActionResult DeletePointOfInterest(int cityId, int pointofinterestid)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (city == null)
            {
                return NotFound();
            }
            var pointofinterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == pointofinterestid);
            if (pointofinterestFromStore == null)
            {
                return NotFound();
            }

            city.PointsOfInterest.Remove(pointofinterestFromStore);
            return NoContent();
        }
    }
}
