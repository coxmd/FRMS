using FarmRecordManagementSystem.Models;
using Npgsql;

namespace FarmRecordManagementSystem.Services
{
    public interface IAdminRepository
    {
        Task<List<Farms>> GetAllFarms();
        Task<List<AppUsers>> GetAllUsers();
        // Task<List<Inventory>> GetAllInventoryItems();

        // Task<Farms> GetFarmDetails(int farmId);
        // // Task<List<Inventory>> GetFarmInventory(int farmId);
        // Task AddUser(AppUsers user);
        // Task UpdateUser(AppUsers user);
    }
}