using FarmRecordManagementSystem.Models;
using Npgsql;

namespace FarmRecordManagementSystem.Services
{
    public interface IFarmRepository
    {
        Task<List<Farms>> GetAllFarms(int? Id);
        Task<Farms> GetFarmDetails(int farmId);
        Task<List<Inventory>> GetFarmInventory(int farmId);
        Task<Inventory> GetInventoryItem(int id);

        Task<Expenses> GetExpense(int id);
        Task<Tasks> GetTask(int id);
        Task UpdateInventoryItem(Inventory inventory, int farmId);
        Task UpdateExpense(Expenses expense);
        Task UpdateTask(Tasks tasks);

        Task CreateFarm(Farms farm, List<FarmPartitions> partitions, int farmerId, int userId);

        Task<List<Crops>> ViewAllCrops(int farmId);
        Task<Crops?> GetCropById(int Id);
        Task<List<Tasks>> GetAllTasks(int farmId);
        Task<List<CropTypes>> GetCropTypes();
        Task<List<CropVariety>> GetVarietiesForCrop(int CropId);
        Task<List<FarmPartitions>> GetAllPartitions(int farmId);
        Task<bool> CheckPartitions(int farmId);

        Task MarkAsFinished(int Id);
        Task MarkAsHarvested(int Id);
        Task<List<Expenses>> ViewAllExpenses(int farmId);
        Task<List<StorageLocation>> ViewAllStorageLocations(int farmId);
        Task AddCrops(CropsFarmViewModel crop, int farmId, int userId);
        Task AddExpenses(Expenses expense, int farmId, int userId);
        Task AddStorageLocation(StorageLocation location, int farmId, int userId);
        Task AddPartition(FarmPartitions partition, int farmId, int userId);
        Task AddInventoryItem(Inventory inventory, int farmId, int userId);
        Task AddTasks(Tasks task, int farmId, int userId);
        Task UpdateFarmDetails(Farms farm);
        Task UpdateCropDetails(Crops crop);

        Task DeleteExpense(int Id);
        Task DeleteCrop(int Id);
        Task DeleteTask(int Id);
        Task DeleteInventoryItem(int id);
    }
}