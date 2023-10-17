using FarmRecordManagementSystem.Models;
using FarmRecordManagementSystem.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Npgsql;

namespace FarmRecordManagementSystem.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private IConfiguration _config;
        public AdminRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task<List<Farms>> GetAllFarms()
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            var farms = new List<Farms>();
            string commandText = $"SELECT * FROM public.\"Farms\"";
            using (NpgsqlCommand command = new NpgsqlCommand(commandText, connection))
            {
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var farm = new Farms
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"],
                            Size = (int)reader["Size"],
                            DateCreated = (DateTime)reader["DateCreated"],
                            CreatedBy = (int)reader["CreatedBy"]
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("SoilType")))
                        {
                            farm.SoilType = (string)reader["SoilType"];
                        }

                        farms.Add(farm);
                    }
                }
            }
            if (farms.Count == 0)
                return null;
            return farms;
        }

        public async Task<List<AppUsers>> GetAllUsers()
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            var users = new List<AppUsers>();
            string commandText = $"SELECT * FROM public.\"AppUsers\"";
            using (NpgsqlCommand command = new NpgsqlCommand(commandText, connection))
            {
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var user = new AppUsers
                        {
                            Id = (int)reader["Id"],
                            UserName = (string)reader["UserName"],
                            Role = (string)reader["Role"],
                            Email = (string)reader["Email"],
                            AccountStatus = (string)reader["AccountStatus"]
                        };
                        users.Add(user);
                    }
                }
            }
            if (users.Count == 0)
                return null;
            return users;
        }
    }
}
