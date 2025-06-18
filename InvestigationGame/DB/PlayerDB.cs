using MySql.Data.MySqlClient;
using InvestigationGame.Person;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using InvestigationGame.Person.Agents;

namespace InvestigationGame.DB
{
    internal class PlayerDB
    {
        private string connectionString;

        // Constructor to initialize the database connection and create the Players table if it doesn't exist
        public PlayerDB(string server, string database, string userId)
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
                        CREATE TABLE IF NOT EXISTS Players (
                            Id INT AUTO_INCREMENT PRIMARY KEY,
                            Name VARCHAR(100) NOT NULL,
                            Score INT DEFAULT 0,
                            CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                            UNIQUE KEY unique_name (Name)
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
                        
                        CREATE TABLE IF NOT EXISTS PlayerAgents (
                            PlayerId INT NOT NULL,
                            AgentId INT NOT NULL,
                            PRIMARY KEY (PlayerId, AgentId),
                            FOREIGN KEY (PlayerId) REFERENCES Players(Id) ON DELETE CASCADE,
                            FOREIGN KEY (AgentId) REFERENCES Agents(Id) ON DELETE CASCADE
                        ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;"; ;

                    using (var cmdTable = new MySqlCommand(createTableQuery, connection))
                    {
                        cmdTable.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing player database: {ex.Message}");
                throw;
            }
        }

        // Create a new player
        public bool CreatePlayerInDb(Player player)
        {
            if (player == null || string.IsNullOrWhiteSpace(player.name))
                return false;

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string insertQuery = @"
                        INSERT INTO Players (Name, Score, CreatedAt)
                        VALUES (@Name, 0, NOW());
                        SELECT LAST_INSERT_ID();";

                    using (var transaction = connection.BeginTransaction())
                    using (var command = new MySqlCommand(insertQuery, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@Name", player.name);

                        try
                        {
                            int newId = Convert.ToInt32(command.ExecuteScalar());
                            transaction.Commit();

                            player.id = newId;
                            return true;
                        }
                        catch (MySqlException sqlEx)
                        {
                            transaction.Rollback();
                            Console.WriteLine($"SQL Error creating player: {sqlEx.Message}");
                            throw new Exception("Failed to create player in database. The player name might already exist.", sqlEx);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine($"Error creating player: {ex.Message}");
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating player: {ex.Message}");
                return false;
            }
        }



        // Retrieve a player by name with their agents
        public Player GetPlayer(string playerName, bool loadAgents = true)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT p.Id, p.Name, p.Score, p.CreatedAt 
                        FROM Players p 
                        WHERE p.Name = @Name";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", playerName);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var player = new Player(
                                    reader.GetInt32("Id"),
                                    reader.GetString("Name"),
                                    reader.GetInt32("Score"),
                                    reader.GetDateTime("CreatedAt")
                                );

                                // Load agents if requested
                                if (loadAgents)
                                {
                                    LoadPlayerAgents(connection, player);
                                }

                                return player;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving player: {ex.Message}");
            }

            return null;
        }

        // Load agents for a player
        private void LoadPlayerAgents(MySqlConnection connection, Player player)
        {
            string query = @"
                SELECT a.* 
                FROM Agents a
                INNER JOIN PlayerAgents pa ON a.Id = pa.AgentId
                WHERE pa.PlayerId = @PlayerId";

            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@PlayerId", player.id);

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
                        agent.rank = rank;
                        agent.capacity = reader.GetInt32("Capacity");
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

                        player.AddAgent(agent);
                    }
                }
            }
        }

        // Add an agent to a player
        public bool AddAgentToPlayer(int playerId, int agentId)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        INSERT INTO PlayerAgents (PlayerId, AgentId)
                        VALUES (@PlayerId, @AgentId)";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PlayerId", playerId);
                        command.Parameters.AddWithValue("@AgentId", agentId);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding agent to player: {ex.Message}");
                return false;
            }
        }

        // Get all agents for a player
        public List<Agent> GetPlayerAgents(int playerId)
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
                                var agent = new Agent();
                                agent.id = reader.GetInt32("Id");
                                agent.rank = reader.GetInt32("Rank");
                                agent.capacity = reader.GetInt32("Capacity");
                                // Map other agent properties

                                agents.Add(agent);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving player agents: {ex.Message}");
            }

            return agents;
        }

        // Get all players from the database
        public List<Player> GetAllPlayers(bool loadAgents = false)
        {
            var players = new List<Player>();

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                SELECT Id, Name, Score, CreatedAt 
                FROM Players
                ORDER BY Score DESC, Name ASC";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var player = new Player(
                                    reader.GetInt32("Id"),
                                    reader.GetString("Name"),
                                    reader.GetInt32("Score"),
                                    reader.GetDateTime("CreatedAt")
                                );

                                players.Add(player);
                            }
                        }

                        if (loadAgents && players.Any())
                        {
                            foreach (var player in players)
                            {
                                LoadPlayerAgents(connection, player);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving all players: {ex.Message}");
            }

            return players;
        }


        // Update a player's score
        public Player UpdatePlayerScore(int playerId, int scoreIncrement)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Update the score
                    string updateQuery = @"
                        UPDATE Players 
                        SET Score = Score + @ScoreIncrement
                        WHERE Id = @PlayerId";

                    using (var command = new MySqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@ScoreIncrement", scoreIncrement);
                        command.Parameters.AddWithValue("@PlayerId", playerId);

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected == 0)
                            return null;
                    }


                    // Return the updated player
                    string selectQuery = "SELECT Id, Name, Score, CreatedAt FROM Players WHERE Id = @PlayerId";
                    using (var selectCommand = new MySqlCommand(selectQuery, connection))
                    {
                        selectCommand.Parameters.AddWithValue("@PlayerId", playerId);

                        using (var reader = selectCommand.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Player(
                                    reader.GetInt32("Id"),
                                    reader.GetString("Name"),
                                    reader.GetInt32("Score"),
                                    reader.GetDateTime("CreatedAt")
                                );
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating player score: {ex.Message}");
            }

            return null;
        }

        // Retrieve the leaderboard of players, sorted by score
        public List<Player> GetLeaderboard(int limit = 10)
        {
            var leaderboard = new List<Player>();

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT Id, Name, Score, CreatedAt 
                        FROM Players 
                        ORDER BY Score DESC 
                        LIMIT @Limit";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Limit", limit);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                leaderboard.Add(new Player(
                                    reader.GetInt32("Id"),
                                    reader.GetString("Name"),
                                    reader.GetInt32("Score"),
                                    reader.GetDateTime("CreatedAt")
                                ));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving leaderboard: {ex.Message}");
            }

            return leaderboard;
        }

        // Verify if a player has any active agents
        public bool HasActiveAgents(int playerId)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT COUNT(*) 
                        FROM PlayerAgents pa
                        INNER JOIN Agents a ON pa.AgentId = a.Id
                        WHERE pa.PlayerId = @PlayerId
                        AND (a.SensorsCopy IS NULL OR JSON_LENGTH(a.SensorsCopy) > 0);";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PlayerId", playerId);
                        
                        int activeAgentsCount = Convert.ToInt32(command.ExecuteScalar());
                        return activeAgentsCount > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking active agents for player {playerId}: {ex.Message}");
                throw;
            }
        }
    }
}
