namespace Kernel.Scripts
{
	public class BooleanLogicalExpression : IBooleanExpression
	{
		public IBooleanExpression Expression1;
		public IBooleanExpression Expression2;
		public OperatorType Operator = OperatorType.AND;

		public bool GetValue(IBooleanProvider provider)
		{
			if(Expression1 != null && Expression2 != null)
			{
				switch(Operator)
				{
					case OperatorType.OR: return Expression1.GetValue(provider) || Expression2.GetValue(provider);
					case OperatorType.AND: return Expression1.GetValue(provider) && Expression2.GetValue(provider);
					case OperatorType.XOR: return Expression1.GetValue(provider) ^ Expression2.GetValue(provider);
				}
			}
			else if(Expression1 != null && Operator == OperatorType.NOT)
			{
				return !Expression1.GetValue(provider);
			}

			return true;
		}
	}
}