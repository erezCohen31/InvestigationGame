using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestigationGame.Agents
{
    internal class SeniorCommander:Agent
    {
        public SeniorCommander() : base()
        {
            rank = 3;
            this.capacity = 6;
            RandomSensor();

        }
    
    }
   
}
