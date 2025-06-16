using InvestigationGame.Agents;
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

        public override bool ActivateSensor(Agent iranianAgent, List<Sensor> sensorsByAgent)
        {
            if (base.ActivateSensor(iranianAgent, sensorsByAgent))
            {
                iranianAgent.isDiscovered = true;
                Console.WriteLine($"You have discovered the rank of Iranian agent: {iranianAgent.rank}");
                return true;
            }
            return false;
        }
    }
    
}
