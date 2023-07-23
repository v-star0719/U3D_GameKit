using System.Collections.Generic;

namespace Kernel.Core
{
    //依次执行，直到出现一个成功的。效果就是依次尝试，直到成功。
    public class BTNodeParallel : BTNode
    {
        public List<BTNode> nodes = new List<BTNode>();
        public int curIndex = 0;

        public BTNodeParallel(BehaviorTree tree, params BTNode[] nodes) : base(tree)
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

        //如果有running的，返回running
        public override BTNodeStatus Tick(float deltaTime)
        {
            var rt = BTNodeStatus.Success;
            foreach (var node in nodes)
            {
                var st = node.Tick(deltaTime);
                if (st == BTNodeStatus.Running)
                {
                    tree.runningNodes.Add(node);
                    rt = BTNodeStatus.Success;
                }
                else if(st == BTNodeStatus.Fail)
                {
                    rt = BTNodeStatus.Fail;
                }
            }

            return rt;
        }
    }
}
