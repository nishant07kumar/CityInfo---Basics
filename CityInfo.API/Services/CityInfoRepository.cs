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
            return await _cityInfoContext.Cities.AnyAsync(c => c.Id == cityid);
        }

        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _cityInfoContext.Cities.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePointofInterest)
        {
            if (includePointofInterest)
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
                .Where(c => c.CityId == cityId && c.Id == pointOfInterestId).FirstOrDefaultAsync();
        }


        public async Task<IEnumerable<PointOfInterest>> GetPointOfIntresetsOfCityAsync(int cityId)
        {
            return await _cityInfoContext.PointsofInterest
                .Where(c => c.CityId == cityId).ToListAsync();
        }


        public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId, false);
            if (city != null)
            {
                city.PointsOfInterest.Add(pointOfInterest);
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _cityInfoContext.SaveChangesAsync() >= 0);
        }

        public void DeletePointOfIntrest(PointOfInterest pointOfInterest)
        {
            _cityInfoContext.PointsofInterest.Remove(pointOfInterest);
        }

        public async Task<IEnumerable<City>> GetCitiesAsync(string? name, string? searchQuery)
        {
            if (string.IsNullOrEmpty(name) && string.IsNullOrWhiteSpace(searchQuery))
            {
                return await GetCitiesAsync();
            }

            var collection = _cityInfoContext.Cities as IQueryable<City>;
            if (!string.IsNullOrEmpty(name))
            {
                name = name.Trim();
                collection = collection.Where(c => c.Name == name);
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(c => c.Name.Contains(searchQuery) || (c.Description != null) && c.Description.Contains(searchQuery));
            }

            return await collection.OrderBy(c => c.Name).ToListAsync();
        }
    }
}
