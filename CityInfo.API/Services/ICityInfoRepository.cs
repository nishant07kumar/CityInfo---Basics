using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {

        Task<IEnumerable<City>> GetCitiesAsync();

        Task<City?> GetCityAsync(int cityId, bool includePointofInterest);

        Task<IEnumerable<PointOfInterest>> GetPointOfIntresetsOfCityAsync(int cityId);

        Task<PointOfInterest?> GetPointOfInterestAsyc(int cityId, int pointOfInterestId);

        Task<bool> CityExistsAsync(int cityid);

    }
}
