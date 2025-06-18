using MySql.Data.MySqlClient;
using InvestigationGame.Sensors;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace InvestigationGame.DB
{
    internal class SensorDB
    {
        private string connectionString;

        // Constructor to initialize the database connection and create the database and table if they do not exist
        public SensorDB(string server, string database, string userId)
        {
            string initialConnectionString = $"Server={server};User ID={userId};";
            connectionString = $"Server={server};Database={database};User ID={userId};";
            
            try
            {
                using (var connection = new MySqlConnection(initialConnectionString))
                {
                    connection.Open();
                    
                    string createDbQuery = $"CREATE DATABASE IF NOT EXISTS `{database}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";
                    using (var command = new MySqlCommand(createDbQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    

                    string createTableQuery = @"
                        CREATE TABLE IF NOT EXISTS Sensors (
                            Id INT AUTO_INCREMENT PRIMARY KEY,
                            Type VARCHAR(50) NOT NULL,
                            IsActive BOOLEAN DEFAULT FALSE,
                            ActivateCount INT DEFAULT 0,
                            MaxActivateCount INT DEFAULT 0,
                            AgentId INT NOT NULL,
                            CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                            FOREIGN KEY (AgentId) REFERENCES Agents(Id) ON DELETE CASCADE
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;";

                    using (var cmdTable = new MySqlCommand(createTableQuery, connection))
                    {
                        cmdTable.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing database: {ex.Message}");
                throw;
            }
        }


        // Method to create a new sensor in the database
        public bool CreateSensor(Sensor sensor, int agentId)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString.Replace("Database=InvestigationGame;", "")))
                {
                    connection.Open();

                    using (var cmdUseDb = new MySqlCommand("USE InvestigationGame;", connection))
                    {
                        cmdUseDb.ExecuteNonQuery();
                    }

                    string insertQuery = @"
                        INSERT INTO Sensors (Type, IsActive, ActivateCount, MaxActivateCount, AgentId)
                        VALUES (@Type, @IsActive, @ActivateCount, @MaxActivateCount, @AgentId);
                        SELECT LAST_INSERT_ID();";

                    using (var command = new MySqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Type", sensor.Type);
                        command.Parameters.AddWithValue("@IsActive", sensor.IsActive);
                        command.Parameters.AddWithValue("@ActivateCount", sensor.ActivateCount);
                        // Récupérer le champ maxActivateCount même s'il est protégé
                        var field = sensor.GetType().GetField("maxActivateCount", 
                            System.Reflection.BindingFlags.NonPublic | 
                            System.Reflection.BindingFlags.Instance);
                        int maxActivateCountValue = (int)(field?.GetValue(sensor) ?? 0);
                        command.Parameters.AddWithValue("@MaxActivateCount", maxActivateCountValue);
                        command.Parameters.AddWithValue("@AgentId", agentId);

                        var result = command.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int sensorId))
                        {
                            sensor.Id = sensorId;
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating sensor: {ex.Message}");
            }

            return false;
        }

        // Method to update the status of a sensor in the database
        public bool UpdateSensorStatus(int sensorId, bool isActive)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString.Replace("Database=InvestigationGame;", "")))
                {
                    connection.Open();

                    using (var cmdUseDb = new MySqlCommand("USE InvestigationGame;", connection))
                    {
                        cmdUseDb.ExecuteNonQuery();
                    }

                    string updateQuery = @"
                        UPDATE Sensors 
                        SET IsActive = @IsActive
                        WHERE Id = @SensorId;";

                    using (var command = new MySqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@IsActive", isActive);
                        command.Parameters.AddWithValue("@SensorId", sensorId);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating sensor status: {ex.Message}");
                return false;
            }
        }

        // Method to retrieve all sensors for a specific agent from the database
        public List<Sensor> GetSensorsByAgent(int agentId)
        {
            var sensors = new List<Sensor>();

            try
            {
                using (var connection = new MySqlConnection(connectionString.Replace("Database=InvestigationGame;", "")))
                {
                    connection.Open();

                    using (var cmdUseDb = new MySqlCommand("USE InvestigationGame;", connection))
                    {
                        cmdUseDb.ExecuteNonQuery();
                    }

                    string query = "SELECT * FROM Sensors WHERE AgentId = @AgentId";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AgentId", agentId);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Sensor sensor = CreateSensorFromReader(reader);
                                if (sensor != null)
                                {
                                    sensors.Add(sensor);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving sensors: {ex.Message}");
            }

            return sensors;
        }

        // Method to retrieve sensor from the database
        private Sensor CreateSensorFromReader(MySqlDataReader reader)
        {
            try
            {
                string type = reader["Type"].ToString();
                Sensor sensor = type switch
                {
                    "Pulse" => new Pulse(),
                    "Motion" => new Motion(),
                    "Thermal" => new Thermal(),
                    "Light" => new Light(),
                    "Magnetic" => new Magnetic(),
                    "Audio" => new Audio(),
                    _ => null
                };

                if (sensor != null)
                {
                    sensor.Id = Convert.ToInt32(reader["Id"]);
                    sensor.Type = type;
                    sensor.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    sensor.ActivateCount = Convert.ToInt32(reader["ActivateCount"]);
                    

                    var maxActivateCountField = sensor.GetType().GetField("maxActivateCount",
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);

                    if (maxActivateCountField != null)
                    {
                        maxActivateCountField.SetValue(sensor, Convert.ToInt32(reader["MaxActivateCount"]));
                    }
                }


                return sensor;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating sensor from reader: {ex.Message}");
                return null;
            }
        }

        // Method to update the activation count of a sensor in the database
        public bool UpdateSensorActivateCount(int sensorId, int newCount)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString.Replace("Database=InvestigationGame;", "")))
                {
                    connection.Open();

                    using (var cmdUseDb = new MySqlCommand("USE InvestigationGame;", connection))
                    {
                        cmdUseDb.ExecuteNonQuery();
                    }

                    string updateQuery = @"
                        UPDATE Sensors 
                        SET ActivateCount = @NewCount
                        WHERE Id = @SensorId;";

                    using (var command = new MySqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@NewCount", newCount);
                        command.Parameters.AddWithValue("@SensorId", sensorId);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating sensor activation count: {ex.Message}");
                return false;
            }
        }

  
    }
}
