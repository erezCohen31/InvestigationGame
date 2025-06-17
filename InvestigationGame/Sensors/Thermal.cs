using InvestigationGame.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestigationGame.Sensors
{
    internal class Thermal : Sensor
    {
        public Thermal() : base()
        {
            this.type = "Thermal";
        }

        public override bool ActivateSensor(Agent iranianAgent, List<Sensor> sensorsByAgent)
        {
            if (base.ActivateSensor(iranianAgent, sensorsByAgent))
            {
                if (iranianAgent.sensorsCopy.Count > 0)
                {
                    Console.WriteLine($"You have revealed another sensor: {iranianAgent.sensorsCopy[0]}");
                }
                else
                {
                    Console.WriteLine("No more sensors to reveal.");
                }
                return true;
            }
            return false;
        }
    }
}
