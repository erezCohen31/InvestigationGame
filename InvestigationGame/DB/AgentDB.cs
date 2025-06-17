using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvestigationGame.Agents;
using MySql.Data.MySqlClient;
using System.Text.Json;

namespace InvestigationGame.DB
{
    internal class AgentDB
    {
        private string connectionString;

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

        public Agent GetAgentById(int agentId)
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

                    string query = "SELECT * FROM Agents WHERE Id = @AgentId";
                    
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AgentId", agentId);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
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
                                
                                if (!reader.IsDBNull(reader.GetOrdinal("SensorsCopy")))
                                {
                                    string sensorsCopyJson = reader.GetString("SensorsCopy");
                                    agent.sensorsCopy = JsonSerializer.Deserialize<List<string>>(sensorsCopyJson) ?? new List<string>();
                                }
                                else
                                {
                                    // Si pas de copie, on la crée à partir de la liste principale
                                    agent.UpdateSensorsCopy();
                                }
                                
                                return agent;
                            }
                        }
                    }                }
                
                return null; 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving agent with ID {agentId}: {ex.Message}");
                throw;
            }
        }

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
    }
}
