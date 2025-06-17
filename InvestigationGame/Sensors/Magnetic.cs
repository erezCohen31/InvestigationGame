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

        public override bool ActivateSensor(Agent iranianAgent, List<Sensor> sensorsByAgent )
        {
            try
            {
                if (base.ActivateSensor(iranianAgent, sensorsByAgent))
                {
                    iranianAgent.notCounterAttack +=2;
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while activating the Magnetic sensor: {ex.Message}");
            }
            return false;
        }
    }

}
