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

        public async Task AddCrops(CropsFarmViewModel crop, int farmId, int userId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            int? partitionId = null;

            var hasPartitions = await CheckPartitions(farmId);

            if (hasPartitions)
            {
                partitionId = (int?)crop.PartitionPlanted; // Assuming crop.PartitionPlanted contains the selected partition ID.
            }

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

            DateTime createdAt = DateTime.UtcNow;

            string query = "INSERT INTO public.\"Crops\" (\"Name\", \"Variety\", \"FarmId\", \"PlantingDate\", \"ExpectedHarvestDate\", \"FarmSizePlanted\", \"PartitionPlanted\", \"QuantityPlanted\", \"ExpectedHarvestQuantity\", \"ExpectedBagsHarvested\", \"CreatedAt\", \"CreatedBy\")" +
                        "VALUES(@Name, @Variety, @FarmId, @PlantingDate, @ExpectedHarvestDate, @FarmSizePlanted, @PartitionPlanted, @QuantityPlanted, @ExpectedHarvestQuantity, @ExpectedBagsHarvested, @CreatedAt, @CreatedBy)";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", crop.Name);
                command.Parameters.AddWithValue("@Variety", string.IsNullOrEmpty(crop.Variety) ? DBNull.Value : (object)crop.Variety);
                command.Parameters.AddWithValue("@FarmId", farmId);
                command.Parameters.AddWithValue("@PlantingDate", new DateTimeOffset(crop.PlantingDate).Date);
                command.Parameters.AddWithValue("@ExpectedHarvestDate", expectedHarvestDate);
                command.Parameters.AddWithValue("@FarmSizePlanted", crop.FarmSizePlanted);
                command.Parameters.AddWithValue("@CreatedAt", createdAt);
                command.Parameters.AddWithValue("@CreatedBy", userId);
                if (hasPartitions)
                {
                    command.Parameters.AddWithValue("@PartitionPlanted", partitionId);
                }
                else
                {
                    command.Parameters.AddWithValue("@PartitionPlanted", DBNull.Value);
                }
                command.Parameters.AddWithValue("@QuantityPlanted", crop.QuantityPlanted);
                command.Parameters.AddWithValue("@ExpectedHarvestQuantity", expectedHarvestQuantity);
                command.Parameters.AddWithValue("@ExpectedBagsHarvested", numberOfBags);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task AddExpenses(Expenses expense, int farmId, int userId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            int? partitionId = null;

            var hasPartitions = await CheckPartitions(farmId);

            if (hasPartitions)
            {
                partitionId = (int?)expense.PartitionId; // Assuming crop.PartitionPlanted contains the selected partition ID.
            }

            DateTime CreatedAt = DateTime.UtcNow;

            string query = "INSERT INTO public.\"Expenses\" (\"Name\", \"Price\", \"Category\", \"FarmId\", \"PartitionId\", \"CropId\", \"CreatedAt\", \"CreatedBy\")" +
                        "VALUES(@Name, @Price, @Category, @FarmId, @PartitionId, @CropId, @CreatedAt, @CreatedBy)";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", expense.Name);
                command.Parameters.AddWithValue("@Price", expense.Price);
                command.Parameters.AddWithValue("@Category", string.IsNullOrEmpty(expense.Category) ? DBNull.Value : (object)expense.Category);
                command.Parameters.AddWithValue("@FarmId", farmId);
                if (hasPartitions)
                {
                    command.Parameters.AddWithValue("@PartitionId", partitionId);
                    command.Parameters.AddWithValue("@CropId", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@PartitionId", DBNull.Value);
                    command.Parameters.AddWithValue("@CropId", expense.CropId);
                }
                command.Parameters.AddWithValue("@CreatedAt", CreatedAt);
                command.Parameters.AddWithValue("@CreatedBy", userId);
                // command.Parameters.AddWithValue("@CropId", string.IsNullOrEmpty(expense.CropId) ? DBNull.Value : (object)expense.CropId);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task AddStorageLocation(StorageLocation location, int farmId, int userId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            string query = "INSERT INTO public.\"StorageLocations\" (\"Name\", \"FarmId\", \"CreatedAt\", \"CreatedBy\")" +
                        "VALUES(@Name, @FarmId, @CreatedAt, @CreatedBy)";

            DateTime CreatedAt = DateTime.UtcNow;

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", location.Name);
                command.Parameters.AddWithValue("@FarmId", farmId);
                command.Parameters.AddWithValue("@CreatedAt", CreatedAt);
                command.Parameters.AddWithValue("@CreatedBy", userId);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task AddInventoryItem(Inventory inventory, int farmId, int userId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            string query = "INSERT INTO public.\"Inventory\" (\"CropName\", \"QuantityHarvested\", \"PriceSold\", \"FarmId\", \"QuantityRemaining\", \"TotalSold\", \"Sales\", \"CreatedAt\", \"CreatedBy\")" +
                        "VALUES(@Name, @QuantityHarvested, @PriceSold, @FarmId, @QuantityRemaining, @TotalSold, @Sales, @CreatedAt, @CreatedBy)";

            int quantityHarvested = inventory.QuantityHarvested ?? 0;
            int totalSold = inventory.TotalSold ?? 0;
            int priceSold = inventory.PriceSold ?? 0;
            int quantityRemaining = quantityHarvested - totalSold;
            int sales = totalSold * priceSold;
            // Fetch the total revenue (SUM of all sales) for the farm
            int revenue = await GetTotalRevenueForFarm(farmId, connection);

            DateTime CreatedAt = DateTime.UtcNow;

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", inventory.CropName);
                command.Parameters.AddWithValue("@QuantityHarvested", quantityHarvested);
                command.Parameters.AddWithValue("@PriceSold", priceSold);
                command.Parameters.AddWithValue("@TotalSold", totalSold);
                command.Parameters.AddWithValue("@QuantityRemaining", quantityRemaining);
                command.Parameters.AddWithValue("@Sales", sales);
                command.Parameters.AddWithValue("@FarmId", farmId);
                command.Parameters.AddWithValue("@CreatedAt", CreatedAt);
                command.Parameters.AddWithValue("@CreatedBy", userId);
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
                            PriceSold = (int)reader["PriceSold"],
                            TotalSold = (int)reader["TotalSold"],
                            FarmId = (int)reader["FarmId"]
                        };
                    }
                }
            }
            return inventoryItem;
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

        public async Task<Expenses> GetExpense(int id)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            Expenses expense = null!;
            string query = "SELECT * FROM public.\"Expenses\" WHERE public.\"Expenses\".\"Id\" =@id";
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        expense = new Expenses
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

                    }
                }
            }
            return expense;
        }

        public async Task UpdateExpense(Expenses expenses)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            string query = "UPDATE public.\"Expenses\" SET \"Name\" = @name, \"Price\" = @price, \"Category\" = @category WHERE public.\"Expenses\".\"Id\" = @id";

            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@name", expenses.Name);
                command.Parameters.AddWithValue("@price", expenses.Price);
                command.Parameters.AddWithValue("@category", string.IsNullOrEmpty(expenses.Category) ? DBNull.Value : (object)expenses.Category);
                command.Parameters.AddWithValue("@Id", expenses.Id);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<Tasks> GetTask(int id)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            Tasks tasks = null!;
            string query = "SELECT * FROM public.\"Tasks\" WHERE public.\"Tasks\".\"Id\" =@id";
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        tasks = new Tasks
                        {
                            Id = (int)reader["Id"],
                            Description = (string)reader["Description"],
                            DueDate = (DateTime)reader["DueDate"],
                            AssignedTo = (string)reader["AssignedTo"],
                            FarmId = (int)reader["FarmId"]
                        };

                    }
                }
            }
            return tasks;
        }

        public async Task UpdateTask(Tasks tasks)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();
            string query = "UPDATE public.\"Tasks\" SET \"Description\" = @description, \"DueDate\" = @duedate, \"AssignedTo\" = @assignedTo WHERE public.\"Tasks\".\"Id\" = @id";

            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@description", tasks.Description);
                command.Parameters.AddWithValue("@duedate", tasks.DueDate);
                command.Parameters.AddWithValue("@assignedTo", tasks.AssignedTo);
                command.Parameters.AddWithValue("@Id", tasks.Id);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task AddTasks(Tasks task, int farmId, int userId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            string query = "INSERT INTO public.\"Tasks\" (\"Description\", \"DueDate\", \"DateStarted\", \"AssignedTo\", \"Status\", \"Completed\", \"FarmId\", \"CreatedBy\", \"CreatedAt\")" +
                        "VALUES(@Description, @DueDate, @DateStarted, @AssignedTo, @Status, @Completed, @FarmId, @CreatedBy, @CreatedAt)";

            DateTime CreatedAt = DateTime.UtcNow;

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Description", task.Description);
                command.Parameters.AddWithValue("@DueDate", task.DueDate);
                command.Parameters.AddWithValue("@DateStarted", DateTime.UtcNow);
                command.Parameters.AddWithValue("@AssignedTo", string.IsNullOrEmpty(task.AssignedTo) ? DBNull.Value : (object)task.AssignedTo);
                command.Parameters.AddWithValue("@Status", "Pending");
                command.Parameters.AddWithValue("@Completed", false);
                command.Parameters.AddWithValue("@FarmId", farmId);
                command.Parameters.AddWithValue("@CreatedBy", userId);
                command.Parameters.AddWithValue("@CreatedAt", CreatedAt);

                // command.Parameters.AddWithValue("@CropId", string.IsNullOrEmpty(task.CropId) ? DBNull.Value : (object)task.CropId);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task AddPartition(FarmPartitions partition, int farmId, int userId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            string query = "INSERT INTO public.\"FarmPartitions\" (\"Name\", \"Size\", \"FarmId\", \"CreatedAt\", \"CreatedBy\")" +
                        "VALUES(@Name, @Size, @farmId, @CreatedAt, @CreatedBy)";

            DateTime CreatedAt = DateTime.UtcNow;

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Name", partition.Name);
                command.Parameters.AddWithValue("@Size", partition.Size);
                command.Parameters.AddWithValue("@farmId", farmId);
                command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
                command.Parameters.AddWithValue("@CreatedBy", userId);

                await command.ExecuteNonQueryAsync();
            }
        }

        // public async Task CreateFarm(Farms farm, int farmerId)
        // {
        //     using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
        //     connection.Open();

        //     decimal totalFarmSize = farm.Size ?? 0;
        //     // int numberOfPartitions = farm.NumberOfPartitions ?? 1;
        //     // decimal partitionSize = totalFarmSize / numberOfPartitions;


        //     string query = "INSERT INTO public.\"Farms\" (\"Name\", \"Size\", \"HasPartitions\", \"NumberOfPartitions\", \"PartitionSizes\", \"FarmerId\")" +
        //             "VALUES(@Name, @Size, @HasPartitions, @NumberOfPartitions, @PartitionSizes, @farmerId)";

        //     using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
        //     {
        //         command.Parameters.AddWithValue("@Name", farm.Name);
        //         command.Parameters.AddWithValue("@Size", totalFarmSize);
        //         command.Parameters.AddWithValue("@HasPartitions", farm.HasPartitions);
        //         // command.Parameters.AddWithValue("@NumberOfPartitions", numberOfPartitions);
        //         // command.Parameters.AddWithValue("@PartitionSizes", partitionSize);
        //         command.Parameters.AddWithValue("@farmerId", farmerId);

        //         await command.ExecuteNonQueryAsync();
        //     }
        // }

        public async Task CreateFarm(Farms farm, List<FarmPartitions> partitions, int farmerId, int userId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            decimal totalFarmSize = farm.Size ?? 0;

            string farmQuery = "INSERT INTO public.\"Farms\" (\"Name\", \"Size\", \"HasPartitions\", \"FarmerId\", \"CreatedAt\", \"CreatedBy\")" +
                    "VALUES(@Name, @Size, @HasPartitions, @farmerId, @CreatedAt, @CreatedBy) RETURNING \"Id\"";

            DateTime CreatedAt = DateTime.UtcNow;

            using (NpgsqlCommand farmCommand = new NpgsqlCommand(farmQuery, connection))
            {
                farmCommand.Parameters.AddWithValue("@Name", farm.Name);
                farmCommand.Parameters.AddWithValue("@Size", totalFarmSize);
                farmCommand.Parameters.AddWithValue("@HasPartitions", farm.HasPartitions);
                farmCommand.Parameters.AddWithValue("@farmerId", farmerId);
                farmCommand.Parameters.AddWithValue("@CreatedAt", CreatedAt);
                farmCommand.Parameters.AddWithValue("@CreatedBy", userId);

                int farmId = (int)await farmCommand.ExecuteScalarAsync();

                if (farm.HasPartitions && partitions != null && partitions.Any())
                {
                    string partitionQuery = "INSERT INTO public.\"FarmPartitions\" (\"FarmId\", \"Name\", \"Size\", \"DateCreated\")" +
                        "VALUES(@FarmId, @Name, @Size, @DateCreated)";

                    using (NpgsqlCommand partitionCommand = new NpgsqlCommand(partitionQuery, connection))
                    {
                        foreach (var partition in partitions)
                        {
                            partitionCommand.Parameters.Clear();
                            partitionCommand.Parameters.AddWithValue("@FarmId", farmId);
                            partitionCommand.Parameters.AddWithValue("@Name", partition.Name);
                            partitionCommand.Parameters.AddWithValue("@Size", partition.Size);
                            partitionCommand.Parameters.AddWithValue("@DateCreated", DateTime.Now);

                            await partitionCommand.ExecuteNonQueryAsync();
                        }
                    }
                }
            }
        }


        public async Task UpdateCropDetails(Crops crop)
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

            string query = "UPDATE public.\"Crops\" SET \"Name\" = @name, \"Variety\" = @variety, \"PlantingDate\" = @plantingdate, \"ExpectedHarvestDate\" = @harvestdate, \"QuantityPlanted\" = @quantityplanted, \"FarmSizePlanted\" = @farmsize, \"ExpectedHarvestQuantity\" = @ExpectedHarvestQuantity, \"ExpectedBagsHarvested\" = @ExpectedBagsHarvested  WHERE public.\"Crops\".\"Id\" = @id";

            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@name", crop.Name);
                command.Parameters.AddWithValue("@variety", string.IsNullOrEmpty(crop.Variety) ? DBNull.Value : (object)crop.Variety);
                command.Parameters.AddWithValue("@plantingdate", crop.PlantingDate);
                command.Parameters.AddWithValue("@harvestdate", crop.ExpectedHarvestDate);
                command.Parameters.AddWithValue("@quantityplanted", crop.QuantityPlanted);
                command.Parameters.AddWithValue("@farmsize", crop.FarmSizePlanted);
                command.Parameters.AddWithValue("@ExpectedHarvestQuantity", expectedHarvestQuantity);
                command.Parameters.AddWithValue("@ExpectedBagsHarvested", numberOfBags);
                command.Parameters.AddWithValue("@Id", crop.Id);
                // command.Parameters.AddWithValue("@PlantingDate", new DateTimeOffset(crop.PlantingDate).Date);
                // command.Parameters.AddWithValue("@ExpectedHarvestDate", expectedHarvestDate);


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

        public async Task<List<FarmPartitions>> GetAllPartitions(int farmId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            var partitions = new List<FarmPartitions>();
            string query = "SELECT * FROM public.\"FarmPartitions\" WHERE public.\"FarmPartitions\".\"FarmId\" = @farmId";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@farmId", farmId);
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var partition = new FarmPartitions
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"],
                            Size = (int)reader["Size"]
                        };

                        partitions.Add(partition);
                        // if(servicePoint is ServicePoint)
                    }
                }
            }
            if (partitions.Count == 0)
                return null;
            return partitions;
        }

        public async Task<bool> CheckPartitions(int farmId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            string query = "SELECT COUNT(*) FROM public.\"FarmPartitions\" WHERE public.\"FarmPartitions\".\"FarmId\" = @farmId";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@farmId", farmId);
                var result = await command.ExecuteScalarAsync();

                // If there are partitions (result > 0), return true; otherwise, return false.
                return Convert.ToInt32(result) > 0;
            }
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
                        if (!reader.IsDBNull(reader.GetOrdinal("HasPartitions")))
                        {
                            farm.HasPartitions = (bool)reader["HasPartitions"];
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
                            ExpectedBagsHarvested = (int)reader["ExpectedBagsHarvested"],
                            PlantingDate = (DateTime)reader["PlantingDate"],
                            QuantityPlanted = (decimal)reader["QuantityPlanted"],
                            FarmSizePlanted = (decimal)reader["FarmSizePlanted"],
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
                            // PartitonPlanted = (decimal)reader["PartitonPlanted"],
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

        public async Task<List<StorageLocation>> ViewAllStorageLocations(int farmId)
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            connection.Open();

            var locations = new List<StorageLocation>();
            string query = "SELECT * FROM public.\"StorageLocations\" WHERE public.\"StorageLocations\".\"FarmId\" = @farmId";

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@farmId", farmId);
                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var location = new StorageLocation
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"],
                            CreatedAt = (DateTime)reader["CreatedAt"],
                            FarmId = (int)reader["FarmId"]
                        };

                        locations.Add(location);
                        // if(servicePoint is ServicePoint)
                    }
                }
            }
            if (locations.Count == 0)
                return null;
            return locations;
        }
    }
}
