namespace Kernel.Scripts
{
	public enum TokenType
	{
		NONE,
		CONST,
		VARIABLE,
		ADD_OPERATOR,
		SUB_OPERATOR,
		MUL_OPERATOR,
		DIV_OPERATOR,
		RELATIONAL_OPERATOR,
		LOGICAL_OPERATOR,

		BOOLEAN_LITERAL,
		NOT_OPERATOR,
		AND_OPERATOR,
		OR_OPERATOR,

		LEFT_PAREN,
		RIGHT_PAREN
	}

	public class Token
	{
		public TokenType TokenType = TokenType.NONE;
		public string Value = "";

		public OperatorType OperatorType
		{
			get
			{
				switch(TokenType)
				{
					case TokenType.ADD_OPERATOR: return OperatorType.ADD;
					case TokenType.SUB_OPERATOR: return OperatorType.SUB;
					case TokenType.MUL_OPERATOR: return OperatorType.MUL;
					case TokenType.DIV_OPERATOR: return OperatorType.DIV;
					case TokenType.NOT_OPERATOR: return OperatorType.NOT;
					case TokenType.AND_OPERATOR: return OperatorType.AND;
					case TokenType.OR_OPERATOR: return OperatorType.OR;

					case TokenType.RELATIONAL_OPERATOR:
					{
						switch(Value)
						{
							case "<": return OperatorType.LESS;
							case "<=": return OperatorType.L_EQUAL;
							case "==": return OperatorType.EQUAL;
							case ">": return OperatorType.GREATER;
							case ">=": return OperatorType.G_EQUAL;
							case "!=": return OperatorType.NOT_EQUAL;
						}
						return OperatorType.NONE;
					}

					case TokenType.LOGICAL_OPERATOR:
					{
						switch(Value)
						{
							case "||": return OperatorType.OR;
							case "&&": return OperatorType.AND;
							case "^": return OperatorType.XOR;
						}
						return OperatorType.NONE;
					}
				}
				return OperatorType.NONE;
			}
		}

		public int Priority
		{
			get
			{
				switch(TokenType)
				{
					case TokenType.CONST:
					case TokenType.VARIABLE:
					case TokenType.BOOLEAN_LITERAL:
					{
						return 1;
					}

					case TokenType.LEFT_PAREN:
					case TokenType.RIGHT_PAREN:
					{
						return 2;
					}

					case TokenType.LOGICAL_OPERATOR: return 3;
					case TokenType.RELATIONAL_OPERATOR: return 4;

					case TokenType.ADD_OPERATOR:
					case TokenType.SUB_OPERATOR:
					{
						return 5;
					}

					case TokenType.MUL_OPERATOR:
					case TokenType.DIV_OPERATOR:
					{
						return 6;
					}

					case TokenType.OR_OPERATOR: return 7;
					case TokenType.AND_OPERATOR: return 8;
					case TokenType.NOT_OPERATOR: return 9;
				}
				return 0;
			}
		}
	}
}