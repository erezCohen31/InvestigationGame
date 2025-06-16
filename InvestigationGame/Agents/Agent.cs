using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestigationGame.Agents
{
    internal class Agent
    {
        public int id;
        public int rank;
        public List<string> sensors;
        public List<string> sensorsCopy;
        public bool isDiscovered = false;
        public int capacity;
        public int foundCount;
        public int notCounterAttack;
        public Agent(int id)
        {
            this.id = id;
            foundCount = 0;
            notCounterAttack = 0;
            sensors = new List<string>();


        }


    }
}
