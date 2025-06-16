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

        public override bool ActivateSensor(IranianAgent iranianAgent, List<Sensor> sensorsByAgent)
        {
            if(base.ActivateSensor(iranianAgent,sensorsByAgent))
            {
                Console.WriteLine($"you have revealed a sensor: {iranianAgent.sensorsCopy[0]}");
                return true;
            }
            return false;
        }
       
    }
}


