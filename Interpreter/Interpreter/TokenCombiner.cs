using System;
using System.Collections.Generic;
using System.Text;

namespace Shiro.Interpreter
{
	public class TokenCombiner
	{
		#region Helpers
		protected bool IsNumeric(Token t)
		{
			return t.vt == ValueType.Number || t.vt == ValueType.Bool;
		}
		protected bool IsList(Token t)
		{
			return t.vt == ValueType.List;
		}
		protected double GetNumber(Token t)
		{
			if (!IsNumeric(t))
				return 0;
			if (t.isNumericLong)
				return (double)long.Parse(t.token);

			if (t.vt == ValueType.Bool)
				if (t.token == "true")
					return 1;
				else
					return 0;
			else
				return double.Parse(t.token);
		}
		protected long GetLong(Token t)
		{
			if (!IsNumeric(t))
				return 0;
			if (!t.isNumericLong)
				return (long)double.Parse(t.token);

			if (t.vt == ValueType.Bool)
				if (t.token == "true")
					return 1;
				else
					return 0;
			else
				return long.Parse(t.token);
		}

		protected bool AreValidOperands(Token t1, Token t2)
		{
			if (t1.tt != TokenType.Value || t2.tt != TokenType.Value)
				return false;
			if (t1.vt == ValueType.Function || t2.vt == ValueType.Function)
				return false;

			return true;
		}

		#endregion

		#region Compound Conditionals

		public Token And(Token t1, Token t2)
		{
			Token retVal = new Token();
			retVal.tt = TokenType.Value;
			retVal.vt = ValueType.Bool;

			if (this.Not(this.Not(t1)).token == this.Not(this.Not(t2)).token)
				retVal.token = "true";
			else
				retVal.token = "false";

			return retVal;
		}

		public Token Or(Token t1, Token t2)
		{
			Token retVal = new Token();
			retVal.tt = TokenType.Value;
			retVal.vt = ValueType.Bool;

			if (t1.vt != ValueType.Bool || t2.vt != ValueType.Bool)
				Error.ReportError("And operator (&) cannot be applied to types other than Bool");

			if (this.Not(this.Not(t1)).token == "true" || this.Not(this.Not(t2)).token == "true")
				retVal.token = "true";
			else
				retVal.token = "false";

			return retVal;
		}
		#endregion

		#region Conditional Ops
		public Token GreaterThan(Token t1, Token t2)
		{
			Token retVal = new Token();
			retVal.tt = TokenType.Value;
			retVal.vt = ValueType.Bool;

			if (IsNumeric(t1))
			{
				if (IsNumeric(t2))
				{
					if (GetNumber(t1) > GetNumber(t2))
						retVal.token = "true";
					else
						retVal.token = "false";
				}
				else
				{
					if (t1.token.Length > t2.token.Length)
						retVal.token = "true";
					else
						retVal.token = "false";
				}
			}
			else
			{
				if (t1.token.Length > t2.token.Length)
					retVal.token = "true";
				else
					retVal.token = "false";
			}
			return retVal;
		}

		public Token LessThan(Token t1, Token t2)
		{
			Token retVal = new Token();
			retVal.tt = TokenType.Value;
			retVal.vt = ValueType.Bool;

			if (IsNumeric(t1))
			{
				if (IsNumeric(t2))
				{
					if (GetNumber(t1) < GetNumber(t2))
						retVal.token = "true";
					else
						retVal.token = "false";
				}
				else
				{
					if (t1.token.Length < t2.token.Length)
						retVal.token = "true";
					else
						retVal.token = "false";
				}
			}
			else
			{
				if (t1.token.Length < t2.token.Length)
					retVal.token = "true";
				else
					retVal.token = "false";
			}
			return retVal;
		}

		public Token EqualTo(Token t1, Token t2)
		{
			Token retVal = new Token();
			retVal.tt = TokenType.Value;
			retVal.vt = ValueType.Bool;

			if (t1.vt == ValueType.List)
			{
				if (t2.vt == ValueType.List)
				{
					if (t1.ToString() == t2.ToString())
						retVal.token = "true";
					else
						retVal.token = "false";
				}
				else
					retVal.token = "false";
			}
			else
			{
				if (t2.vt == ValueType.List)
					retVal.token = "false";
				else
				{
					if (t1.token == t2.token)
						retVal.token = "true";
					else
						retVal.token = "false";
				}
			}
			return retVal;
		}
		#endregion

