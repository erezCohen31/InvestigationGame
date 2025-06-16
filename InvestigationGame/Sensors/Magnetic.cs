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

        public override bool ActivateSensor(IranianAgent iranianAgent, List<Sensor> sensorsByAgent)
        {
            if (base.ActivateSensor(iranianAgent, sensorsByAgent))
            {
                iranianAgent.notCounterAttack +=2;
                return true;
            }
            return false;
        }
    }

}
