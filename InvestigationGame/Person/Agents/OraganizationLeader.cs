using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestigationGame.Person.Agents
{
    internal class OraganizationLeader:Agent
    {
        public OraganizationLeader() : base()
        {
            rank = 4; 
            capacity = 8;
            RandomSensor();

        }
   
    }
    
}
