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

        public override bool ActivateSensor(IranianAgent iranianAgent, Dictionary<IranianAgent, bool> agentByWin)
        {
            if(base.ActivateSensor(iranianAgent,agentByWin))
            {
                Console.WriteLine($"you have revealed a sensor: {iranianAgent.sensorsCopy[0]}");
                return true;
            }
            return false;
        }
       
    }
}


