using FarmRecordManagementSystem.Models;
using FarmRecordManagementSystem.Services;
using BC = BCrypt.Net.BCrypt;
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
                            Size = (int)reader["Size"]
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("SoilType")))
                        {
                            farm.SoilType = (string)reader["SoilType"];
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("CreatedAt")))
                        {
                            farm.CreatedAt = (DateTime)reader["CreatedAt"];
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("CreatedBy")))
                        {
                            farm.CreatedBy = (int)reader["CreatedBy"];
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

        public async Task AddUser(AppUsers user)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            string query = "INSERT INTO public.\"AppUsers\" (\"Email\", \"UserName\", \"Password\", \"Role\", \"AccountStatus\") VALUES (@Email, @UserName, @Password, @Role, @Status)";

            DateTime CreatedAt = DateTime.UtcNow;

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@UserName", user.UserName);
                command.Parameters.AddWithValue("@Password", BC.HashPassword(user.Password));
                command.Parameters.AddWithValue("@Role", user.Role);
                command.Parameters.AddWithValue("@Status", "Active");

                await command.ExecuteNonQueryAsync();
            }
        }
        public async Task<AppUsers> GetUser(int id)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            AppUsers user = null!;
            string query = "SELECT * FROM public.\"AppUsers\" WHERE public.\"AppUsers\".\"Id\" =@id";
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        user = new AppUsers
                        {
                            Id = (int)reader["Id"],
                            Email = (string)reader["Email"],
                            UserName = (string)reader["UserName"],
                            Password = (string)reader["Password"],
                            Role = (string)reader["Role"],
                            AccountStatus = (string)reader["AccountStatus"]
                        };
                    }
                }
            }
            return user;
        }

        public async Task UpdateUser(AppUsers user)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            // Start with the basic update query
            string query = "UPDATE public.\"AppUsers\" SET ";

            // Initialize a list to store the parameters to set
            var parameters = new List<NpgsqlParameter>();

            if (!string.IsNullOrEmpty(user.UserName))
            {
                query += "\"UserName\" = @UserName, ";
                parameters.Add(new NpgsqlParameter("@UserName", user.UserName));
            }

            if (!string.IsNullOrEmpty(user.Email))
            {
                query += "\"Email\" = @Email, ";
                parameters.Add(new NpgsqlParameter("@Email", user.Email));
            }

            if (!string.IsNullOrEmpty(user.Password))
            {
                query += "\"Password\" = @Password, ";
                parameters.Add(new NpgsqlParameter("@Password", user.Password));
            }

            if (!string.IsNullOrEmpty(user.Role))
            {
                query += "\"Role\" = @Role, ";
                parameters.Add(new NpgsqlParameter("@Role", user.Role));
            }

            if (!string.IsNullOrEmpty(user.AccountStatus))
            {
                query += "\"AccountStatus\" = @Status, ";
                parameters.Add(new NpgsqlParameter("@Status", user.AccountStatus));
            }

            // Remove the trailing comma
            query = query.TrimEnd(' ', ',');

            query += " WHERE \"Id\" = @UserId";

            // Include the UserId to specify which user to update
            parameters.Add(new NpgsqlParameter("@UserId", user.Id));

            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddRange(parameters.ToArray());

                await command.ExecuteNonQueryAsync();
            }
        }


        public async Task DeactivateUser(int Id)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            AppUsers user = null!;
            string query = "SELECT * FROM public.\"AppUsers\" WHERE public.\"AppUsers\".\"Id\" = @Id";
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", Id);
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        user = new AppUsers
                        {
                            Id = (int)reader["Id"],
                            AccountStatus = (string)reader["AccountStatus"]
                        };
                    }
                }
            }

            string updateQuery = "UPDATE public.\"AppUsers\" SET \"AccountStatus\" = 'Deactivated' WHERE public.\"AppUsers\".\"Id\" = @Id";
            using (NpgsqlCommand command = new NpgsqlCommand(updateQuery, connection))
            {
                command.Parameters.AddWithValue("@Id", Id);
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
