using InvestigationGame.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestigationGame.Manager
{
    internal class AgentManager
    {
        public Dictionary<int, bool> agentByWin;
        public List<Agent> agents;
        public AgentManager()
        {
            agentByWin = new Dictionary<int, bool>();
            agents = new List<Agent>();
        }
    }
}
