using System;
using System.Collections.Generic;

namespace Kernel.Scripts
{
    public class BooleanExpressionParser : Parser
    {
		private static readonly BooleanExpressionParser instance = new BooleanExpressionParser();

	    public static BooleanExpressionParser Instance
	    {
		    get
		    {
			    return instance;
		    }
	    }

	    protected override List<Token> _Infix2Postfix(List<Token> infix)
        {
            List<Token> postfix = new List<Token>();
            Stack<Token> operatorStack = new Stack<Token>();

			for (int i= 0; i< infix.Count; ++i)
            {
				var token = infix[i];
                switch (token.TokenType)
                {
                    case TokenType.LEFT_PAREN:
                    {
                        operatorStack.Push(token);
                        break;
                    }

                    case TokenType.RIGHT_PAREN:
                    {
                        while (operatorStack.Count > 0 &&
                               operatorStack.Peek().TokenType != TokenType.LEFT_PAREN)
                        {
                            postfix.Add(operatorStack.Pop());
                        }
                        if (operatorStack.Count > 0 &&
                            operatorStack.Peek().TokenType == TokenType.LEFT_PAREN)
                        {
                            operatorStack.Pop();
                        }
                        else
                        {
#if UNITY_EDITOR
                            throw new ArgumentException();
#endif
                        }
                        break;
                    }

                    case TokenType.NOT_OPERATOR:
                    case TokenType.AND_OPERATOR:
                    case TokenType.OR_OPERATOR:
                    {
                        while (operatorStack.Count > 0 &&
                               token.Priority <= operatorStack.Peek().Priority)
                        {
                            postfix.Add(operatorStack.Pop());
                        }
                        operatorStack.Push(token);
                        break;
                    }

                    default:
                    {
                        postfix.Add(token);
                        break;
                    }
                }
            }

            while (operatorStack.Count > 0)
            {
                postfix.Add(operatorStack.Pop());
            }

            return postfix;
        }

	    protected override IExpression _ParseExpression(List<Token> postfix)
        {
            Stack<IExpression> stack = new Stack<IExpression>();

			for (int i= 0; i< postfix.Count; ++i)
            {
				var token = postfix[i];
                switch (token.TokenType)
                {
                    case TokenType.AND_OPERATOR:
                    case TokenType.OR_OPERATOR:
                    {
                        var exp = new BooleanExpression();
                        var exp2 = stack.Pop() as IBooleanExpression;
                        var exp1 = stack.Pop() as IBooleanExpression;
                        if (exp1 != null && exp2 != null)
                        {
                            exp.Expression1 = exp1;
                            exp.Expression2 = exp2;
                            exp.Operator = token.OperatorType;
                        }
                        else
                        {
                            throw new ArgumentException();
                        }
                        stack.Push(exp);
                        break;
                    }
                    case TokenType.NOT_OPERATOR:
                    {
                        var exp = new BooleanExpression();
                        var exp1 = stack.Pop() as IBooleanExpression;
                        if (exp1 != null)
                        {
                            exp.Expression1 = exp1;
                            exp.Operator = token.OperatorType;
                        }
                        else
                        {
                            throw new ArgumentException();
                        }
                        stack.Push(exp);
                        break;
                    }

                    case TokenType.BOOLEAN_LITERAL:
                    {
                        var exp = new BooleanLiteral();
                        exp.Value = bool.Parse(token.Value);
                        stack.Push(exp);
                        break;
                    }

                    case TokenType.VARIABLE:
                    {
                        var exp = new BooleanVariable();
                        exp.Key = int.Parse(token.Value);
                        stack.Push(exp);
                        break;
                    }
                }
            }

            if (stack.Count > 0)
            {
                return stack.Pop();
            }
            return null;
        }

	    protected override Token _ParseNextToken(string expression, ref int pos)
        {
            _SkipSpace(expression, ref pos);
            Token token = new Token();
            if (pos < expression.Length)
            {
                if (expression[pos] == '(')
                {
                    token.TokenType = TokenType.LEFT_PAREN;
                    token.Value = "(";
                    ++pos;
                }
                else if (expression[pos] == ')')
                {
                    token.TokenType = TokenType.RIGHT_PAREN;
                    token.Value = ")";
                    ++pos;
                }
                else if ('0' <= expression[pos] && expression[pos] <= '9')
                {
                    token.TokenType = TokenType.VARIABLE;
                    token.Value = _ParseInt(expression, ref pos);
                }
                else if (expression[pos] == '&')
                {
                    token.TokenType = TokenType.AND_OPERATOR;
                    token.Value = "&";
                    ++pos;
                }
                else if (expression[pos] == '|')
                {
                    token.TokenType = TokenType.OR_OPERATOR;
                    token.Value = "|";
                    ++pos;
                }
                else if (expression[pos] == '!')
                {
                    token.TokenType = TokenType.NOT_OPERATOR;
                    token.Value = "!";
                    ++pos;
                }
                else if (_ParseBooleanLiteral(expression, ref pos, ref token.Value))
                {
                    token.TokenType = TokenType.BOOLEAN_LITERAL;
                }
                else
                {
#if UNITY_EDITOR
                    throw new ArgumentException();
#else
                    ++pos;
#endif
                }
            }
            return token;
        }

        private bool _ParseBooleanLiteral(string expression, ref int pos, ref string token)
        {
            if (expression[pos] == 't' && pos <= expression.Length - 3)
            {
                if ( expression[pos + 1] == 'r'
                    && expression[pos + 2] == 'u'
                    && expression[pos + 3] == 'e')
                {
                    token = "true";
                    pos += 4;
                    return true;
                }
            }
            else if (expression[pos] == 'f' && pos < expression.Length - 4)
            {
                if ( expression[pos + 1] == 'a'
                    && expression[pos + 2] == 'l'
                    && expression[pos + 3] == 's'
                    && expression[pos + 4] == 'e')
                {
                    token = "false";
                    pos += 5;
                    return true;
                }
            }
            return false;
        }
    }
}
