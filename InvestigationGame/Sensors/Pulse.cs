using InvestigationGame.Agents;
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

        public override bool ActivateSensor(Agent iranianAgent, List<Sensor> sensorsByAgent)
        {
            if (base.ActivateSensor(iranianAgent, sensorsByAgent))
            {
                return true;
            }
            foreach (Sensor sensor in sensorsByAgent)
            {
                if (sensor.type == this.type)
                {
                    if (sensor.ActivateCount >= maxActivateCount)
                    {
                        Console.WriteLine("You have reached the maximum activation count for the Pulse sensor.");
                        isActive = false;
                        iranianAgent.foundCount--;
                        return false;
                    }
                    sensor.ActivateCount++;
                    Console.WriteLine($"You have activated the Motion sensor {activateCount} times. You can activate it {maxActivateCount - activateCount} more times.");
                    return false;
                }
            }
            return false;
        }
    }

}
