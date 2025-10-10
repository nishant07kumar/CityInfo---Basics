using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {

        Task<IEnumerable<City>> GetCitiesAsync();

        Task<IEnumerable<City>> GetCitiesAsync(string? name, string? searchQuery);

        Task<City?> GetCityAsync(int cityId, bool includePointofInterest);

        Task<IEnumerable<PointOfInterest>> GetPointOfIntresetsOfCityAsync(int cityId);

        Task<PointOfInterest?> GetPointOfInterestAsyc(int cityId, int pointOfInterestId);

        Task<bool> CityExistsAsync(int cityid);

        Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest);

        Task<bool> SaveChangesAsync();

        void DeletePointOfIntrest(PointOfInterest pointOfInterest);



    }
}
