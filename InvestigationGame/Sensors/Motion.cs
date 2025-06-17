using InvestigationGame.Agents;

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
        public override bool ActivateSensor(Agent iranianAgent, List<Sensor> sensorsByAgent)
        {
            try
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while activating the Motion sensor: {ex.Message}");
            }
            return false;
        }

    }
}

