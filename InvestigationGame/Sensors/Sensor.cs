using InvestigationGame.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestigationGame.Sensors
{
    internal abstract class Sensor
    {
        private int id;
        public string type;
        protected bool isActive;
        protected int activateCount;
        protected int maxActivateCount;

        public Sensor()
        {

            this.isActive = false;
            this.activateCount = 0;
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        public bool IsActive
        {
            get { return isActive; }
            set { isActive = value; }
        }
        public int ActivateCount
        {
            get { return activateCount; }
            set { activateCount = value; }
        }

        public virtual bool ActivateSensor(Agent iranianAgent, List<Sensor> sensorsByAgent)
        {
            try
            {
                string toRemove = null;
                foreach (string sensorName in iranianAgent.sensorsCopy)
                {
                    if (sensorName == this.type)
                    {
                        toRemove = sensorName;
                        this.isActive = true;
                        activateCount++;
                        break;
                    }
                }

                if (toRemove != null)
                {
                    iranianAgent.sensorsCopy.Remove(toRemove);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while activating the sensor '{type}': {ex.Message}");
            }

            return false;
        }





    }
}
