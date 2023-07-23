namespace Kernel.Scripts
{
    public class BooleanVariable : IBooleanExpression
    {
        public bool GetValue(IBooleanProvider provider)
        {
            return provider.GetVariableValue(Key);
        }

        public int Key;
    }
}
