using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestigationGame.Agents
{
    internal class SquadLeader : Agent
    {
        public SquadLeader(int id) : base(id)
        {
            rank = 2; 
            this.capacity = 4;
            RandomSensor();

        }
      
    }
    
}
