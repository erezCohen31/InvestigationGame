using InvestigationGame.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestigationGame.Sensors
{
    internal class Light: Sensor
    {
        public Light() : base()
        {
            this.type = "Light";
        }

        public override int ActivateSensor(Agent iranianAgent, List<Sensor> sensorsByAgent)
        {
            try
            {
                if (base.ActivateSensor(iranianAgent, sensorsByAgent)==0)
                {
                    iranianAgent.isDiscovered = true;
                    Console.WriteLine($"You have discovered the rank of Iranian agent: {iranianAgent.rank}");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while activating the Light sensor: {ex.Message}");
            }
            return -1;
        }
    }
    
}
