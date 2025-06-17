using InvestigationGame.Agents;
using InvestigationGame.Sensors;
using InvestigationGame.DB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InvestigationGame.Manager
{
    internal class GameManager
    {
        private const string DB_SERVER = "localhost";
        private const string DB_NAME = "InvestigationGame";
        private const string DB_USER = "root";

        private AgentDB agentDB;
        private SensorDB sensorDB;

        private SensorManager sensorManager;
        private AgentManager agentManager;
        private int counterRound;

        public GameManager()
        {
            try
            {
                agentDB = new AgentDB(DB_SERVER, DB_NAME, DB_USER);
                sensorDB = new SensorDB(DB_SERVER, DB_NAME, DB_USER);

                Console.WriteLine("Game Manager initialized. Database connections established.");

                sensorManager = new SensorManager();
                agentManager = new AgentManager();
                counterRound = 0;

                LoadGameState();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing GameManager: {ex.Message}");
                throw;
            }
        }

        private void LoadGameState()
        {
            try
            {
                Console.WriteLine("Loading game state from database...");

                agentManager.agents = agentDB.GetAllAgents();
                var agents = agentManager.agents;

                if (agents.Count == 0)
                {
                    Console.WriteLine("No saved game found. Starting a new game.");
                    return;
                }

                Console.WriteLine($"Found {agents.Count} saved agents.");

                foreach (var agent in agents)
                {
                    agentManager.agentByWin[agent.id] = agent.isDiscovered;

                    sensorManager.AddAgent(agent.id);

                    var sensors = sensorDB.GetSensorsByAgent(agent.id);

                    foreach (var sensor in sensors)
                    {
                        sensorManager.AddSensor(agent.id, sensor);
                    }

                }

                Console.WriteLine("Game state loaded successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading game state: {ex.Message}");
                Console.WriteLine("Starting a new game...");
                agentManager.agentByWin.Clear();
                sensorManager.sensorsByAgent.Clear();
            }
        }

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
                    StartGame();
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

        public void StartGame()
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
                            InitializeAgents(count);
                        }
                        else
                        {
                            Console.WriteLine("Invalid number. Creating 2 agents by default.");
                            InitializeAgents(2);
                        }
                        break;

                    case "2":
                        var agents = agentDB.GetAllAgents();
                        if (agents.Count == 0 || agentManager.agentByWin.Values.All(found => found))

                        {
                            Console.WriteLine("No agents found in database. Creating 2 new agents by default.");
                            InitializeAgents(2);
                        }
                        else
                        {
                            Console.WriteLine($"\nFound {agents.Count} agents in database.");
                        }
                        break;

                    case "3":
                        ExitGame();
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Creating 2 agents by default.");
                        InitializeAgents(2);
                        break;
                }

                ChooseAgentLoop();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }
        }

        private void InitializeAgents(int count)
        {
            try
            {
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

                    bool isCreate = agentDB.CreateAgent(agent);

                    agentManager.agents.Add(agent);
                    agentManager.agentByWin.Add(agent.id, false);
                    sensorManager.AddAgent(agent.id);

                    Console.WriteLine($"Agent {agent.id} created and saved to database");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing agents: {ex.Message}");
                throw;
            }
        }

        private void ChooseAgentLoop()
        {
            bool isGameOver = false;

            while (!isGameOver)
            {
                DisplayAgentsStatus();

                int agentId = GetValidAgentId();

                Agent chosenAgent = agentManager.agents.FirstOrDefault(a => a.id == agentId);

                Console.WriteLine($"You have chosen Agent {chosenAgent.id}.");

                FindSensors(chosenAgent);

                isGameOver = CheckIfGameOver();

                if (isGameOver)
                    ExitGame();
            }
        }

        private void DisplayAgentsStatus()
        {
            var agents = agentManager.agents;
            var agentsByWin = agentManager.agentByWin;
            foreach (var agent in agents)
            {
                if (agent.isDiscovered || agentsByWin[agent.id])
                    Console.WriteLine($"Agent ID: {agent.id}, Rank: {agent.rank}, Capacity: {agent.capacity}, Find: {agentsByWin[agent.id]} in percent: {agent.foundCount*100/agent.capacity} %");
                else
                    Console.WriteLine($"Agent ID: {agent.id}, Find: {agentsByWin[agent.id]}");
            }
        }

        private int GetValidAgentId()
        {
            int agentId;
            while (true)
            {
                Console.Write("Enter agent ID: ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out agentId) && agentId >= 0 && !agentManager.agentByWin[agentId])
                {
                    break;
                }

                Console.WriteLine("Invalid ID or already found. Please enter a valid agent ID.");
            }

            return agentId;
        }


        private bool CheckIfGameOver()
        {
            return agentManager.agentByWin.Values.All(found => found);
        }

        public void ExitGame()
        {
            Console.WriteLine("Exiting the game. Goodbye!");
            Environment.Exit(0);
        }

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

        private void HandleCounterAttackSingleSensorLoss(Agent iranianAgent)
        {
            if (iranianAgent.notCounterAttack > 0)
            {
                iranianAgent.notCounterAttack--;
                Console.WriteLine("You have been attacked by the enemy. You lost one notCounterAttack point.");
                return;
            }

            var sensors = sensorManager.sensors.Where(s => sensorManager.sensorsByAgent[iranianAgent.id].Contains(s.Id)).ToList();


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

            var sensors = sensorManager.sensors.Where(s => sensorManager.sensorsByAgent[iranianAgent.id].Contains(s.Id)).ToList();
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

            var sensors = sensorManager.sensors.Where(s => sensorManager.sensorsByAgent[iranianAgent.id].Contains(s.Id)).ToList();
            foreach (var sensor in sensors)
            {
                iranianAgent.sensorsCopy.Add(sensor.type);
            }
            sensors.Clear();
            iranianAgent.foundCount = 0;
            Console.WriteLine("All your sensors have been removed by the enemy attack.");
        }

        public bool FindSensors(Agent iranianAgent)
        {
            counterRound++;
            CounterAttack(iranianAgent);

            if (!sensorManager.sensorsByAgent.ContainsKey(iranianAgent.id))
            {
                sensorManager.AddAgent(iranianAgent.id);
            }

            bool isFind = false;

            while (!isFind)
            {
                Sensor currentSensor = PromptSensorSelection();

                if (currentSensor == null)
                {
                    Console.WriteLine("Invalid sensor type. Please try again.");
                    continue;
                }

                if (!sensorManager.sensorsByAgent.TryGetValue(iranianAgent.id, out var agentSensors))
                {
                    Console.WriteLine("Error: Agent not found in sensor manager");
                    return false;
                }
                int response = currentSensor.ActivateSensor(iranianAgent, sensorManager.GetSensorsByAgent(iranianAgent.id));
                if (response > 0)
                {
                    sensorDB.UpdateSensorActivateCount(response, iranianAgent.foundCount);
                    sensorDB.UpdateSensorStatus(response, currentSensor.IsActive);
                }

                else if (response == 0)
                {
                    iranianAgent.foundCount++;

                    bool updateSuccess = agentDB.UpdateAgentFoundCount(iranianAgent.id, iranianAgent.foundCount);

                    agentDB.UpdateAgentDiscoveryStatus(iranianAgent.id, iranianAgent.isDiscovered);
                    agentDB.UpdateAgentNotCounterAttack(iranianAgent.id, iranianAgent.notCounterAttack);
                    agentDB.UpdateAgentSensorsCopy(iranianAgent.id, iranianAgent.sensorsCopy);





                    if (!updateSuccess)
                    {
                        Console.WriteLine("Warning: Could not update agent's found count in database");
                    }



                    bool createSensorId = sensorDB.CreateSensor(currentSensor, iranianAgent.id);



                    if (createSensorId)
                    {
                        bool sensorUpdateSuccess = sensorDB.UpdateSensorStatus(currentSensor.Id, currentSensor.IsActive);
                        if (!sensorUpdateSuccess)
                        {
                            Console.WriteLine($"Warning: Could not update sensor {currentSensor.Id} status in database");
                        }
                    }

                    sensorManager.AddSensor(iranianAgent.id, currentSensor);

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
                    agentManager.agentByWin[iranianAgent.id] = true;

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
