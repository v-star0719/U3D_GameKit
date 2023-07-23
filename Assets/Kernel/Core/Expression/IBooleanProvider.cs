namespace Kernel.Scripts
{
    public interface IBooleanProvider : IProvider
    {
        bool GetVariableValue(int key);
    }
}
