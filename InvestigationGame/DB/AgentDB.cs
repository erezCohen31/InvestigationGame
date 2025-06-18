using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestigationGame.Personn;
using MySql.Data.MySqlClient;
using System.Text.Json;
using InvestigationGame.Personn.Agents;

namespace InvestigationGame.DB
{
    internal class AgentDB
    {
        private string connectionString;

        //constructor to initialize the database connection and create the database and table if they do not exist
        public AgentDB(string server, string database, string userId)
        {
            string initialConnectionString = $"Server={server};User ID={userId};";
            connectionString = $"Server={server};Database={database};User ID={userId};";

            try
            {
                using (var connection = new MySqlConnection(initialConnectionString))
                {
                    connection.Open();

                    string createDbQuery = $"CREATE DATABASE IF NOT EXISTS `{database}`;";
                    using (var command = new MySqlCommand(createDbQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string createTableQuery = @"
                        CREATE TABLE IF NOT EXISTS Agents (
                            Id INT AUTO_INCREMENT PRIMARY KEY,
                            `Rank` INT NOT NULL,
                            Capacity INT NOT NULL,
                            IsDiscovered BOOLEAN DEFAULT FALSE,
                            FoundCount INT DEFAULT 0,
                            NotCounterAttack INT DEFAULT 0,
                            Sensors JSON,
                            SensorsCopy JSON,
                            CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                        );";

                    using (var command = new MySqlCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing database: {ex.Message}");
                throw;
            }
        }

        // Method to create a new agent in the database
        public bool CreateAgent(Agent agent)
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

                    string sensorsJson = JsonSerializer.Serialize(agent.sensors);

                    string insertQuery = @"
                        INSERT INTO Agents (`Rank`, Capacity, IsDiscovered, FoundCount, NotCounterAttack, Sensors, SensorsCopy)
                        VALUES (@Rank, @Capacity, @IsDiscovered, @FoundCount, @NotCounterAttack, @Sensors, @SensorsCopy);
                        SELECT LAST_INSERT_ID();";

                    using (var command = new MySqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Rank", agent.rank);
                        command.Parameters.AddWithValue("@Capacity", agent.capacity);
                        command.Parameters.AddWithValue("@IsDiscovered", agent.isDiscovered);
                        command.Parameters.AddWithValue("@FoundCount", agent.foundCount);
                        command.Parameters.AddWithValue("@NotCounterAttack", agent.notCounterAttack);
                        command.Parameters.AddWithValue("@Sensors", agent.sensors != null ?
                        JsonSerializer.Serialize(agent.sensors) : DBNull.Value);
                        command.Parameters.AddWithValue("@SensorsCopy", agent.sensorsCopy != null ?
                        JsonSerializer.Serialize(agent.sensorsCopy) : DBNull.Value);

                        var result = command.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            agent.id = Convert.ToInt32(result);
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating agent: {ex.Message}");
                return false;
            }
        }

        // Method to update the discovery status of an agent
        public bool UpdateAgentDiscoveryStatus(int agentId, bool isDiscovered)
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
                        UPDATE Agents 
                        SET IsDiscovered = @IsDiscovered
                        WHERE Id = @AgentId;";

                    using (var command = new MySqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@IsDiscovered", isDiscovered);
                        command.Parameters.AddWithValue("@AgentId", agentId);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating agent status: {ex.Message}");
                return false;
            }
        }

        // Method to update the found count of an agent
        public bool UpdateAgentFoundCount(int agentId, int foundCount)
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
                        UPDATE Agents 
                        SET FoundCount = @FoundCount
                        WHERE Id = @AgentId;";

                    using (var command = new MySqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@FoundCount", foundCount);
                        command.Parameters.AddWithValue("@AgentId", agentId);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating found sensors counter: {ex.Message}");
                return false;
            }
        }

        // Method to update the non-counterattack counter of an agent
        public bool UpdateAgentNotCounterAttack(int agentId, int notCounterAttack)
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
                        UPDATE Agents 
                        SET NotCounterAttack = @NotCounterAttack
                        WHERE Id = @AgentId;";

                    using (var command = new MySqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@NotCounterAttack", notCounterAttack);
                        command.Parameters.AddWithValue("@AgentId", agentId);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating non-counterattack counter: {ex.Message}");
                return false;
            }
        }

        // Method to update the sensors copy of an agent
        public bool UpdateAgentSensorsCopy(int agentId, List<string> sensorsCopy)
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
                        UPDATE Agents 
                        SET SensorsCopy = @SensorsCopy
                        WHERE Id = @AgentId;";

                    using (var command = new MySqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@SensorsCopy", JsonSerializer.Serialize(sensorsCopy));
                        command.Parameters.AddWithValue("@AgentId", agentId);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating sensors copy: {ex.Message}");
                return false;
            }
        }

