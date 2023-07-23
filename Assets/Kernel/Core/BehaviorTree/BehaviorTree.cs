using System.Collections.Generic;

namespace Kernel.Core
{
    public class BehaviorTree
    {
        public BTNode root;
        public List<BTNode> runningNodes = new List<BTNode>();
        private List<BTNode> lastRuninnNodes = new List<BTNode>();
        public Blackboard blackboard = new Blackboard();

        public void Tick(float deltaTime)
        {
            lastRuninnNodes.AddRange(runningNodes);
            runningNodes.Clear();
            root.Tick(deltaTime);
            foreach (var node in lastRuninnNodes)
            {
                if (!runningNodes.Contains(node))
                {
                    node.Exist();
                }
            }
            lastRuninnNodes.Clear();
        }
    }
}

