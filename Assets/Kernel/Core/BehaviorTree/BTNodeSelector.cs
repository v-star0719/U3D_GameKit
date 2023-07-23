using System.Collections.Generic;

namespace Kernel.Core
{
    //依次执行，直到出现一个成功的。效果就是依次尝试，直到成功。
    public class BTNodeSelector : BTNode
    {
        public List<BTNode> nodes = new List<BTNode>();
        public int curIndex = 0;

        public BTNodeSelector(BehaviorTree tree, params BTNode[] nodes) : base(tree)
        {
            foreach(var node in nodes)
            {
                AddNode(node);
            }
        }

        public void AddNode(BTNode node)
        {
            nodes.Add(node);
            node.parent = this;
            node.tree = tree;
        }

        public override void Start()
        {

        }

        public override void Finish()
        {

        }

        public override BTNodeStatus Tick(float deltaTime)
        {
            var n = nodes.Count;
            if (n == 0)
            {
                return BTNodeStatus.Success;
            }

            for (int i = curIndex; i < nodes.Count; i++)
            {
                var rt = nodes[i].Tick(deltaTime);
                if (rt == BTNodeStatus.Success)
                {
                    curIndex = 0;
                    return rt;
                }
                else if (rt == BTNodeStatus.Running)
                {
                    curIndex = i;
                    tree.runningNodes.Add(nodes[i]);
                    return BTNodeStatus.Success;
                }
            }

            curIndex = 0;
            return BTNodeStatus.Fail;
        }
    }
}
