using InvestigationGame.Person;
using InvestigationGame.Sensors;
using InvestigationGame.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using InvestigationGame.Person.Agents;

namespace InvestigationGame.Manager
{
    internal class GameManager
    {
        private const string DB_SERVER = "localhost";
        private const string DB_NAME = "InvestigationGame";
        private const string DB_USER = "root";

        private AgentDB agentDB;
        private SensorDB sensorDB;
        private PlayerDB playerDB;


        private int counterRound;

        //constructor to initialize the GameManager
        public GameManager()
        {
            try
            {
                agentDB = new AgentDB(DB_SERVER, DB_NAME, DB_USER);
                sensorDB = new SensorDB(DB_SERVER, DB_NAME, DB_USER);
                playerDB = new PlayerDB(DB_SERVER, DB_NAME, DB_USER);

                Console.WriteLine("Game Manager initialized. Database connections established.");


                counterRound = 0;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing GameManager: {ex.Message}");
                throw;
            }
        }


        // Method to display the main menu and handle user input
        public void Menu()
        {
            Console.WriteLine("Welcome to the Investigation Game!");
            Console.WriteLine("Please select an option:");
            Console.WriteLine("1. Start Game");
            Console.WriteLine("2. Exit");

            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    int playerId = ChoosePlayer();
                    StartGame(playerId);
                    break;
                case "2":
                    ExitGame();
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    Menu();
                    break;
            }
        }

        //Method to choose the player
        public int ChoosePlayer()
        {
            List<Player> players = playerDB.GetAllPlayers();
            if (players.Count == 0)
            {
                Console.WriteLine("No players found in the database. Creating a new player.");
                return CreatePlayer();
            }
            else
            {
                Console.WriteLine("Choose a player from the list below:");
                foreach (var player in players)
                {
                    Console.WriteLine($"ID: {player.id}, Name: {player.name}, Score: {player.score}, Created At: {player.createdAt}");
                }
                Console.WriteLine("Enter the ID of the player you want to choose or type 'new' to create a new player:");
                string input = Console.ReadLine();
                if (input.ToLower() == "new")
                {
                    bool isCreated = false;

                    int newPlayerId = -1;
                    while (!isCreated)
                    {
                        newPlayerId = CreatePlayer();
                        if (newPlayerId != -1)
                        {
                            isCreated = true;
                            Console.WriteLine($"New player created with ID: {newPlayerId}");
                        }
                        else
                        {
                            Console.WriteLine("Failed to create player. Please try again.");
                        }
                    }
                    return newPlayerId;
                }
                else if (int.TryParse(input, out int playerId) && players.Any(p => p.id == playerId))
                {
                    Player selectedPlayer = players.First(p => p.id == playerId);
                    Console.WriteLine($"Welcome back, {selectedPlayer.name}! Your ID is {selectedPlayer.id}.");
                    return selectedPlayer.id;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please try again.");
                    return -1;
                }
            }

        }

