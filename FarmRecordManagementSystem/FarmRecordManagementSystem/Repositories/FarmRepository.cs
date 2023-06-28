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

            string query = "INSERT INTO public.\"Crops\" (\"Name\", \"Variety\", \"FarmId\", \"PlantingDate\", \"ExpectedHarvestDate\")" +
                        "VALUES(@Name, @Variety, @FarmId, @PlantingDate, @ExpectedHarvestDate)";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", crop.Name);
                command.Parameters.AddWithValue("@Variety", string.IsNullOrEmpty(crop.Variety) ? DBNull.Value : (object)crop.Variety);
                command.Parameters.AddWithValue("@FarmId", farmId);
                command.Parameters.AddWithValue("@PlantingDate", crop.PlantingDate);
                command.Parameters.AddWithValue("@ExpectedHarvestDate", crop.ExpectedHarvestDate);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task AddExpenses(Expenses expense, int farmId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            string query = "INSERT INTO public.\"Expenses\" (\"DateCreated\", \"Name\", \"Price\", \"Category\", \"FarmId\")" +
                        "VALUES(@DateCreated, @Name, @Price, @Category, @FarmId)";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@DateCreated", DateTime.UtcNow);
                command.Parameters.AddWithValue("@Name", expense.Name);
                command.Parameters.AddWithValue("@Price", expense.Price);
                command.Parameters.AddWithValue("@Category", string.IsNullOrEmpty(expense.Category) ? DBNull.Value : (object)expense.Category);
                command.Parameters.AddWithValue("@FarmId", farmId);
                // command.Parameters.AddWithValue("@CropId", string.IsNullOrEmpty(expense.CropId) ? DBNull.Value : (object)expense.CropId);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task AddInventoryItem(Inventory inventory, int farmId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            string query = "INSERT INTO public.\"Inventory\" (\"DateCreated\", \"CropName\", \"QuantityHarvested\", \"PriceSold\", \"FarmId\", \"QuantityRemaining\", \"TotalSold\", \"Sales\")" +
                        "VALUES(@DateCreated, @Name, @QuantityHarvested, @PriceSold, @FarmId, @QuantityRemaining, @TotalSold, @Sales)";

            int quantityHarvested = inventory.QuantityHarvested ?? 0;
            int totalSold = inventory.TotalSold ?? 0;
            int priceSold = inventory.PriceSold ?? 0;
            int quantityRemaining = quantityHarvested - totalSold;
            int sales = totalSold * priceSold;

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@DateCreated", DateTime.UtcNow);
                command.Parameters.AddWithValue("@Name", inventory.CropName);
                command.Parameters.AddWithValue("@QuantityHarvested", quantityHarvested);
                command.Parameters.AddWithValue("@PriceSold", priceSold);
                command.Parameters.AddWithValue("@TotalSold", totalSold);
                command.Parameters.AddWithValue("@QuantityRemaining", quantityRemaining);
                command.Parameters.AddWithValue("@Sales", sales);
                command.Parameters.AddWithValue("@FarmId", farmId);
                // command.Parameters.AddWithValue("@CropId", string.IsNullOrEmpty(expense.CropId) ? DBNull.Value : (object)expense.CropId);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task AddTasks(Tasks task, int farmId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            string query = "INSERT INTO public.\"Tasks\" (\"Description\", \"DueDate\", \"AssignedTo\", \"Status\", \"FarmId\")" +
                        "VALUES(@Description, @DueDate, @AssignedTo, @Status, @FarmId)";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Description", task.Description);
                command.Parameters.AddWithValue("@DueDate", task.DueDate);
                command.Parameters.AddWithValue("@AssignedTo", string.IsNullOrEmpty(task.AssignedTo) ? DBNull.Value : (object)task.AssignedTo);
                command.Parameters.AddWithValue("@Status", task.Status);
                command.Parameters.AddWithValue("@FarmId", farmId);
                // command.Parameters.AddWithValue("@CropId", string.IsNullOrEmpty(task.CropId) ? DBNull.Value : (object)task.CropId);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task CreateFarm(Farms farm, int farmerId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            string query = "INSERT INTO public.\"Farms\" (\"Name\", \"Size\", \"FarmerId\")" +
                        "VALUES(@Name, @Size, @farmerId)";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", farm.Name);
                command.Parameters.AddWithValue("@Size", farm.Size);
                command.Parameters.AddWithValue("@farmerId", farmerId);

                await command.ExecuteNonQueryAsync();
            }
        }

        public Task<List<Crops>> GetAllCrops()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Farms>> GetAllFarms(int? Id)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            var farms = new List<Farms>();
            string commandText = $"SELECT * FROM public.\"Farms\" WHERE public.\"Farms\".\"FarmerId\" = @Id";
            using (NpgsqlCommand command = new NpgsqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@Id", Id);
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

                        farms.Add(farm);
                        // if(servicePoint is ServicePoint)
                    }
                }
            }
            if (farms.Count == 0)
                return null;
            return farms;
        }

        public async Task<List<Tasks>> GetAllTasks(int farmId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            var tasks = new List<Tasks>();
            string query = "SELECT * FROM public.\"Tasks\" WHERE public.\"Tasks\".\"FarmId\" = @farmId";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@farmId", farmId);
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var task = new Tasks
                        {
                            Id = (int)reader["Id"],
                            Description = (string)reader["Description"],
                            AssignedTo = (string)reader["AssignedTo"],
                            DueDate = (DateTime)reader["DueDate"],
                            Status = (string)reader["Status"]
                        };

                        tasks.Add(task);
                        // if(servicePoint is ServicePoint)
                    }
                }
            }
            if (tasks.Count == 0)
                return null;
            return tasks;
        }

        public async Task<Farms> GetFarmDetails(int farmId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            Farms farm = null!;
            string query = "SELECT * FROM public.\"Farms\" WHERE public.\"Farms\".\"Id\" = @farmId";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@farmId", farmId);
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        farm = new Farms
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

        public async Task<List<Inventory>> GetFarmInventory(int farmId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            var inventories = new List<Inventory>();
            string query = "SELECT * FROM public.\"Inventory\" WHERE public.\"Inventory\".\"FarmId\" = @farmId";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@farmId", farmId);
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var inventory = new Inventory
                        {
                            Id = (int)reader["Id"],
                            CropName = (string)reader["CropName"],
                            QuantityHarvested = (int)reader["QuantityHarvested"],
                            PriceSold = (int)reader["PriceSold"],
                            QuantityRemaining = (int)reader["QuantityRemaining"]
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("TotalSold")))
                        {
                            inventory.TotalSold = (int)reader["TotalSold"];
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("Sales")))
                        {
                            inventory.Sales = (int)reader["Sales"];
                        }
                        inventories.Add(inventory);
                    }
                }
            }
            return inventories;
        }

        public Task UpdateCropDetails(int cropId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateFarmDetails(Farms farm)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Crops>> ViewAllCrops(int farmId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            var crops = new List<Crops>();
            string query = "SELECT * FROM public.\"Crops\" WHERE public.\"Crops\".\"FarmId\" = @farmId";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@farmId", farmId);
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var crop = new Crops
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"],
                            PlantingDate = (DateTime)reader["PlantingDate"],
                            ExpectedHarvestDate = (DateTime)reader["ExpectedHarvestDate"]
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("Variety")))
                        {
                            crop.Variety = (string)reader["Variety"];
                        }

                        crops.Add(crop);
                        // if(servicePoint is ServicePoint)
                    }
                }
            }
            if (crops.Count == 0)
                return null;
            return crops;
        }

        public async Task<List<Expenses>> ViewAllExpenses(int farmId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            var expenses = new List<Expenses>();
            string query = "SELECT * FROM public.\"Expenses\" WHERE public.\"Expenses\".\"FarmId\" = @farmId";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@farmId", farmId);
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var expense = new Expenses
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"],
                            Price = (int)reader["Price"],
                            FarmId = (int)reader["FarmId"]
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("Category")))
                        {
                            expense.Category = (string)reader["Category"];
                        }


                        expenses.Add(expense);
                        // if(servicePoint is ServicePoint)
                    }
                }
            }
            if (expenses.Count == 0)
                return null;
            return expenses;
        }
    }
}
