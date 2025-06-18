using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestigationGame.Personn.Agents
{
    internal class SeniorCommander:Agent
    {
        public SeniorCommander() : base()
        {
            rank = 3;
            capacity = 6;
            RandomSensor();

        }
    
    }
   
}
