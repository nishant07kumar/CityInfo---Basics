using CityInfo.API.DbContexts;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _cityInfoContext;

        public CityInfoRepository(CityInfoContext cityInfoContext)
        {
            this._cityInfoContext = cityInfoContext ?? throw new ArgumentNullException(nameof(cityInfoContext));
        }

        public async Task<bool> CityExistsAsync(int cityid)
        {
            return await _cityInfoContext.Cities.AnyAsync(c=>c.Id == cityid);
        }

        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _cityInfoContext.Cities.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePointofInterest)
        {
            if(includePointofInterest)
            {
                return await _cityInfoContext.Cities
                    .Include(c => c.PointsOfInterest)
                    .Where(c => c.Id == cityId).FirstOrDefaultAsync();
            }
            return await _cityInfoContext.Cities.Where(c => c.Id == cityId).FirstOrDefaultAsync();
        }

        public async Task<PointOfInterest?> GetPointOfInterestAsyc(int cityId, int pointOfInterestId)
        {
            return await _cityInfoContext.PointsofInterest
                .Where(c=> c.CityId == cityId && c.Id == pointOfInterestId).FirstOrDefaultAsync();
        }


        public async Task<IEnumerable<PointOfInterest>> GetPointOfIntresetsOfCityAsync(int cityId)
        {
            return await _cityInfoContext.PointsofInterest
                .Where(c => c.CityId == cityId ).ToListAsync();
        }
    }
}
