using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {

        Task<IEnumerable<City>> GetCitiesAsync();

        Task<(IEnumerable<City>, PaginationMetaData)> GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize);

        Task<City?> GetCityAsync(int cityId, bool includePointofInterest);

        Task<IEnumerable<PointOfInterest>> GetPointOfIntresetsOfCityAsync(int cityId);

        Task<PointOfInterest?> GetPointOfInterestAsyc(int cityId, int pointOfInterestId);

        Task<bool> CityExistsAsync(int cityid);

        Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest);

        Task<bool> SaveChangesAsync();

        void DeletePointOfIntrest(PointOfInterest pointOfInterest);


        Task<bool> CityNameMatchesCityId(string? cityName, int cityId);



    }
}