        public int CreatePlayer()
        {
            Console.WriteLine("Please enter your name:");
            string playerName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(playerName))
            {
                Console.WriteLine("Invalid name. Please try again.");
                return -1;
            }
            else
            {
                Player player = new Player(playerName, 0, DateTime.Now);
                playerDB.CreatePlayerInDb(player);
                Console.WriteLine($"Welcome, {player.name}! Your ID is {player.id}.");
                Console.WriteLine("Your player has been created successfully.");
                return player.id;
            }
        }

        // Method to start the game
        public void StartGame(int playerId)
        {
            try
            {
                Console.WriteLine("\n=== Game Setup ===");
                Console.WriteLine("1. Create new agents");
                Console.WriteLine("2. Use existing agents from database");
                Console.WriteLine("3. Exit the game");
                Console.Write("Choose an option (1-2): ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("\nHow many agents do you want to create? (1-10): ");
                        if (int.TryParse(Console.ReadLine(), out int count) && count > 0 && count <= 10)
                        {
                            InitializeAgents(count, playerId);
                        }
                        else
                        {
                            Console.WriteLine("Invalid number. Creating 2 agents by default.");
                            InitializeAgents(2, playerId);
                        }
                        break;

                    case "2":
                        var agentsCount = playerDB.GetPlayerAgents(playerId).Count;
                        if (agentsCount == 0 ||!playerDB.HasActiveAgents(playerId))

                        {
                            Console.WriteLine("No agents found in database. Creating 2 new agents by default.");
                            InitializeAgents(2, playerId);
                        }
                        else
                        {
                            Console.WriteLine($"\nFound {agentsCount} agents in database.");
                        }
                        break;

                    case "3":
                        ExitGame();
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Creating 2 agents by default.");
                        InitializeAgents(2, playerId);
                        break;
                }

                ChooseAgentLoop(playerId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }

        // Method to initialize agents with random ranks and save them to the database
        private void InitializeAgents(int count, int playerId = -1)
        {
            try
            {
                if (playerId == -1)
                {
                    Console.WriteLine("Error: Player ID is required to initialize agents");
                    return;
                }

                Random random = new Random();
                for (int i = 0; i < count; i++)
                {
                    int rank = random.Next(1, 5);
                    Agent agent = rank switch
                    {
                        1 => new FootSoldier(),
                        2 => new SquadLeader(),
                        3 => new SeniorCommander(),
                        4 => new OraganizationLeader(),
                        _ => throw new Exception("Rank out of bounds")
                    };

                    // Create the agent in the database
                    bool isCreated = agentDB.CreateAgent(agent);
                    
                    if (isCreated && agent.id > 0)
                    {
                        // Associate the agent with the player
                        bool isAssociated = playerDB.AddAgentToPlayer(playerId, agent.id);
                        
                        if (isAssociated)
                        {
                            Console.WriteLine($"Agent {agent.id} (Rank: {agent.rank}) created and associated with player {playerId}");
                        }
                        else
                        {
                            Console.WriteLine($"Agent {agent.id} created but failed to associate with player {playerId}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Failed to create agent. Please try again.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing agents: {ex.Message}");
                throw;
            }
        }

        // Method to handle the agent selection loop
        private void ChooseAgentLoop(int playerId)
        {
            bool isGameOver = false;

            while (!isGameOver)
            {
                DisplayAgentsStatus(playerId);

                int agentId = GetValidAgentId();

                Agent chosenAgent = agentDB.GetAgentById(agentId);

                Console.WriteLine($"You have chosen Agent {chosenAgent.id}.");

                FindSensors(chosenAgent);

                isGameOver = CheckIfGameOver(playerId);

                if (isGameOver)
                    ExitGame();
            }
        }

        // Method to display the status of all agents
        private void DisplayAgentsStatus(int playerId)
        {
            var agents = playerDB.GetPlayerAgents(playerId);
            foreach (var agent in agents)
            {
                if (agent.isDiscovered || agentDB.GetAgentTerminateStatus(agent.id))
                    Console.WriteLine($"Agent ID: {agent.id}, Rank: {agent.rank}, Capacity: {agent.capacity}, Find: {agent.isDiscovered} in percent: {agent.foundCount*100/agent.capacity} %");
                else
                    Console.WriteLine($"Agent ID: {agent.id}, Find: {agent.isTerminate}");
            }
        }

        // Method to get a valid agent ID from the user
        private int GetValidAgentId()
        {
            int agentId;
            while (true)
            {
                Console.Write("Enter agent ID: ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out agentId) && agentId >= 0 && !agentDB.GetAgentTerminateStatus(agentId))
                {
                    break;
                }

                Console.WriteLine("Invalid ID or already found. Please enter a valid agent ID.");
            }

            return agentId;
        }

        // Method to check if the game is over
        private bool CheckIfGameOver(int playerId)
        {
            return playerDB.GetPlayerAgents(playerId).All(agent => agent.isTerminate);
        }

        // Method to exit the game
        public void ExitGame()
        {
            Console.WriteLine("Exiting the game. Goodbye!");
            Environment.Exit(0);
        }

        // Method to handle the counter-attack logic
        public void CounterAttack(Agent iranianAgent)
        {
            try
            {
                if (counterRound % 3 == 0)
                {
                    if (iranianAgent.rank == 2 || iranianAgent.rank == 4)
                    {
                        HandleCounterAttackSingleSensorLoss(iranianAgent);
                    }
                    else if (iranianAgent.rank == 3)
                    {
                        HandleCounterAttackDoubleSensorLoss(iranianAgent);
                    }
                }
                if (counterRound % 10 == 0 && iranianAgent.rank == 4)
                {
                    HandleCounterAttackAllSensorsLoss(iranianAgent);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during the counter-attack: {ex.Message}");
            }
        }

        // Methods to handle different counter-attack scenarios based on agent rank
        private void HandleCounterAttackSingleSensorLoss(Agent iranianAgent)
        {
            if (iranianAgent.notCounterAttack > 0)
            {
                iranianAgent.notCounterAttack--;
                Console.WriteLine("You have been attacked by the enemy. You lost one notCounterAttack point.");
                return;
            }

            var sensors = sensorDB.GetSensorsByAgent(iranianAgent.id);


            if (sensors.Count > 0)
            {
                Random rnd = new Random();
                int indexToRemove = rnd.Next(sensors.Count);
                iranianAgent.sensorsCopy.Add(sensors[indexToRemove].type);
                sensors.RemoveAt(indexToRemove);
                iranianAgent.foundCount--;
                Console.WriteLine("You have been attacked by the enemy. You lost a sensor");
            }
        }

        private void HandleCounterAttackDoubleSensorLoss(Agent iranianAgent)
        {
            if (iranianAgent.notCounterAttack > 0)
            {
                iranianAgent.notCounterAttack--;
                Console.WriteLine("You have been attacked by the enemy. You lost one notCounterAttack point.");
                return;
            }

            var sensors = sensorDB.GetSensorsByAgent(iranianAgent.id);
            Random rnd = new Random();
            for (int i = 0; i < 2 && sensors.Count > 0; i++)
            {
                int indexToRemove = rnd.Next(sensors.Count);
                iranianAgent.sensorsCopy.Add(sensors[indexToRemove].type);
                sensors.RemoveAt(indexToRemove);
                iranianAgent.foundCount--;
            }
            Console.WriteLine("You have been attacked by the enemy. You lost 2 sensors");
        }

        private void HandleCounterAttackAllSensorsLoss(Agent iranianAgent)
        {
            if (iranianAgent.notCounterAttack > 0)
            {
                iranianAgent.notCounterAttack--;
                Console.WriteLine("You have been attacked by the enemy. You lost one notCounterAttack point.");
                return;
            }

            var sensors = sensorDB.GetSensorsByAgent(iranianAgent.id);
            foreach (var sensor in sensors)
            {
                iranianAgent.sensorsCopy.Add(sensor.type);
            }
            sensors.Clear();
            iranianAgent.foundCount = 0;
            Console.WriteLine("All your sensors have been removed by the enemy attack.");
        }

        // Method to find sensors for the given agent
        public bool FindSensors(Agent iranianAgent)
        {
            counterRound++;
            CounterAttack(iranianAgent);



            bool isFind = false;

            while (!isFind)
            {
                Sensor currentSensor = PromptSensorSelection();

                if (currentSensor == null)
                {
                    Console.WriteLine("Invalid sensor type. Please try again.");
                    continue;
                }


                int response = currentSensor.ActivateSensor(iranianAgent, sensorDB.GetSensorsByAgent(iranianAgent.id));
                if (response > 0)
                {
                    sensorDB.UpdateSensorActivateCount(response, iranianAgent.foundCount);
                    sensorDB.UpdateSensorStatus(response, currentSensor.IsActive);
                }

                else if (response == 0)
                {
                    iranianAgent.foundCount++;

                    agentDB.UpdateAgentFoundCount(iranianAgent.id, iranianAgent.foundCount);
                    agentDB.UpdateAgentDiscoveryStatus(iranianAgent.id, iranianAgent.isDiscovered);
                    agentDB.UpdateAgentNotCounterAttack(iranianAgent.id, iranianAgent.notCounterAttack);
                    agentDB.UpdateAgentSensorsCopy(iranianAgent.id, iranianAgent.sensorsCopy);


                    bool createSensorId = sensorDB.CreateSensor(currentSensor, iranianAgent.id);



                    if (createSensorId)
                    {
                        bool sensorUpdateSuccess = sensorDB.UpdateSensorStatus(currentSensor.Id, currentSensor.IsActive);
                        if (!sensorUpdateSuccess)
                        {
                            Console.WriteLine($"Warning: Could not update sensor {currentSensor.Id} status in database");
                        }
                    }


                    Console.WriteLine($"You found a {currentSensor.type} sensor!");
                    if (iranianAgent.isDiscovered)
                    {
                        Console.WriteLine($"You found {iranianAgent.foundCount}/{iranianAgent.capacity} sensors.");
                    }
                    else
                    {
                        Console.WriteLine($"You found {iranianAgent.foundCount} sensors.");
                    }
                }
                else
                {
                    Console.WriteLine("You didn't find the sensor. Try again.");
                }

                if (iranianAgent.sensorsCopy.Count == 0)
                {
                    isFind = true;
                    iranianAgent.isTerminate = true;

                    bool agentUpdateSuccess = agentDB.UpdateAgentDiscoveryStatus(iranianAgent.id, true);
                    if (!agentUpdateSuccess)
                    {
                        Console.WriteLine("Warning: Could not update agent's discovery status in database");
                    }

                    Console.WriteLine($"You found {iranianAgent.foundCount*100/iranianAgent.capacity} % sensors!");
                }
            }

            return isFind;
        }

        // Method to prompt the user for sensor selection
        private Sensor PromptSensorSelection()
        {
            Console.WriteLine("Enter a sensor type:");
            Console.WriteLine("1. Audio");
            Console.WriteLine("2. Thermal");
            Console.WriteLine("3. Pulse");
            Console.WriteLine("4. Motion");
            Console.WriteLine("5. Magnetic");
            Console.WriteLine("6. Signal");
            Console.WriteLine("7. Light");
            Console.WriteLine("8. Return to Menu");

            switch (Console.ReadLine())
            {
                case "1": return new Audio();
                case "2": return new Thermal();
                case "3": return new Pulse();
                case "4": return new Motion();
                case "5": return new Magnetic();
                case "6": return new Signal();
                case "7": return new Light();

                default: return null;
            }
        }
    }
}
