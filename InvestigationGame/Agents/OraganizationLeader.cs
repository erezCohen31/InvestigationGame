using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestigationGame.Agents
{
    internal class OraganizationLeader:Agent
    {
        public OraganizationLeader(int id) : base(id)
        {
            rank = 4; 
            this.capacity = 8; 
        }
       
    }
    
}
