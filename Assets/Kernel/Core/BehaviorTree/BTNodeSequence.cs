using System.Collections.Generic;

namespace Kernel.Core
{
    //依次执行，直到失败。
    public class BTNodeSequence : BTNode
    {
        public List<BTNode> nodes = new List<BTNode>();
        public int curIndex = 0;

        public BTNodeSequence(BehaviorTree tree, params BTNode[] nodes) : base(tree)
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
            if(n == 0)
            {
                return BTNodeStatus.Success;
            }

            for(int i = curIndex; i < nodes.Count; i++)
            {
                var rt = nodes[i].Tick(deltaTime);
                if(rt == BTNodeStatus.Fail)
                {
                    curIndex = 0;
                    return rt;
                }
                else if(rt == BTNodeStatus.Running)
                {
                    curIndex = i;
                    tree.runningNodes.Add(nodes[i]);
                    return BTNodeStatus.Success;//返回成功，如果返回运行中，上层会直接执行这个节点，造成死锁。需要在进行这个行为的时候，还判断其他行为是否进行。
                }
            }

            curIndex = 0;
            return BTNodeStatus.Success;
        }
    }
}
