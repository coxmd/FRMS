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
        Task UpdateInventoryItem(Inventory inventory);
        Task UpdateExpense(Expenses expense);
        Task UpdateTask(Tasks tasks);

        Task CreateFarm(Farms farm, List<FarmPartitions> partitions, int farmerId);

        Task<List<Crops>> ViewAllCrops(int farmId);
        Task<Crops?> GetCropById(int Id);
        Task<List<Tasks>> GetAllTasks(int farmId);

        Task MarkAsFinished(int Id);
        Task<List<Expenses>> ViewAllExpenses(int farmId);
        Task AddCrops(Crops crop, int farmId);
        Task AddExpenses(Expenses expense, int farmId);
        Task AddInventoryItem(Inventory inventory, int farmId);
        Task AddTasks(Tasks task, int farmId);
        Task UpdateFarmDetails(Farms farm);
        Task UpdateCropDetails(Crops crop);

        Task DeleteExpense(int Id);
        Task DeleteCrop(int Id);
        Task DeleteTask(int Id);
        Task DeleteInventoryItem(int id);
    }
}