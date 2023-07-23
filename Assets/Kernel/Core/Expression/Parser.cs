using System;
using System.Collections.Generic;
using Kernel.Core;

namespace Kernel.Scripts
{
	public abstract class Parser
	{
		public IExpression BuildExpression(string expression)
		{
#if UNITY_EDITOR
			try
#endif
			{
				if(!string.IsNullOrEmpty(expression))
				{
					var infix = _LexicalInfix(expression);
					var postfix = _Infix2Postfix(infix);
					return _ParseExpression(postfix);
				}
			}
#if UNITY_EDITOR
			catch(Exception)
			{
                LoggerX.Error("@策划，表达式: {0} 非法", expression);
			}
#endif
			return null;
		}

		protected abstract List<Token> _Infix2Postfix(List<Token> infix);
		protected abstract IExpression _ParseExpression(List<Token> postfix);


		protected string _ParseInt(string expression, ref int pos)
		{
			var token = "";
			while(pos < expression.Length)
			{
				if('0' <= expression[pos] && expression[pos] <= '9')
				{
					token += expression[pos++];
				}
				else
				{
					break;
				}
			}
			return token;
		}

		protected abstract Token _ParseNextToken(string expression, ref int pos);


		protected void _SkipSpace(string expression, ref int pos)
		{
			while(pos < expression.Length)
			{
				if(expression[pos] == ' ' ||
				   expression[pos] == '\t' ||
				   expression[pos] == '\n' ||
				   expression[pos] == '\r')
				{
					++pos;
				}
				else
				{
					break;
				}
			}
		}

		private List<Token> _LexicalInfix(string expression)
		{
			var infix = new List<Token>();
			var pos = 0;
			if(!string.IsNullOrEmpty(expression))
			{
				while(pos < expression.Length)
				{
					infix.Add(_ParseNextToken(expression, ref pos));
				}
			}
			return infix;
		}
	}
}