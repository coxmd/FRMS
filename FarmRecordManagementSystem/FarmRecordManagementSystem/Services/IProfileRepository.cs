using FarmRecordManagementSystem.Models;
using Npgsql;

namespace FarmRecordManagementSystem.Services
{
    public interface IProfileRepository
    {
        Task<AppUsers> GetUser(int id);
        Task UpdateUser(AppUsers user, int id);
    }
}