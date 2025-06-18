using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestigationGame.Personn.Agents
{
    internal class SquadLeader : Agent
    {
        public SquadLeader() : base()
        {
            rank = 2; 
            capacity = 4;
            RandomSensor();

        }
      
    }
    
}
