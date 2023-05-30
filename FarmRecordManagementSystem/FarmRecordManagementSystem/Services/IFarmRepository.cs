using FarmRecordManagementSystem.Models;
using Npgsql;

namespace FarmRecordManagementSystem.Services
{
    public interface IFarmRepository
    {
        // Service Points
        Task<List<Land>> GetAllFarms();
        Task<List<Crops>> GetAllCrops();
        // Task<ServicePoint> GetServicePointById(int id);
        System.Threading.Tasks.Task CreateFarm(Land farm);
        System.Threading.Tasks.Task AddCrops(int farmId);
        System.Threading.Tasks.Task UpdateFarmDetails(Land farm);
        System.Threading.Tasks.Task UpdateCropDetails(int cropId);
    }
}