		public Token Not(Token t)
		{
			Token retVal = new Token();
			retVal.tt = TokenType.Value;
			retVal.vt = ValueType.Bool;

			if (t.vt == ValueType.Bool)
				if (t.token == "true")
					retVal.token = "false";
				else
					retVal.token = "true";
			else if (t.token == "0" || t.token == "")
				retVal.token = "true";
			else
				retVal.token = "false";

			return retVal;
		}

		#region Mulops
		public Token Mod(Token t1, Token t2)
		{
			if (!IsNumeric(t1) || !IsNumeric(t2))
				Error.ReportError("Modulus operator (%) can only be applied to Numbers");

			double op1 = double.Parse(t1.token);
			double op2 = double.Parse(t2.token);
			double retVal = op1 % op2;

			return Token.FromString(retVal.ToString());
		}

		public Token Div(Token t1, Token t2)
		{
			Token retVal = new Token();
			retVal.tt = TokenType.Value;

			if (!AreValidOperands(t1, t2))
				Error.ReportError("Invalid Operation, cannot divide types " + t1.vt.ToString() + ", " + t2.vt.ToString());

			if (IsList(t1))
			{
				if (IsNumeric(t2))
				{
					retVal.vt = ValueType.List;
					retVal.list = new List<Token>();

					List<Token> selectFrom = new List<Token>(t1.list.ToArray());
					Random r = new Random();
					for (int i = 0; i < int.Parse(t2.token); i++)
					{
						retVal.list.Add(selectFrom[r.Next(0, selectFrom.Count -1)]);
					}
				}
				else if (IsList(t2))
				{
					retVal.vt = ValueType.List;
					retVal.list = new List<Token>();

					foreach (Token vs in t1.list)
						foreach (Token vs2 in t2.list)
							if (vs.token == vs2.token)
								retVal.list.Add(vs);
				}
				else
				{
					Error.ReportError("Cannot divide a List by a String");
				}
			}
			else
			{
				if (IsNumeric(t1))
				{
					if (IsNumeric(t2))
					{
						retVal.vt = ValueType.Number;
						retVal.token = (GetNumber(t1) / GetNumber(t2)).ToString();
					}
					else
					{
						Error.ReportError("Cannot divide a Number by a String");
					}
				}
				else
				{
					if (!IsNumeric(t2))
					{
						retVal.vt = ValueType.List;
						retVal.list = new List<Token>();
						string[] eles;

						if (IsList(t2))
						{
							List<string> seps = new List<string>();
							foreach (Token vs in t2.list)
								seps.Add(vs.token);
							eles = t1.token.Split(seps.ToArray(), StringSplitOptions.RemoveEmptyEntries);
						}
						else
						{
							eles = t1.token.Split(new string[] { t2.token }, StringSplitOptions.RemoveEmptyEntries);
						}

						foreach (string ele in eles)
						{
							Token vs = new Token();
							vs.vt = ValueType.String;
							vs.token = ele;
							vs.scope = t1.scope;
							retVal.list.Add(vs);
						}
					}
					else
					{
						Error.ReportError("Cannot divide a String by a Number");
					}
				}
			}
			return retVal;
		}
		public Token Mul(Token t1, Token t2)
		{
			Token retVal = new Token();
			retVal.tt = TokenType.Value;

			if (!AreValidOperands(t1, t2))
				Error.ReportError("Invalid Operation, cannot multiply types " + t1.vt.ToString() + ", " + t2.vt.ToString());

			if (IsList(t1) || IsList(t2))
			{
				retVal.vt = ValueType.List;
				retVal.list = new List<Token>();
				if (IsList(t1))
				{
					if (IsList(t2))
					{
						retVal.list.AddRange(t1.list);
						Token vs = new Token();
						vs.list = t2.list;
						vs.tuple = t2.tuple;
						vs.baseClass = t2.baseClass;
						vs.vt = t2.vt;
						vs.scope = t2.scope;
						retVal.list.Add(vs);
					} 
					else if (IsNumeric(t2))
					{
						for (int i = 0; i < int.Parse(t2.token); i++)
							retVal.list.AddRange(t1.list);
					}
					else
					{
						Error.ReportError("Lists can only be multiplied by numbers, not '" + t2.token + "'");
					}
				}
				else
				{
					if (IsNumeric(t1))
					{
						for (int i = 0; i < int.Parse(t1.token); i++)
							retVal.list.AddRange(t2.list);
					}
					else
					{
						Error.ReportError("Lists can only be multiplied by numbers, not '" + t1.token + "'");
					}
				}
			}
			else
			{
				if (IsNumeric(t1))
				{
					if (IsNumeric(t2))
					{
						retVal.vt = ValueType.Number;
						retVal.token = (GetNumber(t1) * GetNumber(t2)).ToString();
					}
					else
					{
						retVal.vt = ValueType.String;
						StringBuilder sb = new StringBuilder();
						for (int i = 0; i < int.Parse(t1.token); i++)
							sb.Append(t2.token);
						retVal.token = sb.ToString();
					}
				}
				else
				{
					if (IsNumeric(t2))
					{
						retVal.vt = ValueType.String;
						StringBuilder sb = new StringBuilder();
						for (int i = 0; i < int.Parse(t2.token); i++)
							sb.Append(t1.token);
						retVal.token = sb.ToString();
					}
					else
					{
						Error.ReportError("Cannot multiply strings with strings ('" + t1.token + "', '" + t2.token + "')");
					}
				}
			}
			return retVal;
		}
		#endregion

