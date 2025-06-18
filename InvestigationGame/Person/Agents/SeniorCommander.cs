using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestigationGame.Person.Agents
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
