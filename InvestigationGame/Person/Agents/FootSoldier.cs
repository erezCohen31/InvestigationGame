using InvestigationGame.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace InvestigationGame.Person.Agents
{
    enum SensorType
    {
        Audio,
        Thermal,
        Pulse,
        Motion,
        Magnetic,
        Signal,
        Light
    }

    internal class FootSoldier : Agent
    {


        public FootSoldier() : base()
        {
            rank =1;
            capacity = 2;
            RandomSensor();
        }



      






    }
}