		#region Addops
		public Token Sub(Token t1, Token t2)
		{
			Token retVal = new Token();
			retVal.tt = TokenType.Value;

			if (!AreValidOperands(t1, t2))
				Error.ReportError("Invalid Operation, cannot subtract types " + t1.vt.ToString() + ", " + t2.vt.ToString());

			if (IsList(t1) || IsList(t2))
			{
				retVal.vt = ValueType.List;
				retVal.list = new List<Token>();
				
				if (IsList(t1))
					retVal.list.AddRange(t1.list);
				else
				{
					Token listEle = new Token();
					listEle.vt = t1.vt;
					listEle.token = t1.token;
					listEle.scope = t1.scope;
					retVal.list.Add(listEle);
				}

				List<Token> toRemove = new List<Token>();

				if (IsList(t2))
				{
					foreach (Token vs in t2.list)
						foreach (Token cmp in retVal.list)
							if (cmp.token == vs.token)
								toRemove.Add(cmp);
				} 
				else
				{
					foreach (Token vs in retVal.list)
						if (vs.token == t2.token)
							toRemove.Add(vs);
				}

				foreach (Token vs in toRemove)
					retVal.list.Remove(vs);
			}
			else
			{
				if (IsNumeric(t1))
				{
					if (IsNumeric(t2))
					{
						retVal.vt = ValueType.Number;
						retVal.token = (GetNumber(t1) - GetNumber(t2)).ToString();
					}
					else
					{
						retVal.vt = ValueType.String;
						retVal.token = t1.token.Replace(t2.token, "");
					}
				}
				else
				{
					retVal.vt = ValueType.String;
					retVal.token = t1.token.Replace(t2.token, "");
				}
			}
			return retVal;
		}
		public Token Add(Token t1, Token t2)
		{
			Token retVal = new Token();
			retVal.tt = TokenType.Value;

			if(!AreValidOperands(t1, t2))
				Error.ReportError("Invalid Operation, cannot add types " + t1.vt.ToString() + ", " + t2.vt.ToString());

			if (IsList(t1) || IsList(t2))
			{
				retVal.vt = ValueType.List;
				retVal.list = new List<Token>();
				if (IsList(t1))
				{
					retVal.list.AddRange(t1.list);
				}
				else
				{
					Token listEle = new Token();
					listEle.vt = t1.vt;
					listEle.token = t1.token;
					listEle.scope = t1.scope;
					retVal.list.Add(listEle);
				}
				if (!IsList(t2))
				{
					Token listEle = new Token();
					listEle.vt = t2.vt;
					listEle.token = t2.token;
					listEle.scope = t2.scope;
					retVal.list.Add(listEle);
				}
				else
				{
					retVal.list.AddRange(t2.list);
				}
			}
			else
			{
				if (IsNumeric(t1))
				{
					if (IsNumeric(t2))
					{
						retVal.vt = ValueType.Number;
						retVal.token = (GetNumber(t1) + GetNumber(t2)).ToString();
					}
					else
					{
						retVal.vt = ValueType.String;
						retVal.token = t1.token + t2.token;
					}
				}
				else
				{
					retVal.vt = ValueType.String;
					retVal.token = t1.token + t2.token;
				}
			}
			return retVal;
		}
		#endregion

