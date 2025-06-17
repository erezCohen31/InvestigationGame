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

       public Dictionary<int, List<Sensor>> sensorsByAgent;
        public SensorManager()
        {
            sensorsByAgent = new Dictionary<int, List<Sensor>>();
        }

        public void AddSensor(int agentId,Sensor sensor)
        {
            AddAgent(agentId);
            
            sensorsByAgent[agentId].Add(sensor);
           

        }
        public void AddAgent(int agentId)
        {
            if (!sensorsByAgent.ContainsKey(agentId))
            {
                sensorsByAgent[agentId] = new List<Sensor>();
            }

        }
        public List<Sensor> GetSensorsByAgent(int agentId)
        {
            if (sensorsByAgent.ContainsKey(agentId))
            {
                return sensorsByAgent[agentId];
            }
            return new List<Sensor>();
        }

    }
}
