
namespace Kernel.Core
{
    public class BTNodeInvert : BTNode
    {
        public BTNode child;

        public BTNodeInvert(BehaviorTree tree, BTNode child) : base(tree)
        {
            this.child = child;
        }

        public override BTNodeStatus Tick(float deltaTime)
        {
            var rt = child.Tick(deltaTime);
            if (rt == BTNodeStatus.Fail)
            {
                return BTNodeStatus.Success;
            }
            else if (rt == BTNodeStatus.Success)
            {
                return BTNodeStatus.Fail;
            }
            return BTNodeStatus.Running;
        }
    }
}

