using InvestigationGame.Personn.Agents;

namespace InvestigationGame.Sensors
{

    // Represents a motion sensor in the game   
    internal class Motion : Sensor
    {
        public Motion() : base()
        {
            this.type = "Motion";
            maxActivateCount = 3;
        }
        public override int ActivateSensor(Agent iranianAgent, List<Sensor> sensorsByAgent)
        {
            try
            {
                if (base.ActivateSensor(iranianAgent, sensorsByAgent)==0)
                {
                    return 0;
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
                            return sensor.Id;
                        }
                        sensor.ActivateCount++;
                        Console.WriteLine($"You have activated the Motion sensor {activateCount} times. You can activate it {maxActivateCount - activateCount} more times.");
                        return sensor.Id;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while activating the Motion sensor: {ex.Message}");
            }
            return -1;
        }

    }
}

