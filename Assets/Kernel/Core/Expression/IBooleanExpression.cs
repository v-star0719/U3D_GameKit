namespace Kernel.Scripts
{
    public interface IBooleanExpression : IExpression
    {
        bool GetValue(IBooleanProvider provider);
    }
}