        // Method to retrieve all agents from the database
        public List<Agent> GetAllAgents()
        {
            var agents = new List<Agent>();

            try
            {
                using (var connection = new MySqlConnection(connectionString.Replace("Database=InvestigationGame;", "")))
                {
                    connection.Open();

                    using (var cmdUseDb = new MySqlCommand("USE InvestigationGame;", connection))
                    {
                        cmdUseDb.ExecuteNonQuery();
                    }

                    string query = "SELECT * FROM Agents";

                    using (var command = new MySqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int rank = reader.GetInt32("Rank");


                            Agent agent = rank switch
                            {
                                1 => new FootSoldier(),
                                2 => new SquadLeader(),
                                3 => new SeniorCommander(),
                                4 => new OraganizationLeader(),
                                _ => throw new Exception($"Unknown agent rank: {rank}")
                            };


                            agent.id = reader.GetInt32("Id");
                            agent.isDiscovered = reader.GetBoolean("IsDiscovered");
                            agent.foundCount = reader.GetInt32("FoundCount");
                            agent.notCounterAttack = reader.GetInt32("NotCounterAttack");


                            if (!reader.IsDBNull(reader.GetOrdinal("Sensors")))
                            {
                                string sensorsJson = reader.GetString("Sensors");
                                agent.sensors = JsonSerializer.Deserialize<List<string>>(sensorsJson) ?? new List<string>();
                            }


                            agents.Add(agent);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving agents: {ex.Message}");
                throw;
            }

            return agents;
        }
    

        // Method to get the total count of agents in the database
        public int GetAgentsCount()
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string countQuery = "SELECT COUNT(*) FROM Agents;";

                    using (var command = new MySqlCommand(countQuery, connection))
                    {
                        return Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting agents count: {ex.Message}");
                return -1; // Return -1 to indicate an error
            }
        }

        // Method to get an agent by its ID
        public Agent GetAgentById(int agentId)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM Agents WHERE Id = @AgentId;";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AgentId", agentId);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var agent = new Agent
                                {
                                    id = reader.GetInt32("Id"),
                                    rank = reader.GetInt32("Rank"),
                                    capacity = reader.GetInt32("Capacity"),
                                    isDiscovered = reader.GetBoolean("IsDiscovered"),
                                    foundCount = reader.GetInt32("FoundCount"),
                                    notCounterAttack = reader.GetInt32("NotCounterAttack")
                                };

                                // Désérialiser les capteurs
                                if (!reader.IsDBNull(reader.GetOrdinal("Sensors")))
                                {
                                    string sensorsJson = reader.GetString("Sensors");
                                    agent.sensors = JsonSerializer.Deserialize<List<string>>(sensorsJson);
                                }

                                // Désérialiser les copies des capteurs
                                if (!reader.IsDBNull(reader.GetOrdinal("SensorsCopy")))
                                {
                                    string sensorsCopyJson = reader.GetString("SensorsCopy");
                                    agent.sensorsCopy = JsonSerializer.Deserialize<List<string>>(sensorsCopyJson);
                                }

                                return agent;
                            }
                            return null; // Aucun agent trouvé avec cet ID
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting agent by ID {agentId}: {ex.Message}");
                return null;
            }
        }

        // Method to check if an agent is terminated (sensorsCopy is empty)
        public bool GetAgentTerminateStatus(int agentId)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT JSON_LENGTH(SensorsCopy) = 0 AS IsTerminate FROM Agents WHERE Id = @AgentId;";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AgentId", agentId);
                        
                        var result = command.ExecuteScalar();
                        if (result == null || result == DBNull.Value)
                        {
                            return false;
                        }

                        return Convert.ToBoolean(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting terminate status for agent {agentId}: {ex.Message}");
                return false;
            }
        }


        // Method to get all sensors for a specific agent
        public List<string> GetAgentSensors(int agentId)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT Sensors FROM Agents WHERE Id = @AgentId;";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AgentId", agentId);
                        
                        var result = command.ExecuteScalar();
                        if (result == null || result == DBNull.Value)
                        {
                            return new List<string>(); // Retourne une liste vide si l'agent n'est pas trouvé
                        }

                        string sensorsJson = result.ToString();
                        var sensors = JsonSerializer.Deserialize<List<string>>(sensorsJson);
                        
                        return sensors ?? new List<string>();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting sensors for agent {agentId}: {ex.Message}");
                return new List<string>(); // Retourne une liste vide en cas d'erreur
            }
        }

        // Method to get all agents for a specific player
        public List<Agent> GetAgentsByPlayerId(int playerId)
        {
            var agents = new List<Agent>();

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT a.* 
                        FROM Agents a
                        INNER JOIN PlayerAgents pa ON a.Id = pa.AgentId
                        WHERE pa.PlayerId = @PlayerId";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PlayerId", playerId);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int rank = reader.GetInt32("Rank");
                                Agent agent = rank switch
                                {
                                    1 => new FootSoldier(),
                                    2 => new SquadLeader(),
                                    3 => new SeniorCommander(),
                                    4 => new OraganizationLeader(),
                                    _ => throw new Exception($"Unknown agent rank: {rank}")
                                };

                                agent.id = reader.GetInt32("Id");
                                agent.isDiscovered = reader.GetBoolean("IsDiscovered");
                                agent.foundCount = reader.GetInt32("FoundCount");
                                agent.notCounterAttack = reader.GetInt32("NotCounterAttack");

                                // Désérialiser les capteurs
                                if (!reader.IsDBNull(reader.GetOrdinal("Sensors")))
                                {
                                    string sensorsJson = reader.GetString("Sensors");
                                    agent.sensors = JsonSerializer.Deserialize<List<string>>(sensorsJson) ?? new List<string>();
                                }

                                // Désérialiser les copies des capteurs
                                if (!reader.IsDBNull(reader.GetOrdinal("SensorsCopy")))
                                {
                                    string sensorsCopyJson = reader.GetString("SensorsCopy");
                                    agent.sensorsCopy = JsonSerializer.Deserialize<List<string>>(sensorsCopyJson) ?? new List<string>();
                                }

                                agents.Add(agent);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving agents for player {playerId}: {ex.Message}");
                throw;
            }

            return agents;
        }
    }
}
