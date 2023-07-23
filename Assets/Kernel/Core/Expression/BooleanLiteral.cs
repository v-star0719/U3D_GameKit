namespace Kernel.Scripts
{
	public class BooleanLiteral : IBooleanExpression
	{
		public bool Value;

		public bool GetValue(IBooleanProvider provider)
		{
			return Value;
		}
	}
}