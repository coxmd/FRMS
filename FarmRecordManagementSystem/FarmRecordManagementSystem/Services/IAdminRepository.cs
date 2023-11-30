using FarmRecordManagementSystem.Models;
using Npgsql;

namespace FarmRecordManagementSystem.Services
{
    public interface IAdminRepository
    {
        Task<List<Farms>> GetAllFarms();
        Task<int> GetFarmCount();
        Task<int> GetUserCount();
        Task<ExpenseCategory> GetExpenseCategory(int id);
        Task<CropVariety> GetCropVariety(int id);
        Task<CropTypes> GetCropType(int id);
        Task UpdateExpenseCategory(ExpenseCategory expenseCategory);
        Task UpdateCropVariety(CropVariety cropVariety);
        Task UpdateCropTypes(CropTypes cropTypes);
        Task<List<ExpenseCategory>> GetAllExpenseCategory();
        Task<List<CropTypes>> GetAllCropTypes();
        Task<List<CropVariety>> GetAllCropVariety();
        Task<int> GetTotalRevenue();
        Task<List<AppUsers>> GetAllUsers();
        Task AddUser(AppUsers user);
        Task AddExpenseCategory(ExpenseCategory expenseCategory);
        Task AddCropTypes(CropTypes cropTypes);
        Task AddCropVariety(CropVariety cropVariety, int CropId);
        Task<AppUsers> GetUser(int id);
        Task UpdateUser(AppUsers user);
        Task DeactivateUser(int Id);
    }
}