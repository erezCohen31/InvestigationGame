using InvestigationGame.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestigationGame.Manager
{
    internal class SensorManager
    {

        public Dictionary<int, List<int>> sensorsByAgent;
        public List<Sensor> sensors;
        public SensorManager()
        {
            sensorsByAgent = new Dictionary<int, List<int>>();
            sensors = new List<Sensor>();
        }

        public void AddSensor(int agentId, Sensor sensor)
        {
            AddAgent(agentId);

            sensorsByAgent[agentId].Add(sensor.Id);
            sensors.Add(sensor);


        }
        public void AddAgent(int agentId)
        {
            if (!sensorsByAgent.ContainsKey(agentId))
            {
                sensorsByAgent[agentId] = new List<int>();
            }

        }
        public List<Sensor> GetSensorsByAgent(int agentId)
        {
            if (sensorsByAgent.ContainsKey(agentId))
            {
                return sensors.Where(s => sensorsByAgent[agentId].Contains(s.Id)).ToList();
            }
            return new List<Sensor>();
        }

    }
}
