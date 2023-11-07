using FarmRecordManagementSystem.Models;
using Npgsql;

namespace FarmRecordManagementSystem.Services
{
    public interface IAdminRepository
    {
        Task<List<Farms>> GetAllFarms();
        Task<List<AppUsers>> GetAllUsers();
        Task AddUser(AppUsers user);
        Task<AppUsers> GetUser(int id);
        Task UpdateUser(AppUsers user);
        Task DeactivateUser(int Id);
    }
}