using FarmRecordManagementSystem.Models;
using Npgsql;

namespace FarmRecordManagementSystem.Services
{
    public interface IFarmRepository
    {
        // Service Points
        Task<List<Farms>> GetAllFarms(int? Id);

        Task<Farms> GetFarmDetails(int farmId);
        Task<List<Inventory>> GetFarmInventory(int farmId);
        Task<List<Crops>> GetAllCrops();

        Task CreateFarm(Farms farm, int farmerId);

        Task<List<Crops>> ViewAllCrops(int farmId);
        Task<List<Tasks>> GetAllTasks(int farmId);
        Task<List<Expenses>> ViewAllExpenses(int farmId);
        Task AddCrops(Crops crop, int farmId);
        Task AddExpenses(Expenses expense, int farmId);
        Task AddInventoryItem(Inventory inventory, int farmId);
        Task AddTasks(Tasks task, int farmId);
        Task UpdateFarmDetails(Farms farm);
        Task UpdateCropDetails(int cropId);
    }
}