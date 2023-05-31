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

        public async Task AddCrops(Crops crop, int farmId)
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

        public Task CreateFarm(Land farm)
        {
            throw new NotImplementedException();
        }

        public Task<List<Crops>> GetAllCrops()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Land>> GetAllFarms()
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            var farms = new List<Land>();
            string commandText = $"SELECT * FROM public.\"Land\"";
            using (NpgsqlCommand command = new NpgsqlCommand(commandText, connection))
            {
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var farm = new Land
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"],
                            Size = (int)reader["Size"]
                        };

                        farms.Add(farm);
                        // if(servicePoint is ServicePoint)
                    }
                }
            }
            if (farms.Count == 0)
                return null;
            return farms;
        }

        public async Task<Land> GetFarmDetails(int farmId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            Land farm = null!;
            string query = "SELECT * FROM public.\"Land\" WHERE public.\"Land\".\"Id\" = @farmId";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@farmId", farmId);
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        farm = new Land
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"],
                            Size = (int)reader["Size"]
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("SoilType")))
                        {
                            farm.SoilType = (string)reader["SoilType"];
                        }
                    }
                }
            }
            return farm;
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
