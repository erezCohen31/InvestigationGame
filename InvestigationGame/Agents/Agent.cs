using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestigationGame.Agents
{
    internal class Agent
    {
        public int id;
        public int rank;
        public List<string> sensors;
        public List<string> sensorsCopy;
        public bool isDiscovered = false;
        public int capacity;
        public int foundCount;
        public int notCounterAttack;
        public Agent(int id)
        {
            this.id = id;
            foundCount = 0;
            notCounterAttack = 0;
            sensors = new List<string>();


        }
        public void RandomSensor()
        {
            try
            {
                Random random = new Random();
                for (int i = 0; i < capacity; i++)
                {
                    SensorType sensorType = (SensorType)random.Next(0, Enum.GetValues(typeof(SensorType)).Length);
                    switch (sensorType)
                    {
                        case SensorType.Audio:
                            sensors.Add("Audio");
                            break;
                        case SensorType.Thermal:
                            sensors.Add("Thermal");
                            break;
                        case SensorType.Pulse:
                            sensors.Add("Pulse");
                            break;
                        case SensorType.Motion:
                            sensors.Add("Motion");
                            break;
                        case SensorType.Magnetic:
                            sensors.Add("Magnetic");
                            break;
                        case SensorType.Signal:
                            sensors.Add("Signal");
                            break;
                        case SensorType.Light:
                            sensors.Add("Light");
                            break;
                    }
                }
                sensorsCopy = new List<string>(sensors);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while assigning sensors: {ex.Message}");
            }
        }


    }
}
