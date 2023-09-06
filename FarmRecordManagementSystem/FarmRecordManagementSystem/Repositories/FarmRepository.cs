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

            // Calculate the expected harvest quantity based on farm size and total quantity planted
            decimal expectedHarvestQuantity = crop.FarmSizePlanted;
            if (crop.Name == "Maize" || crop.Name == "Wheat")
            {
                expectedHarvestQuantity = crop.FarmSizePlanted * 4000;
            }
            else if (crop.Name == "Beans")
            {
                expectedHarvestQuantity = crop.FarmSizePlanted * 1000;
            }
            else if (crop.Name == "Tea")
            {
                expectedHarvestQuantity = crop.FarmSizePlanted * 4000;
            }
            else if (crop.Name == "Kale")
            {
                expectedHarvestQuantity = crop.FarmSizePlanted * 30000;
            }
            else if (crop.Name == "Cabbage")
            {
                expectedHarvestQuantity = crop.FarmSizePlanted * 20000;
            }
            else if (crop.Name == "Rice")
            {
                expectedHarvestQuantity = crop.FarmSizePlanted * 14000;
            }

            else if (crop.Name == "Potatoes" && crop.Variety == "Arizona")
            {
                expectedHarvestQuantity = crop.FarmSizePlanted * 16000;
            }
            else if (crop.Name == "Potatoes")
            {
                expectedHarvestQuantity = crop.FarmSizePlanted * 50000;
            }

            else if (crop.Name == "Potatoes" && crop.Variety == "Shangi")
            {
                expectedHarvestQuantity = crop.FarmSizePlanted * 30000;
            }
            // decimal expectedHarvestQuantity = crop.FarmSizePlanted * 4000;

            // Calculate the number of 50kg bags
            decimal numberOfBags = expectedHarvestQuantity / 50;

            DateTime expectedHarvestDate = crop.PlantingDate;
            if (crop.Name == "Maize" && crop.Variety == "Highlands")
            {
                expectedHarvestDate = crop.PlantingDate.AddMonths(6);
            }
            else if (crop.Name == "Maize" || crop.Name == "Cabbage" || crop.Name == "Rice" || crop.Name == "Potatoes")
            {
                expectedHarvestDate = crop.PlantingDate.AddMonths(3);
            }
            else if (crop.Name == "Beans" || crop.Name == "Kale")
            {
                expectedHarvestDate = crop.PlantingDate.AddMonths(2);
            }
            else if (crop.Name == "Beans" && crop.Variety == "Mwezi Moja")
            {
                expectedHarvestDate = crop.PlantingDate.AddMonths(3);
            }
            else if (crop.Name == "Wheat")
            {
                expectedHarvestDate = crop.PlantingDate.AddMonths(4);
            }
            else if (crop.Name == "Tea")
            {
                expectedHarvestDate = crop.PlantingDate.AddYears(3);
            }
            else if (crop.Name == "Potatoes" && crop.Variety == "Markies")
            {
                expectedHarvestDate = crop.PlantingDate.AddMonths(4);
            }

            string query = "INSERT INTO public.\"Crops\" (\"Name\", \"Variety\", \"FarmId\", \"PlantingDate\", \"ExpectedHarvestDate\", \"FarmSizePlanted\", \"QuantityPlanted\", \"ExpectedHarvestQuantity\", \"ExpectedBagsHarvested\")" +
                        "VALUES(@Name, @Variety, @FarmId, @PlantingDate, @ExpectedHarvestDate, @FarmSizePlanted, @QuantityPlanted, @ExpectedHarvestQuantity, @ExpectedBagsHarvested)";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", crop.Name);
                command.Parameters.AddWithValue("@Variety", string.IsNullOrEmpty(crop.Variety) ? DBNull.Value : (object)crop.Variety);
                command.Parameters.AddWithValue("@FarmId", farmId);
                command.Parameters.AddWithValue("@PlantingDate", new DateTimeOffset(crop.PlantingDate).Date);
                command.Parameters.AddWithValue("@ExpectedHarvestDate", expectedHarvestDate);
                command.Parameters.AddWithValue("@FarmSizePlanted", crop.FarmSizePlanted);
                command.Parameters.AddWithValue("@QuantityPlanted", crop.QuantityPlanted);
                command.Parameters.AddWithValue("@ExpectedHarvestQuantity", expectedHarvestQuantity);
                command.Parameters.AddWithValue("@ExpectedBagsHarvested", numberOfBags);

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
            // Fetch the total revenue (SUM of all sales) for the farm
            int revenue = await GetTotalRevenueForFarm(farmId, connection);

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

            int newTotalRevenue = revenue + sales;

            // Calculate and update the new total revenue in the "FarmRevenue" table
            string updateRevenueQuery = "UPDATE public.\"Farms\" SET \"TotalFarmRevenue\" = @TotalRevenue WHERE public.\"Farms\".\"Id\" = @FarmId";

            using (NpgsqlCommand updateCommand = new NpgsqlCommand(updateRevenueQuery, connection))
            {
                updateCommand.Parameters.AddWithValue("@FarmId", farmId);
                updateCommand.Parameters.AddWithValue("@TotalRevenue", newTotalRevenue);

                await updateCommand.ExecuteNonQueryAsync();
            }
        }

        public async Task<Inventory> GetInventoryItem(int id)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            Inventory inventoryItem = null!;
            string query = "SELECT * FROM public.\"Inventory\" WHERE public.\"Inventory\".\"Id\" =@id";
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        inventoryItem = new Inventory
                        {
                            Id = (int)reader["Id"],
                            CropName = (string)reader["CropName"],
                            QuantityHarvested = (int)reader["QuantityHarvested"],
                            TotalSold = (int)reader["TotalSold"]
                        };
                    }
                }
            }
            return inventoryItem;
        }

        // Helper method to get the total revenue for a farm from the "Inventory" table
        private async Task<int> GetTotalRevenueForFarm(int farmId, NpgsqlConnection connection)
        {
            string query = "SELECT SUM(\"Sales\") FROM public.\"Inventory\" WHERE \"FarmId\" = @FarmId";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@FarmId", farmId);
                var totalRevenue = await command.ExecuteScalarAsync();

                // Check if the result is null or DBNull (i.e., no records for the farm)
                if (totalRevenue != null && totalRevenue != DBNull.Value)
                {
                    return Convert.ToInt32(totalRevenue);
                }
            }

            return 0; // Return 0 if no revenue found for the farm
        }

        public async Task UpdateInventoryItem(Inventory inventory)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            string query = "UPDATE public.\"Inventory\" SET \"CropName\" = @name, \"QuantityHarvested\" = @harvested, \"TotalSold\" = @TotalSold WHERE public.\"Inventory\".\"Id\" = @id";

            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@name", inventory.CropName);
                command.Parameters.AddWithValue("@harvested", inventory.QuantityHarvested);
                command.Parameters.AddWithValue("@TotalSold", inventory.TotalSold);
                command.Parameters.AddWithValue("@Id", inventory.Id);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task AddTasks(Tasks task, int farmId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            string query = "INSERT INTO public.\"Tasks\" (\"Description\", \"DueDate\", \"DateStarted\", \"AssignedTo\", \"Status\", \"Completed\", \"FarmId\")" +
                        "VALUES(@Description, @DueDate, @DateStarted, @AssignedTo, @Status, @Completed, @FarmId)";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Description", task.Description);
                command.Parameters.AddWithValue("@DueDate", task.DueDate);
                command.Parameters.AddWithValue("@DateStarted", DateTime.UtcNow);
                command.Parameters.AddWithValue("@AssignedTo", string.IsNullOrEmpty(task.AssignedTo) ? DBNull.Value : (object)task.AssignedTo);
                command.Parameters.AddWithValue("@Status", "Pending");
                command.Parameters.AddWithValue("@Completed", false);
                command.Parameters.AddWithValue("@FarmId", farmId);
                // command.Parameters.AddWithValue("@CropId", string.IsNullOrEmpty(task.CropId) ? DBNull.Value : (object)task.CropId);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task CreateFarm(Farms farm, int farmerId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            decimal totalFarmSize = farm.Size ?? 0;
            int numberOfPartitions = farm.NumberOfPartitions ?? 1;
            decimal partitionSize = totalFarmSize / numberOfPartitions;


            string query = "INSERT INTO public.\"Farms\" (\"Name\", \"Size\", \"HasPartitions\", \"NumberOfPartitions\", \"PartitionSizes\", \"FarmerId\")" +
                    "VALUES(@Name, @Size, @HasPartitions, @NumberOfPartitions, @PartitionSizes, @farmerId)";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", farm.Name);
                command.Parameters.AddWithValue("@Size", totalFarmSize);
                command.Parameters.AddWithValue("@HasPartitions", farm.HasPartitions);
                command.Parameters.AddWithValue("@NumberOfPartitions", numberOfPartitions);
                command.Parameters.AddWithValue("@PartitionSizes", partitionSize);
                command.Parameters.AddWithValue("@farmerId", farmerId);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateCropDetails(Crops crop)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            string query = "UPDATE public.\"Crops\" SET \"Name\" = @name, \"Variety\" = @variety, \"PlantingDate\" = @plantingdate, \"ExpectedHarvestDate\" = @harvestdate WHERE public.\"Crops\".\"Id\" = @id";

            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@name", crop.Name);
                command.Parameters.AddWithValue("@variety", crop.Variety);
                command.Parameters.AddWithValue("@plantingdate", crop.PlantingDate);
                command.Parameters.AddWithValue("@harvestdate", crop.ExpectedHarvestDate);
                command.Parameters.AddWithValue("@Id", crop.Id);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteExpense(int Id)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            string query = "DELETE FROM public.\"Expenses\" WHERE public.\"Expenses\".\"Id\" = @id";
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", Id);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteCrop(int Id)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            string query = "DELETE FROM public.\"Crops\" WHERE public.\"Crops\".\"Id\" = @id";
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", Id);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteTask(int Id)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            string query = "DELETE FROM public.\"Tasks\" WHERE public.\"Tasks\".\"Id\" = @id";
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", Id);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task DeleteInventoryItem(int Id)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            string query = "DELETE FROM public.\"Inventory\" WHERE public.\"Inventory\".\"Id\" = @id";
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", Id);
                await command.ExecuteNonQueryAsync();
            }
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
                            FarmId = (int)reader["FarmId"],
                            Description = (string)reader["Description"],
                            AssignedTo = (string)reader["AssignedTo"],
                            DueDate = (DateTime)reader["DueDate"],
                            Status = (string)reader["Status"]
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("Completed")))
                        {
                            task.Completed = (bool)reader["Completed"];
                        }

                        tasks.Add(task);
                        // if(servicePoint is ServicePoint)
                    }
                }
            }
            if (tasks.Count == 0)
                return null;
            return tasks;
        }

        public async Task MarkAsFinished(int Id)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            Tasks task = null!;
            string query = "SELECT * FROM public.\"Tasks\" WHERE public.\"Tasks\".\"Id\" = @Id";
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", Id);
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        task = new Tasks
                        {
                            Id = (int)reader["Id"],
                            Status = (string)reader["Status"],
                            Completed = (bool)reader["Completed"]
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("DateCompleted")))
                        {
                            task.DateCompleted = (DateTime)reader["DateCompleted"];
                        }
                    }
                }
            }

            string updateQuery = "UPDATE public.\"Tasks\" SET \"Status\" = 'Finished', \"Completed\" = @completed, \"DateCompleted\" = @datecompleted  WHERE public.\"Tasks\".\"Id\" = @Id";
            using (NpgsqlCommand command = new NpgsqlCommand(updateQuery, connection))
            {
                command.Parameters.AddWithValue("@completed", true);
                command.Parameters.AddWithValue("@datecompleted", DateTime.UtcNow);
                command.Parameters.AddWithValue("@Id", Id);
                await command.ExecuteNonQueryAsync();
            }
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

        public async Task<Crops?> GetCropById(int id)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            Crops crop = null!;
            string query = "SELECT * FROM public.\"Crops\" WHERE public.\"Crops\".\"Id\" =@id";
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        crop = new Crops
                        {
                            Id = (int)reader["Id"],
                            FarmId = (int)reader["FarmId"],
                            Name = (string)reader["Name"],
                            Variety = (string)reader["Variety"],
                            PlantingDate = (DateTime)reader["PlantingDate"],
                            ExpectedHarvestDate = (DateTime)reader["ExpectedHarvestDate"]
                        };
                    }
                }
            }
            return crop;
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
                            FarmId = (int)reader["FarmId"],
                            Name = (string)reader["Name"],
                            PlantingDate = (DateTime)reader["PlantingDate"],
                            ExpectedHarvestDate = (DateTime)reader["ExpectedHarvestDate"],
                            QuantityPlanted = (decimal)reader["QuantityPlanted"],
                            ExpectedHarvestedQuantity = (decimal)reader["ExpectedHarvestQuantity"],
                            ExpectedBagsHarvested = (int)reader["ExpectedBagsHarvested"],
                            FarmSizePlanted = (decimal)reader["FarmSizePlanted"]
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
