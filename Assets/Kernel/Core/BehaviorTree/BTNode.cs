
namespace Kernel.Core
{
    public enum BTNodeStatus
    {
        Success,
        Fail,
        Running,
    }

    public class BTNode
    {
        public BTNode parent;
        public BehaviorTree tree;

        public BTNode(BehaviorTree tree = null)
        {
            this.tree = tree;
        }

        public virtual void Start()
        {

        }

        public virtual void Finish()
        {

        }

        public virtual void Exist()
        {

        }

        public virtual BTNodeStatus Tick(float deltaTime)
        {
            return BTNodeStatus.Success;
        }
    }
}

