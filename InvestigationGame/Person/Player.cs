using InvestigationGame.Person.Agents;
using System;

namespace InvestigationGame.Person
{
    public class Player
    {
        public int id;
        public string name;
        public int score;
        public DateTime createdAt;
        List<Agent> agents;

        public Player(string name, int score, DateTime createdAt)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.score = score;
            this.createdAt = createdAt;
            this.agents = new List<Agent>();
        }
        public Player(int id, string name, int score, DateTime createdAt)
        {
            this.id = id;
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.score = score;
            this.createdAt = createdAt;
            this.agents = new List<Agent>();
        }

        public void AddAgent(Agent agent)
        {
            if (agent == null)
            {
                throw new ArgumentNullException(nameof(agent), "Agent cannot be null");
            }
            
            agents.Add(agent);
        }

    }
}
