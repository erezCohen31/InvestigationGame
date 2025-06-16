using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestigationGame.Sensors
{
    internal class Pulse : Sensor
    {
        public Pulse() : base()
        {
            this.type = "Pulse";
            maxActivateCount = 3;
        }

        public override bool ActivateSensor(IranianAgent iranianAgent, Dictionary<IranianAgent, bool> agentByWin)
        {
            if (base.ActivateSensor(iranianAgent, agentByWin))
            {
                return true;
            }
            else if (agentByWin[iranianAgent] && activateCount < maxActivateCount)
            {
                activateCount++;
                Console.WriteLine($"You have activated the Pulse sensor {activateCount} times. You can activate it {maxActivateCount - activateCount} more times.");
                return false;
            }
            else if (activateCount >= maxActivateCount)
            {
                Console.WriteLine("You have reached the maximum activation count for the Pulse sensor.");
                isActive = false;
                iranianAgent.foundCount--;
                return false;
            }




            return false;
        }
    }

}
