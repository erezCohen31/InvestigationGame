using InvestigationGame.Personn.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestigationGame.Sensors
{
    internal class Signal: Sensor
    {
        public Signal() : base()
        {
            this.type = "Signal";
        }

        public override int ActivateSensor(Agent iranianAgent, List<Sensor> sensorsByAgent)
        {
            if (base.ActivateSensor(iranianAgent, sensorsByAgent) == 0)
            {
                iranianAgent.isDiscovered = true;
                Console.WriteLine($"You have discovered the rank of Iranian agent: {iranianAgent.rank}");
                return 0;
            }
            return -1;
        }
    }
    
}
