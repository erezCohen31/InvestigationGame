using InvestigationGame.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestigationGame.Sensors
{
    internal class Magnetic : Sensor
    {
        public Magnetic() : base()
        {
            this.type = "Magnetic";
        }

        public override int ActivateSensor(Agent iranianAgent, List<Sensor> sensorsByAgent )
        {
            try
            {
                if (base.ActivateSensor(iranianAgent, sensorsByAgent) == 0)
                {
                    iranianAgent.notCounterAttack +=2;
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while activating the Magnetic sensor: {ex.Message}");
            }
            return -1;
        }
    }

}