		#region OO

		public Token InheritTuple(Token baseTuple, string inheritType, SymbolTable st)
		{
			Token retVal = new Token();

			//Sanity checks
			if (!st.IsInTable(inheritType))
				Error.ReportError("Could not inherit from type '" + inheritType + "', name was not found.");
			else if (st.GetSymbolType(inheritType) != ValueType.List)
				Error.ReportError("Can only inherit from Classes, not '" + st.GetSymbolType(inheritType) + "'");
			else if (!st.table[inheritType].IsObject)
				Error.ReportError("'" + inheritType + "' is a List, not a Class.  Cannot inherit.");

			Token vs = st.table[inheritType];
			retVal.vt = ValueType.List;
			retVal.tt = TokenType.Value;
			retVal.list = baseTuple.list;
			retVal.tuple = baseTuple.tuple;
			retVal.baseClass = baseTuple.baseClass;

			string[] inheritTupleArray = vs.tuple.ToArray();
			string tupleCheck = "| " + retVal.tuple + " |";

			for (int i = 0; i < inheritTupleArray.Length; i++)
			{
				string newField = inheritTupleArray[i];
				if (!tupleCheck.Contains("| " + newField + " |"))
				{
					retVal.tuple.Add(newField);

					Token vsNew = new Token();
					vsNew.vt = vs.list[i].vt;
					vsNew.token = vs.list[i].token;
					retVal.list.Add(vsNew);
				}
			}
			
			return retVal;
		}

		public Token ClassEquivalence(Token firstTerm, Token typeToken, SymbolTable st)
		{
			Token retVal = new Token();
			retVal.tt = TokenType.Value;
			retVal.vt = ValueType.Bool;

			if (firstTerm.vt != ValueType.List)
			{
				retVal.token = "false";
				return retVal;
			}

			if ((typeToken.tt != TokenType.Name && typeToken.tt != TokenType.Value) || !st.IsInTable(typeToken.token))
				Error.ReportError("Invalid name for equivalence operator: '" + typeToken.token + "'");

			//The idea here is that we go through the tuple of the typeToken and make sure all the properties are on the firstTerm
			List<string> tuple = st.table[typeToken.token].tuple;
			if(tuple == null || tuple.Count == 0)
			{
				retVal.token = "false";
				return retVal;
			}
			string[] checkThese = tuple.ToArray();
			List<string> checkAgainst = firstTerm.tuple;
			foreach(string check in checkThese)
				if(!checkAgainst.Contains(check))
				{
					retVal.token = "false";
					return retVal;
				}

			retVal.token = "true";

			return retVal;
		}

		public Token Is(Token firstTerm, Token typeToken, SymbolTable st)
		{
			Token retVal = new Token();
			retVal.tt = TokenType.Value;
			retVal.vt = ValueType.Bool;

			if (typeToken.token == "Class")
			{
				retVal.SetBooleanValue(firstTerm.isClass);
				return retVal;
			}
			if (typeToken.token == "Object")
			{
				//Classes are not objects
				if (firstTerm.isClass)
				{
					retVal.SetBooleanValue(false);
					return retVal;
				}

				retVal.SetBooleanValue(firstTerm.IsObject);
				return retVal;
			}

			if (firstTerm.vt != ValueType.List || string.IsNullOrEmpty(firstTerm.baseClass))
			{
				//check for value
				retVal.SetBooleanValue(firstTerm.vt.ToString() == typeToken.token);
				return retVal;
			}

			if (firstTerm.baseClass == typeToken.token || typeToken.token == "List")    //All Objects/tuples are Lists
				retVal.token = "true";
			else
			{
				Token t = new Token();
				
				Token baseClass = st.table[firstTerm.baseClass];
				
				//Have we reached the end of the inheritance chain?
				if (firstTerm.baseClass == baseClass.baseClass)
				{
					retVal.token = "false";
					return retVal;
				}
					
				t.baseClass = baseClass.baseClass;
				t.vt = baseClass.vt;

				if (string.IsNullOrEmpty(t.baseClass))
					retVal.token = "false";
				else
					return Is(t, typeToken, st);                    
			}
			
			return retVal;
		}
		#endregion
	}
}
