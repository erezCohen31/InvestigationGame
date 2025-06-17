using InvestigationGame.Agents;
using InvestigationGame.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InvestigationGame
{
    internal class GameManager
    {
        SensorManager sensorManager;
        AgentManager agentManager;
        int counterRound;

        public GameManager()
        {
            Console.WriteLine("Game Manager initialized. Ready to start the game.");
            sensorManager = new SensorManager();
            agentManager = new AgentManager();
            counterRound = 0;
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
                InitializeAgents();
                Console.WriteLine("Game is starting...");
                ChooseAgentLoop();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private void InitializeAgents()
        {
            Random random = new Random();
            for (int i = 0; i < 2; i++)
            {
                int rank = random.Next(1, 5);
                Agent agent = rank switch
                {
                    1 => new FootSoldier(i),
                    2 => new SquadLeader(i),
                    3 => new SeniorCommander(i),
                    4 => new OraganizationLeader(i),
                    _ => throw new Exception("Rank out of bounds")
                };
                agentManager.agentByWin.Add(agent, false);
                sensorManager.AddAgent(i);
            }
        }

        private void ChooseAgentLoop()
        {
            bool isGameOver = false;

            while (!isGameOver)
            {
                DisplayAgentsStatus();

                int agentId = GetValidAgentId();

                Agent chosenAgent = agentManager.agentByWin.Keys.ElementAt(agentId);

                Console.WriteLine($"You have chosen Agent {chosenAgent.id}.");

                FindSensors(chosenAgent);

                isGameOver = CheckIfGameOver();

                if (isGameOver)
                    ExitGame();
            }
        }

        private void DisplayAgentsStatus()
        {
            var agentByWin = agentManager.agentByWin;
            foreach (var agent in agentByWin.Keys)
            {
                if (agent.isDiscovered || agentByWin[agent])
                    Console.WriteLine($"Agent ID: {agent.id}, Rank: {agent.rank}, Capacity: {agent.capacity}, Find: {agentByWin[agent]} in percent: {agent.foundCount*100/agent.capacity} %");
                else
                    Console.WriteLine($"Agent ID: {agent.id}, Find: {agentByWin[agent]}");
            }
        }

        private int GetValidAgentId()
        {
            int agentId;
            while (!int.TryParse(Console.ReadLine(), out agentId) || agentId < 0 || agentId >= agentManager.agentByWin.Count)
            {
                Console.WriteLine($"Invalid ID. Please enter a valid agent ID (0-{agentManager.agentByWin.Count - 1}):");
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

            var sensors = sensorManager.sensorsByAgent[iranianAgent.id];
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

            var sensors = sensorManager.sensorsByAgent[iranianAgent.id];
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

            var sensors = sensorManager.sensorsByAgent[iranianAgent.id];
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

            bool isFind = false;

            while (!isFind)
            {
                Sensor currentSensor = PromptSensorSelection();

                if (currentSensor == null)
                {
                    Console.WriteLine("Invalid sensor type. Please try again.");
                    continue;
                }

                if (currentSensor.ActivateSensor(iranianAgent, sensorManager.sensorsByAgent[iranianAgent.id]))
                {
                    iranianAgent.foundCount++;
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
                    agentManager.agentByWin[iranianAgent] = true;
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
