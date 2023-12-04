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
            using (NpgsqlCommand command = new(commandText, connection))
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

        public async Task<int> GetFarmCount()
        {
            int farmCount = 0;
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            string query = "SELECT COUNT(*) FROM public.\"Farms\"";

            using (NpgsqlCommand command = new(query, connection))
            {
                var result = await command.ExecuteScalarAsync();

                if (result != null && result != DBNull.Value)
                {
                    farmCount = Convert.ToInt32(result);
                }
            }

            return farmCount;
        }

        public async Task<int> GetUserCount()
        {
            int userCount = 0;
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            string query = "SELECT COUNT(*) FROM public.\"AppUsers\"";

            using (NpgsqlCommand command = new(query, connection))
            {
                var result = await command.ExecuteScalarAsync();

                if (result != null && result != DBNull.Value)
                {
                    userCount = Convert.ToInt32(result);
                }
            }

            return userCount;
        }

        public async Task<int> GetTotalRevenue()
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            string query = "SELECT SUM(\"TotalRevenue\") FROM public.\"Revenue\"";

            using (NpgsqlCommand command = new(query, connection))
            {
                var totalRevenue = await command.ExecuteScalarAsync();

                // Check if the result is null or DBNull (i.e., no records for the farm)
                if (totalRevenue != null && totalRevenue != DBNull.Value)
                {
                    return Convert.ToInt32(totalRevenue);
                }
            }

            return 0; // Return 0 if no revenue found for the farm
        }

        public async Task<List<AppUsers>> GetAllUsers()
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            var users = new List<AppUsers>();
            string commandText = $"SELECT * FROM public.\"AppUsers\"";
            using (NpgsqlCommand command = new(commandText, connection))
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

        public async Task<List<ExpenseCategory>> GetAllExpenseCategory()
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            var categories = new List<ExpenseCategory>();
            string query = "SELECT * FROM public.\"ExpenseCategory\"";

            using (NpgsqlCommand command = new(query, connection))
            {
                using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var category = new ExpenseCategory
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"]
                    };
                    categories.Add(category);
                    // if(servicePoint is ServicePoint)
                }
            }
            if (categories.Count == 0)
                return null;
            return categories;
        }

        public async Task<List<CropTypes>> GetAllCropTypes()
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            var types = new List<CropTypes>();
            string query = "SELECT * FROM public.\"CropTypes\"";

            using (NpgsqlCommand command = new(query, connection))
            {
                using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var type = new CropTypes
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"]
                    };
                    types.Add(type);
                    // if(servicePoint is ServicePoint)
                }
            }
            if (types.Count == 0)
                return null;
            return types;
        }

        public async Task<List<CropVariety>> GetAllCropVariety()
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            var varieties = new List<CropVariety>();
            string query = "SELECT * FROM public.\"CropVariety\"";

            using (NpgsqlCommand command = new(query, connection))
            {
                using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var variety = new CropVariety
                    {
                        Id = (int)reader["Id"],
                        Name = (string)reader["Name"]
                    };
                    varieties.Add(variety);
                    // if(servicePoint is ServicePoint)
                }
            }
            if (varieties.Count == 0)
                return null;
            return varieties;
        }

        public async Task AddUser(AppUsers user)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            string query = "INSERT INTO public.\"AppUsers\" (\"Email\", \"UserName\", \"Password\", \"Role\", \"AccountStatus\") VALUES (@Email, @UserName, @Password, @Role, @Status)";

            DateTime CreatedAt = DateTime.UtcNow;

            using (NpgsqlCommand command = new(query, connection))
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
            using (NpgsqlCommand command = new(query, connection))
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
                parameters.Add(new NpgsqlParameter("@Password", BC.HashPassword(user.Password)));
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
            using (NpgsqlCommand command = new(query, connection))
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
            using (NpgsqlCommand command = new(updateQuery, connection))
            {
                command.Parameters.AddWithValue("@Id", Id);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task AddExpenseCategory(ExpenseCategory expenseCategory)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            string query = "INSERT INTO public.\"ExpenseCategory\" (\"Name\") VALUES(@Name)";


            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", expenseCategory.Name);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task AddCropTypes(CropTypes cropTypes)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            string query = "INSERT INTO public.\"CropTypes\" (\"Name\") VALUES(@Name)";


            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", cropTypes.Name);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task AddCropVariety(CropVariety cropVariety, int CropId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            string query = "INSERT INTO public.\"CropVariety\" (\"Name\") VALUES(@Name) WHERE public.\"CropVariety\".\"CropId\" = @cropId";


            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", cropVariety.Name);
                command.Parameters.AddWithValue("@CropId", CropId);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<ExpenseCategory> GetExpenseCategory(int id)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            ExpenseCategory expenseCategory = null!;
            string query = "SELECT * FROM public.\"ExpenseCategory\" WHERE public.\"ExpenseCategory\".\"Id\" =@id";
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        expenseCategory = new ExpenseCategory
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"]
                        };
                    }
                }
            }
            return expenseCategory;
        }

        public async Task UpdateExpenseCategory(ExpenseCategory expenseCategory)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            string query = "UPDATE public.\"ExpenseCategory\" SET \"Name\" = @name WHERE public.\"ExpenseCategory\".\"Id\" = @id";

            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@name", expenseCategory.Name);
                command.Parameters.AddWithValue("@Id", expenseCategory.Id);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<CropVariety> GetCropVariety(int id)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            CropVariety cropVariety = null!;
            string query = "SELECT * FROM public.\"CropVariety\" WHERE public.\"CropVariety\".\"Id\" =@id";
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        cropVariety = new CropVariety
                        {
                            Id = (int)reader["Id"],
                            CropId = (int)reader["Id"],
                            Name = (string)reader["Name"]
                        };
                    }
                }
            }
            return cropVariety;
        }

        public async Task UpdateCropVariety(CropVariety cropVariety)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            string query = "UPDATE public.\"CropVariety\" SET \"Name\" = @name WHERE public.\"CropVariety\".\"Id\" = @id";

            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@name", cropVariety.Name);
                command.Parameters.AddWithValue("@Id", cropVariety.Id);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<CropTypes> GetCropType(int id)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            CropTypes cropTypes = null!;
            string query = "SELECT * FROM public.\"CropTypes\" WHERE public.\"CropTypes\".\"Id\" =@id";
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        cropTypes = new CropTypes
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"]
                        };
                    }
                }
            }
            return cropTypes;
        }

        public async Task UpdateCropTypes(CropTypes cropTypes)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            string query = "UPDATE public.\"CropTypes\" SET \"Name\" = @name WHERE public.\"CropTypes\".\"Id\" = @id";

            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@name", cropTypes.Name);
                command.Parameters.AddWithValue("@Id", cropTypes.Id);

                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
