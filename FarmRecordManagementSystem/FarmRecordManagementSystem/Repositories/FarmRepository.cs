using FarmRecordManagementSystem.Models;
using FarmRecordManagementSystem.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Npgsql;

namespace FarmRecordManagementSystem.Repositories
{
    public class FarmRepository : IFarmRepository
    {
        private IConfiguration _config;
        public FarmRepository(IConfiguration config)
        {
            _config = config;
        }

        public async System.Threading.Tasks.Task AddCrops(Crops crop)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            string query = "INSERT INTO public.\"Crops\" (\"Name\", \"Variety\", \"FarmId\", \"Variety\", \"PlantingDate\", \"ExpectedHarvestDate\")" +
                        "VALUES(@Name, @Variety, @FarmId, @PlantingDate, @ExpectedHarvestDate)";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", crop.Name);
                command.Parameters.AddWithValue("@Variety", crop.Variety);
                command.Parameters.AddWithValue("@FarmId", farmId);
                command.Parameters.AddWithValue("@PlantingDate", crop.PlantingDate);
                command.Parameters.AddWithValue("@ExpectedHarvestDate", crop.ExpectedHarvestDate);

                await command.ExecuteNonQueryAsync();
            }
        }

        public System.Threading.Tasks.Task CreateFarm(Land farm)
        {
            throw new NotImplementedException();
        }

        public Task<List<Crops>> GetAllCrops()
        {
            throw new NotImplementedException();
        }

        public Task<List<Land>> GetAllFarms()
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task UpdateCropDetails(int cropId)
        {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task UpdateFarmDetails(Land farm)
        {
            throw new NotImplementedException();
        }
    }
}
