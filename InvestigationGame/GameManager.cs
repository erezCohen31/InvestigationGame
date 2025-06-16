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
        IranianAgent iranianAgent;
        bool isWin;
        Dictionary<IranianAgent, bool> agentByWin;
        SensorManager sensorManager;

        public GameManager()
        {
            // Initialize the game manager
            Console.WriteLine("Game Manager initialized. Ready to start the game.");
            isWin = false;
            agentByWin= new Dictionary<IranianAgent, bool>();
            sensorManager = new SensorManager();
        }
        public void Menu()
        {
            // Initialize the game menu

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
        public void StartGame()
        {
            // Logic to start the game
            for (int i = 0; i<2; i++)
            {
                agentByWin.Add(new IranianAgent(i), false);
                sensorManager.AddAgent(i);
            }
            Console.WriteLine("Game is starting...");
            chooseAgent();

            // Here you would typically initialize game state, load resources, etc.
        }
        public void chooseAgent()
        {
            bool isTerminate = false;
            // Logic to choose an agent
            while (!isTerminate)
            {

                isTerminate = true;
                Console.WriteLine($"Choose an agent by entering their ID:");
                int agentId;
                foreach (var agent in agentByWin.Keys)
                {
                    Console.WriteLine($"Agent ID: {agent.id}, Rank: {agent.rank}, Capacity: {agent.capacity}, Find: {agentByWin[agent]}");
                    if (agentByWin[agent]== false)
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
        }

        public void ExitGame()
        {
            // Logic to exit the game
            Console.WriteLine("Exiting the game. Goodbye!");
            Environment.Exit(0);
        }
        public bool FindSensors(IranianAgent iranianAgent)
        {
            bool isFind = false;

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
                    agentByWin[iranianAgent] = true;
                    Console.WriteLine("You found all sensors!");
                }


            }
            return isFind;

        }

    }
}
