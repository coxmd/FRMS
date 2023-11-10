using FarmRecordManagementSystem.Models;
using FarmRecordManagementSystem.Services;
using BC = BCrypt.Net.BCrypt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Npgsql;

namespace FarmRecordManagementSystem.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private IConfiguration _config;
        public ProfileRepository(IConfiguration config)
        {
            _config = config;
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
                            Password = (string)reader["Password"]
                        };
                    }
                }
            }
            return user;
        }

        public async Task UpdateUser(AppUsers user, int id)
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
                parameters.Add(new NpgsqlParameter("@Password", BC.HashPassword(user.Password)));
            }

            // Remove the trailing comma
            query = query.TrimEnd(' ', ',');

            query += " WHERE \"Id\" = @UserId";

            // Include the UserId to specify which user to update
            parameters.Add(new NpgsqlParameter("@UserId", id));

            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddRange(parameters.ToArray());

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
