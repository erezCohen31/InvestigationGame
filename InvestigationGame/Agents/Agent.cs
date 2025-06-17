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
        public Agent()
        {
            foundCount = 0;
            notCounterAttack = 0;
            sensors = new List<string>();
            sensorsCopy = new List<string>();
        }
        public void RandomSensor()
        {
            try
            {
                Random random = new Random();
                var sensorTypes = Enum.GetValues(typeof(SensorType)).Cast<SensorType>().ToList();

                for (int i = 0; i < capacity; i++)
                {
                    SensorType randomType = sensorTypes[random.Next(sensorTypes.Count)];
                    sensors.Add(randomType.ToString());
                }

                UpdateSensorsCopy();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while assigning sensors: {ex.Message}");
            }
        }



        
        public void UpdateSensorsCopy()
        {
            sensorsCopy = new List<string>(sensors);
        }
        
    }
}
