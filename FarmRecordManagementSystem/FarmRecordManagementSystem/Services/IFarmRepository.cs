using FarmRecordManagementSystem.Models;
using Npgsql;

namespace FarmRecordManagementSystem.Services
{
    public interface IFarmRepository
    {
        // Service Points
        Task<List<Land>> GetAllFarms();

        Task<Land> GetFarmDetails(int farmId);
        Task<List<Crops>> GetAllCrops();
        // Task<ServicePoint> GetServicePointById(int id);
        Task CreateFarm(Land farm);
        Task AddCrops(Crops crop, int farmId);
        Task UpdateFarmDetails(Land farm);
        Task UpdateCropDetails(int cropId);
    }
}