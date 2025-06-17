using InvestigationGame.Agents;
using InvestigationGame.Sensors;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestigationGame
{
    internal class GameManager
    {
        SensorManager sensorManager;
        AgentManager agentManager;
        int counterRound;

        public GameManager()
        {
            // Initialize the game manager
            Console.WriteLine("Game Manager initialized. Ready to start the game.");
            sensorManager = new SensorManager();
            agentManager = new AgentManager();
            counterRound = 0;
        }
        public void Menu()
        {
            // Initialize the game menu
            try
            {

                Console.WriteLine("Welcome to the Investigation Game!");
                Console.WriteLine("Please select an option:");
                Console.WriteLine("1. Start Game");
                Console.WriteLine("2. Exit");
                switch (Console.ReadLine())
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
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Menu();
            }
        }
        public void StartGame()
        {
            // Logic to start the game
            try
            {
                for (int i = 0; i<2; i++)
                {
                    Random random = new Random();
                    int rank = random.Next(1, 5); // Random rank between 1 and 4
                    switch (rank)
                    {
                        case 1:
                            agentManager.agentByWin.Add(new FootSoldier(i), false);
                            break;
                        case 2:
                            agentManager.agentByWin.Add(new SquadLeader(i), false);
                            break;
                        case 3:
                            agentManager.agentByWin.Add(new SeniorCommander(i), false);
                            break;
                        case 4:
                            agentManager.agentByWin.Add(new OraganizationLeader(i), false);
                            break;
                    }
                    sensorManager.AddAgent(i);
                }
                Console.WriteLine("Game is starting...");
                chooseAgent();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        public void chooseAgent()
        {
            Agent iranianAgent = null;
            Dictionary<Agent, bool> agentByWin = agentManager.agentByWin;
            bool isTerminate = false;
            // Logic to choose an agent
            while (!isTerminate)
            {
                try
                {

                    isTerminate = true;
                    Console.WriteLine($"Choose an agent by entering their ID:");
                    int agentId;
                    foreach (var agent in agentByWin.Keys)
                    {
                        if (agent.isDiscovered)
                        {
                            Console.WriteLine($"Agent ID: {agent.id}, Rank: {agent.rank}, Capacity: {agent.capacity}, Find: {agentByWin[agent]}");

                        }
                        else { Console.WriteLine($"Agent ID: {agent.id}, Find: {agentByWin[agent]}"); }

                        if (agentManager.agentByWin[agent]== false)
                        {
                            isTerminate = false;
                        }
                    }
                    while (!int.TryParse(Console.ReadLine(), out agentId) || agentId < 0 || agentId >= agentByWin.Count)
                    {
                        Console.WriteLine("Invalid ID. Please enter a valid agent ID (0-9):");
                    }
                    iranianAgent = agentByWin.Keys.ElementAt(agentId);
                    Console.WriteLine($"You have chosen Agent {iranianAgent.id} with rank {iranianAgent.rank} and capacity {iranianAgent.capacity}.");

                    FindSensors(iranianAgent);
                    isTerminate = true;
                    foreach (var agent in agentByWin.Keys)
                    {
                        if (agentByWin[agent]== false)
                        {
                            isTerminate = false;
                        }
                    }
                    if (isTerminate)
                    {
                        ExitGame();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    isTerminate = false;

                }
            }
        }

        public void ExitGame()
        {
            // Logic to exit the game
            Console.WriteLine("Exiting the game. Goodbye!");
            Environment.Exit(0);
        }

        public void CounterAttack(Agent iranianAgent)
        {
            try
            {
                if (counterRound%3==0 && iranianAgent.rank== 2|| iranianAgent.rank==4)
                {
                    if (iranianAgent.notCounterAttack > 0)
                    {
                        iranianAgent.notCounterAttack--;
                        Console.WriteLine("You have been attacked by the enemy. You lost one notCounterAttack point.");
                    }
                    else
                    {
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
                }
                if (counterRound%3==0 && iranianAgent.rank== 3)
                {
                    if (iranianAgent.notCounterAttack > 0)
                    {
                        iranianAgent.notCounterAttack--;
                        Console.WriteLine("You have been attacked by the enemy. You lost one notCounterAttack point.");
                    }
                    else
                    {
                        var sensors = sensorManager.sensorsByAgent[iranianAgent.id];

                        for (int i = 0; i<2; i++)
                        {
                            if (sensors.Count > 0)
                            {
                                Random rnd = new Random();
                                int indexToRemove = rnd.Next(sensors.Count);
                                iranianAgent.sensorsCopy.Add(sensors[indexToRemove].type);
                                sensors.RemoveAt(indexToRemove);
                                iranianAgent.foundCount--;

                            }
                        }
                        Console.WriteLine("You have been attacked by the enemy. You lost 2 sensors");



                    }
                }
                if (counterRound%10==0 && iranianAgent.rank== 4)
                {
                    if (iranianAgent.notCounterAttack > 0)
                    {
                        iranianAgent.notCounterAttack--;
                        Console.WriteLine("You have been attacked by the enemy. You lost one notCounterAttack point.");
                    }
                    else
                    {
                        var sensors = sensorManager.sensorsByAgent[iranianAgent.id];
                        foreach (var sensor in sensors)
                        {
                            iranianAgent.sensorsCopy.Add(sensor.type);
                        }
                        sensors.Clear();
                        iranianAgent.foundCount=0;
                        Console.WriteLine("All your sensors have been removed by the enemy attack.");


                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during the counter-attack: {ex.Message}");
            }
        }
        public bool FindSensors(Agent iranianAgent)
        {
            bool isFind = false;
            counterRound++;
            CounterAttack(iranianAgent);
            try
            {

                while (!isFind)
                {
                    Sensor currentSensor = null;
                    Console.WriteLine("enter a sensor.");
                    Console.WriteLine("1. Audio");
                    Console.WriteLine("2. Thermal");
                    Console.WriteLine("3. Pulse");
                    Console.WriteLine("4. Motion");
                    Console.WriteLine("5. Magnetic");
                    Console.WriteLine("6. Signal");
                    Console.WriteLine("7. Light");

                    switch (Console.ReadLine())
                    {
                        case "1":
                            currentSensor = new Audio();
                            break;
                        case "2":
                            currentSensor = new Thermal();
                            break;
                        case "3":
                            currentSensor = new Pulse();
                            break;
                        case "4":
                            currentSensor =new Motion();
                            break;
                        case "5":
                            currentSensor =new Magnetic();
                            break;
                        case "6":
                            currentSensor =new Signal();
                            break;
                        case "7":
                            currentSensor =new Light();
                            break;
                        default:
                            Console.WriteLine("Invalid sensor type. Please try again.");
                            break;
                    }


                    if (currentSensor.ActivateSensor(iranianAgent, sensorManager.sensorsByAgent[iranianAgent.id]))
                    {
                        iranianAgent.foundCount++;
                        sensorManager.AddSensor(iranianAgent.id, currentSensor);

                        Console.WriteLine($"You found a {currentSensor.type} sensor!");
                        if (iranianAgent.isDiscovered)
                        {
                            Console.WriteLine($"you found{iranianAgent.foundCount}/{iranianAgent.capacity} ");
                        }

                    }
                    else
                    {
                        Console.WriteLine("You didn't find the sensor. Try again.");

                    }

                    if (iranianAgent.sensorsCopy.Count==0)
                    {
                        isFind = true;
                        agentManager.agentByWin[iranianAgent] = true;
                        Console.WriteLine("You found all sensors!");
                    }

                }
                return isFind;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

    }
}
