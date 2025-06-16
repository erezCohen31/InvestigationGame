using InvestigationGame.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace InvestigationGame.Agents
{
    enum SensorType
    {
        Audio,
        Thermal,
        Pulse,
        Motion,
        Magnetic,
        Signal,
        Light
    }

    internal class FootSoldier : Agent
    {


        public FootSoldier(int id): base(id)
        {
            rank =1;
            this.capacity = 2;
        }

        public int RandomRank()
        {
            Random random = new Random();
            return random.Next(1, 4);
        }


        public void RandomSensor()
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







    }
}
