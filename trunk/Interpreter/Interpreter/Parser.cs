using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Shiro.Interpreter
{
	public class Parser
	{
		protected List<Token> m_tokens;
		protected Stack<List<Token>> m_pushDown = new Stack<List<Token>>();
		TokenCombiner comb = new TokenCombiner();

		private Dictionary<string, string> _anonymousFunctionNames = new Dictionary<string, string>();

		public SymbolTable SymbolTable = null;
		public Parser()
		{
			SymbolTable = new SymbolTable(this);
		}

		protected bool returned = false;

		#region Token Streamers

		public static int CurrentLine = 0;
		public static int CurrentPos = 0;

		protected Token PopToken()
		{
			return PopToken(true);
		}

		protected string LookAheadPast(string terminator)
		{
			for (int i = 0; i < m_tokens.Count; i++)
				if (m_tokens[i].token == terminator && m_tokens.Count > i + 1)
					return m_tokens[i + 1].token;

			return "";
		}

		protected Token PopToken(bool ProcessPound)
		{
			if (m_tokens.Count > 0)
			{
				if (ProcessPound)
				{
					//Execution operator
					if (m_tokens[0].token == "~")
					{
						m_tokens.RemoveAt(0);
						Token value = default(Token);
						if (PeekToken(false).token == "(")
						{
							value = GetExpressionValue();
							List<Token> newTokes = new Scanner().Scan(Token.Tokenize(value.ToString()));
							for (int i = 0; i < newTokes.Count; i++)
								newTokes[i].scope += SymbolTable.scope;
							m_tokens.InsertRange(0, newTokes);
						}
						else
						{
							string name = PopToken().token;
							if (SymbolTable.IsInTable(name))
							{
								List<Token> newTokes = new Scanner().Scan(Token.Tokenize(SymbolTable.GetSymbolValue(name)));
								for (int i = 0; i < newTokes.Count; i++)
									newTokes[i].scope += SymbolTable.scope;
								m_tokens.InsertRange(0, newTokes);
							}
							else
							{
								Error.ReportError("Cannot use Execution Operator (~) with a nonexistant symbol like '" + name + "'.  If you intended to use an expression, put the value after the tilde in parenthesis (ie:  ~('hello' + 'world')");
							}
						}
					}
				}
				Token ret = m_tokens[0];
				m_tokens.RemoveAt(0);

				CurrentLine = ret.codeLine;
				CurrentPos = ret.codePos;

				return ret;
			}
			else
				return default(Token);
		}
		protected Token PeekToken(bool ProcessPound = true)
		{
			if (m_tokens.Count > 0)
			{
				if (ProcessPound)
				{
					//Execution operator
					if (m_tokens[0].token == "~")
					{
						m_tokens.RemoveAt(0);
						Token value = default(Token);
						if (PeekToken(false).token == "(")
						{
							value = GetExpressionValue();
							List<Token> newTokes = new Scanner().Scan(Token.Tokenize(value.ToString()));
							for (int i = 0; i < newTokes.Count; i++)
								newTokes[i].scope += SymbolTable.scope;
							m_tokens.InsertRange(0, newTokes);
						}
						else
						{
							string name = PopToken().token;
							if (SymbolTable.IsInTable(name))
							{
								List<Token> newTokes = new Scanner().Scan(Token.Tokenize(SymbolTable.GetSymbolValue(name)));
								for (int i = 0; i < newTokes.Count; i++)
									newTokes[i].scope += SymbolTable.scope;
								m_tokens.InsertRange(0, newTokes);
							}
							else
							{
								Error.ReportError("Cannot use Execution Operator (~) with a nonexistant symbol like '" + name + "'.  If you intended to use an expression, put the value after the tilde in parenthesis (ie:  ~('hello' + 'world')");
							}
						}
					}
				}
				
				return m_tokens[0];
			}
			else
				return default(Token);
		}
		protected void PushToken(Token toke)
		{
			m_tokens.Insert(0, toke);
		}
		protected bool Expected(string str)
		{
			Token toke = PopToken();
			if (toke == null)
			{
				Error.ReportError("Unexpected end of the input while expecting '" + str + "'.");
				return false;
			}
			else if (toke.token != str)
			{
				Error.ReportError("Expected '" + str + "', '" + toke.token + "' found instead.");
				return false;
			}
			else
				return true;
		}
		protected bool PeekAndDestroy(string expected, bool processPound = true)
		{
			Token t = PeekToken(processPound);
			if (t != null && t.token == expected)
			{
				PopToken(processPound);
				return true;
			}
			else
			{
				return false;
			}
		}

		protected List<Token> ScanBlock()
		{
			List<Token> retVal = new List<Token>();
			bool done = false;
			int nestCount = 0;

			Expected("{");
			while (m_tokens.Count > 0 && !done)
			{
				if (PeekToken(false).token == "}")
				{
					nestCount -= 1;
					if (nestCount < 0)
					{
						done = true;
						PeekAndDestroy("}");
						continue;
					}
				}
				else if (PeekToken(false).token == "{")
					nestCount += 1;

				Token toke = PopToken(false);
				retVal.Add(toke);
			}

			return retVal;
		}
		protected List<Token> ScanParameterizedBlock()
		{
			List<Token> retVal = new List<Token>();
			bool done = false;
			int nestCount = 0;

			Expected("{");
			while (m_tokens.Count > 0 && !done)
			{
				if (PeekToken(false).token == "}")
				{
					nestCount -= 1;
					if (nestCount < 0)
					{
						done = true;
						PeekAndDestroy("}");
						continue;
					}
				}
				else if (PeekToken(false).token == "{")
					nestCount += 1;

				Token toke = PopToken(false);
				if (toke.token == "@")        //Parameterization Operator (soooo sexy)
				{
					string name = PopToken().token;
					if (SymbolTable.IsInTable(name))
					{
						retVal.Add(SymbolTable.table[name].Clone());
					}
					else
					{
						Error.ReportError("Cannot use Parameterization Operator (@) with a nonexistant symbol like '" + name + "'");
					}
				}
				else
					retVal.Add(toke);
			}

			return retVal;
		}

		#endregion

		#region Expression Parser Helpers
		protected Token ListIndexerParser(Token toke, bool usedDotNotation)
		{
			Token retVal = new Token();
			retVal.tt = TokenType.Value;
			List<Token> vals = null;
			List<string> tuple = null;
			int count;

			if (toke.tt == TokenType.Name || (toke.tt == TokenType.Value && toke.vt == ValueType.NotAValue))
			{
				if (!(SymbolTable.IsInTable(toke.token) && SymbolTable.GetSymbolType(toke.token) == ValueType.List))
					Error.ReportError("Indexer can only follow a list, '" + toke.token + "' is not a list");

				vals = SymbolTable.GetSymbolList(toke.token);
				tuple = SymbolTable.table[toke.token].tuple;
			}
			else if (toke.tt == TokenType.Value && toke.vt == ValueType.List)	//Inline Lists (not user-generated)
			{
				vals = toke.list;
				tuple = toke.tuple;
			}
			else
			{
				Error.ReportError("Cannot Index something that isn't a List or a name thereof (Internal System Problem)");
			}

			Token index = GetExpressionValue();

			//tuple indexes (also class members remember) whose name starts with an _ are protected
			if (usedDotNotation && index.vt == ValueType.String && index.token.StartsWith("_"))
				if (toke.token != "this")
					Error.ReportError("Index '" + index.token + "' is protected and cannot be accessed this way.");

			Token subIndex = null;

			if (PeekAndDestroy("to"))
			{
				retVal.vt = ValueType.List;
				Token endIndex = GetExpressionValue();
				if (index.vt != ValueType.Number || endIndex.vt != ValueType.Number)
				{
					Error.ReportError("Index and End Index must be numeric.  One or both of: '" + index.token + "', '" + endIndex.token + "' is not numeric");
					return default(Token);
				}

				bool bReverse = false;
				count = vals.Count;
				int start = int.Parse(index.token),
					end = int.Parse(endIndex.token);

				if (start < 0)
					start = count + (start % count);
				else
					start = start % count;

				if (end < 0)
					end = count + (end % count);
				else
					end = end % count;

				if (start > end)
				{
					int temp = start;   //Let's not get fancy w/ XOR and say we did.
					start = end;
					end = temp;
					bReverse = true;
				}

				retVal.list = new List<Token>();
				for (int i = start; i <= end; i++)  //'to' is inclusive in Shiro
				{
					retVal.list.Add(vals[i]);
				}
				if (bReverse)
					retVal.list.Reverse();

				Expected("]");
				return retVal;
			}

			if (PeekAndDestroy(","))
			{
				//2D indexing (index a sub-list)
				subIndex = GetExpressionValue();
			}

			//Process (both) indexes for tuple-name indexes
			if (index.vt != ValueType.Number)
			{
				if (index.vt != ValueType.String)
					Error.ReportError("Index must be numeric or string.  '" + index.token + "' is not");
				else
				{
					index.vt = ValueType.Number;
					index.token = SymbolTable.GetTupleIndexInline(vals, tuple, index.token).ToString();
				}
			}

			count = vals.Count;
			int passedIndex = int.Parse(index.token);
			if (passedIndex < 0)
				passedIndex = count + (passedIndex % count);
			else
				passedIndex = passedIndex % count;

			if (subIndex != null)
			{
				if (subIndex.vt != ValueType.Number)
				{
					if (subIndex.vt != ValueType.String)
						Error.ReportError("Index must be numeric or string.  '" + subIndex.token + "' is not");
					else
					{
						subIndex.vt = ValueType.Number;
						subIndex.token = SymbolTable.GetTupleSubIndex(toke.token, passedIndex, subIndex.token).ToString();
					}
				}
				if (vals[passedIndex].vt != ValueType.List)
					Error.ReportError("Invalid Subindex, indexed element was not a List");

				count = vals[passedIndex].list.Count;
				int actualSubIdx = int.Parse(subIndex.token);
				if (actualSubIdx < 0)
					actualSubIdx = count + (actualSubIdx % count);
				else
					actualSubIdx = actualSubIdx % count;

				retVal = vals[passedIndex].list[actualSubIdx].Clone();
			}
			else
			{
				retVal = vals[passedIndex].Clone();
			}

			if (PeekAndDestroy(","))
				Error.ReportError("Only two-dimensional lists (ie Lists of Lists) are presently supported with ',' indexing.");

			Expected("]");

			return retVal;
		}
		#endregion

		#region Expression Parser (Thanks Dr. Crenshaw)

		protected Token GetBaseElement()
		{
			Token toke = PopToken();
			Token retVal = new Token();
			retVal.tt = TokenType.Value;

			if (toke == null)
				Error.ReportError("Unexpected end of input");

			switch (toke.tt)
			{
				case TokenType.Keyword:
					if (toke.token == "def")
					{
						retVal.vt = ValueType.Function;
						retVal.tt = TokenType.Value;
						retVal.token = "anon" + Guid.NewGuid().ToString();

						ParseAnonymousFunctionDefinition(retVal.token, toke.scope);
					}
					else if (toke.token == "new")
					{
						retVal = ParseNew();
					} else 
						Error.ReportError("Only the def or new keywords can be included as a value type, not '" + toke.token + "'");


					break;

				case TokenType.Value:
					retVal = toke.Clone();
					break;

				case TokenType.Name:
					retVal = ParseName(toke);
					break;

				case TokenType.Operator:
					if (toke.token == "-")
					{
						retVal = GetBaseElement();
						switch (retVal.vt)
						{
							case ValueType.Number:
								if(retVal.isNumericLong)
									retVal.token = (0 - long.Parse(retVal.token)).ToString();
								else
									retVal.token = (0 - double.Parse(retVal.token)).ToString();
								break;
							case ValueType.Bool:
								retVal = comb.Not(retVal);
								break;
							case ValueType.String:
								retVal.token = retVal.token.Trim();
								break;
							case ValueType.List:
								retVal.list.Reverse();
								break;
						}
					}
					else if (toke.token == "!")
					{
						retVal = GetBaseElement();
						retVal = comb.Not(retVal);
					}
					else
					{
						Error.ReportError("Only '-' and '!' operators may prefix a value in an expression, not '" + toke.token + "'.");
						return default(Token); ;
					}

					break;

				case TokenType.Symbol:
					if (toke.token == "(")
					{
						retVal = GetExpressionValue();
						Expected(")");
					}
					else if (toke.token == "{")
					{
						bool cont = true;
						List<string> theTuple = new List<string>();
						retVal.vt = ValueType.List;
						retVal.list = new List<Token>();

						while (cont)
						{
							if (PeekToken().token == "}")
							{
								Expected("}");
								cont = false;
								break;
							}
							else if (PeekToken().token == "," && PeekToken().vt == ValueType.NotAValue)
							{
								Expected(",");
							}

							//Check ahead 2 tokens for ':' (named assignment)
							if (m_tokens.Count > 1 && m_tokens[1].token == ":")
							{
								//L = {name: "Dan", age: 27}
								Token tupe = PopToken();
								if (tupe.tt != TokenType.Name)
									Error.ReportError("Member name must be a name, '" + tupe.token + "' is not valid.");

								Expected(":");
								theTuple.Add(tupe.token);

								Token val = GetExpressionValue();
								if (val.tt == TokenType.Value)
								{
									Token listEle = val.Clone();
									listEle.scope = SymbolTable.scope;
									retVal.list.Add(listEle);
								}
							}
							else
							{
								//L = {"Dan", 27}
								if (theTuple.Count != 0)
									theTuple.Add(retVal.list.Count.ToString());

								Token val = GetExpressionValue();
								if (val.tt == TokenType.Value)
								{
									Token listEle = val.Clone();
									listEle.scope = SymbolTable.scope;
									retVal.list.Add(listEle);
								}
							}
						}

						if (theTuple.Count > 0)
							retVal.tuple = theTuple;
					}
					else
					{
						Error.ReportError("Expected expression, '" + toke.token + "' found instead");
						return default(Token);
					}
					break;
			}
			return retVal;
		}

		private Token ParseName(Token toke)
		{
			string oName = toke.token;
			Token retVal = new Token();
			Token currentThis = toke;

			retVal.tt = TokenType.Value;
			if (SymbolTable.IsInTable(oName))
			{
				if (SymbolTable.IsClass(oName))
				{
					string lookAhead = PeekToken().token;
					if (lookAhead == ".")
						Error.ReportError("Classes are not instances and their properties and methods cannot be directly accessed, found '" + oName + "' in an expression");
				}

				if (SymbolTable.GetSymbolType(oName) == ValueType.List)
				{
					retVal = SymbolTable.table[oName].Clone();
					
					//We need to know later on if the list was accessed via 'this' or by
					// name (for example, accessors on members), so this is how we tell.
					if (toke.token == "this")
						retVal.token = "this";
				}
				else
				{
					retVal.vt = SymbolTable.GetSymbolType(toke.token);
					retVal.token = SymbolTable.GetSymbolValue(toke.token);
				}
			}
			else if (SymbolTable.IsInFTab(toke.token))
			{
				retVal.vt = ValueType.Function;
				retVal.token = toke.token;
			}
			else
			{
				//If we're in a tuple/class, we have an implicit 'this.'
				bool processed = false;
				if (SymbolTable.IsInTable("this"))
				{
					Token vs = SymbolTable.table["this"].Clone();
					int index = SymbolTable.GetTupleIndexInline(vs.list, vs.tuple, toke.token, false);

					if (index != -1)
					{
						Token val = vs.list[index];
						retVal = val.Clone();
						retVal.tt = TokenType.Value;
						processed = true;
					}
				} 

				if(!processed)
				{
					retVal = toke.Clone();
				}
			}

			//Check for in-line assignments
			if (PeekAndDestroy("="))
			{
				if (PeekAndDestroy("new"))
					retVal = ParseNew();
				else
				{
					retVal = GetExpressionValue();
					if (retVal.isClass)
						Error.ReportError("You cannot assign a class to a variable.  Please use new to instance it.");
				}

				bool processed = false;
				if (SymbolTable.IsInTable(toke.token))
				{
					if (SymbolTable.GetSymbolType(toke.token) != retVal.vt)
					{
						Error.ReportError("Type mismatch, '" + toke.token + "' is a " + SymbolTable.GetSymbolType(toke.token).ToString()
											+ ", not a " + retVal.vt.ToString() + ".  Check out the 'convert' keyword");
					}
				}
				else
				{
					if (SymbolTable.IsInTable("this"))
					{
						Token vs = SymbolTable.table["this"].Clone();
						int index = SymbolTable.GetTupleIndexInline(vs.list, vs.tuple, toke.token, false);

						if (index != -1 && vs.list[index].vt == retVal.vt)
						{
							processed = true;
							SymbolTable.table["this"].list[index] = retVal.Clone();
						}
					}
				}

				if (!processed)
					if (retVal.vt == ValueType.List)
					{
						List<string> tuple = new List<string>(retVal.tuple.ToArray());
						SymbolTable.CreateListSymbol(toke.token, SymbolTable.scope, retVal.list, tuple, retVal.baseClass);

						//Root classes are their own base classes
						if (tuple != null && tuple.Count > 0 && string.IsNullOrEmpty(SymbolTable.table[toke.token].baseClass))
							SymbolTable.table[toke.token].baseClass = toke.token;

					}
					else
						SymbolTable.CreateSymbol(toke.token, SymbolTable.scope, retVal.vt, retVal.token);

				return retVal;
			}
			else if (PeekAndDestroy(":="))
			{
				//Assignment with inheritance.
				string baseTupleName = PopToken().token;
				List<string> otherTuples = new List<string>();

				while (PeekAndDestroy(","))
					otherTuples.Add(PopToken().token);

				Token newTuple = GetBaseElement();
				if (newTuple.vt != ValueType.List)
					Error.ReportError("Only a list or class may inherit, not a '" + newTuple.tt.ToString() + "'");

				if (SymbolTable.IsInTable(toke.token))
					if (SymbolTable.GetSymbolType(toke.token) != ValueType.List)
					{
						Error.ReportError("Type mismatch, '" + toke.token + "' is a " + SymbolTable.GetSymbolType(toke.token).ToString()
											+ ", not a List.");
					}

				newTuple = comb.InheritTuple(newTuple, baseTupleName, SymbolTable);
				newTuple.baseClass = baseTupleName;

				//Multiple inheritance doesn't change the base class but does otherwise work
				foreach (string otherBase in otherTuples)
					newTuple = comb.InheritTuple(newTuple, otherBase, SymbolTable);

				SymbolTable.CreateListSymbol(toke.token, SymbolTable.scope, newTuple.list, newTuple.tuple, newTuple.baseClass);
				retVal = newTuple.Clone();
			}

			while (true)
			{
				if (PeekAndDestroy("["))
				{
					string s = "";
					if ((s = LookAheadPast("]")) == "=" || s == ":=")
						retVal = ParseListAssignment(toke.token, false);
					else
						retVal = ListIndexerParser(retVal, false);
				}
				else if (PeekAndDestroy("("))
				{
					bool useRetValForFunctionName = false;

					if (!SymbolTable.IsInFTab(toke.token))
					{
						if (!SymbolTable.IsInTable(toke.token) || (SymbolTable.GetSymbolType(toke.token) != ValueType.Function))
						{
							if (!SymbolTable.IsInFTab(retVal.token))
								if (SymbolTable.IsInTable("this"))
								{
									Token vs = SymbolTable.table["this"].Clone();
									int index = SymbolTable.GetTupleIndexInline(vs.list, vs.tuple, toke.token, false);

									if (index == -1 || vs.list[index].vt != ValueType.Function)
										Error.ReportError("Unknown function '" + toke.token + "'");
								} else
									Error.ReportError("Unknown function '" + toke.token + "'");
								
							else
								useRetValForFunctionName = true;
						}
					}

					List<Token> vals = new List<Token>();
					while (!PeekAndDestroy(")"))
					{
						vals.Add(GetExpressionValue());
						PeekAndDestroy(",");
					}

					//Is it a single-token function call, or a member?
					if (!useRetValForFunctionName)
						retVal = ParseFunctionCall(toke.token, vals);
					else
					{
						ThisHelper th = new ThisHelper(SymbolTable);
						string listName = toke.token;
						if(SymbolTable.table.ContainsKey(listName) && SymbolTable.table[listName].IsObject)
							SymbolTable.CreateListSymbol("this", 0, SymbolTable.table[listName].list, SymbolTable.table[listName].tuple, SymbolTable.table[listName].baseClass);

						retVal = ParseFunctionCall(retVal.token, vals);

						SymbolTable.table[listName] = th.UnstoreThis(SymbolTable);
					}
				}
				else if (PeekAndDestroy("$"))
				{
					retVal.tt = TokenType.Value;
					retVal.vt = ValueType.String;
					retVal.token = toke.token;
				}
				else if (PeekAndDestroy("."))
				{
					Token tupe = PopToken();

					if (!retVal.IsObject)
						Error.ReportError("Tuple index operator (.) used with non-tupled list {" + retVal.ToString() + "}");

					tupe.tt = TokenType.Value;
					tupe.vt = ValueType.String;

					PushToken(Token.FromString("]"));
					PushToken(tupe);

					currentThis = retVal;

					string s = "";
					if ((s = LookAheadPast("]")) == "=" || s == ":=")
						retVal = ParseListAssignment(toke.token, true);
					else
						retVal = ListIndexerParser(retVal, true);
				}
				else
				{
					if (retVal.tt == TokenType.Name)
					{
						if (retVal.token == "true" || retVal.token == "false")
						{
							retVal.tt = TokenType.Value;
							retVal.vt = ValueType.Bool;
							return retVal;
						}

						if (retVal.tt != TokenType.Value)
							Error.ReportError("Symbol '" + retVal.token + "' not found.");
					}

					retVal.tt = TokenType.Value;
					return retVal;      //Allows nested indexers and accessors (Crazy stuff)
				}
			}
		}

		protected Token GetFactor()
		{
			Token work = GetBaseElement();
			Token toke;
			bool keepRunning = true;

			while (keepRunning)
			{
				toke = PeekToken();
				if (toke == null)
				{
					keepRunning = false;
					continue;
				}

				switch (toke.token)
				{
					case "*":
						PopToken();
						work = comb.Mul(work, GetBaseElement());
						break;

					case "/":
						PopToken();
						work = comb.Div(work, GetBaseElement());
						break;

					case "%":
						PopToken();
						work = comb.Mod(work, GetBaseElement());
						break;

					default:
						keepRunning = false;
						return work;
				}
			}

			return work;
		}

		protected Token GetTerm()
		{
			Token work = GetFactor();
			Token toke;
			bool keepRunning = true;

			while (keepRunning)
			{
				toke = PeekToken();
				if (toke == null)
				{
					keepRunning = false;
					continue;
				}

				switch (toke.token)
				{
					case "+":
						PopToken();
						work = comb.Add(work, GetFactor());
						break;

					case "-":
						PopToken();
						work = comb.Sub(work, GetFactor());
						break;

					default:
						keepRunning = false;
						return work;
				}
			}

			return work;
		}

		protected Token GetComparison()
		{
			Token work = GetTerm();
			Token toke;
			bool keepRunning = true;

			while (keepRunning)
			{
				toke = PeekToken();
				if (toke == null)
				{
					keepRunning = false;
					continue;
				}

				switch (toke.token)
				{
					case ">":
						PopToken();
						work = comb.GreaterThan(work, GetTerm());
						break;

					case "<":
						PopToken();
						work = comb.LessThan(work, GetTerm());
						break;

					case "<>":
					case "!=":
						PopToken();
						work = comb.Not(comb.EqualTo(work, GetTerm()));
						break;

					case "==":
						PopToken();
						work = comb.EqualTo(work, GetTerm());
						break;

					case ">=":
						PopToken();
						work = comb.Not(comb.LessThan(work, GetTerm()));
						break;

					case "<=":
						PopToken();
						work = comb.Not(comb.GreaterThan(work, GetTerm()));
						break;

					case "=?":
						PopToken();
						work = comb.ClassEquivalence(work, PopToken(), SymbolTable);
						break;

					case "is":
						PopToken();
						work = comb.Is(work, PopToken(), SymbolTable);
						break;

					default:
						keepRunning = false;
						return work;
				}
			}

			return work;
		}

		protected Token GetComparisonCompound()
		{
			Token work = GetComparison();
			Token toke;
			bool keepRunning = true;

			while (keepRunning)
			{
				toke = PeekToken();
				if (toke == null)
				{
					keepRunning = false;
					continue;
				}

				switch (toke.token)
				{
					case "&":
						PopToken();
						work = comb.And(work, GetComparison());
						break;

					case "|":
						PopToken();
						work = comb.Or(work, GetComparison());
						break;

					default:
						keepRunning = false;
						return work;
				}
			}

			return work;
		}

		protected Token GetTernaryLevelExpression()
		{
			Token work = GetComparisonCompound();
			Token toke;
			bool keepRunning = true;

			while (keepRunning)
			{
				toke = PeekToken();
				if (toke == null)
				{
					keepRunning = false;
					continue;
				}

				switch (toke.token)
				{
					case "?":
						PopToken();
						if (comb.Not(comb.Not(work)).token == "true")
						{
							work = GetExpressionValue();
							if (!PeekAndDestroy(":"))
								Error.ReportError("Ternary operator must include colon");
							GetExpressionValue();

						}
						else
						{
							GetExpressionValue();
							if (!PeekAndDestroy(":"))
								Error.ReportError("Ternary operator must include colon");
							else
								work = GetExpressionValue();
						}
						break;

					case "??":
						PopToken();
						work.vt = ValueType.Bool;
						if (SymbolTable.IsInTable(work.token))
							work.token = "true";
						else if (SymbolTable.IsInFTab(work.token))
							work.token = "true";
						else
							work.token = "false";
						break;

					default:
						keepRunning = false;
						return work;
				}
			}

			return work;
		}

		protected Token GetExpressionValue()
		{
			return GetTernaryLevelExpression();
		}
		#endregion

		#region Control Flow

		//if <condition>:
		//    <code>
		//[else:
		//    <code>]
		protected Token ParseIf()
		{
			return ParseIf(false);
		}
		protected Token ParseIf(bool skip)
		{
			Token conditionalValue = comb.Not(comb.Not(GetExpressionValue()));
			Token retVal = default(Token);
			bool wasTrue = false;

			//Get the main block (and else block, if applicable)
			List<Token> trueBlock = ScanBlock();
			List<Token> falseBlock = null;

			//Executional stuff
			wasTrue = conditionalValue.token == "true";
			if (wasTrue)
				retVal = ParseInternal(trueBlock);

			//Have to parse the else even if we don't do anything with it, obviously
			if (PeekAndDestroy("else"))
			{
				if (PeekAndDestroy("if"))
					retVal = ParseIf(wasTrue);
				else
				{
					falseBlock = ScanBlock();
					if (!wasTrue)
						retVal = ParseInternal(falseBlock);
				}
			}
			return retVal;
		}

		//for <name> in <list> [where <condition>]:
		//    <code>
		protected Token ParseFor()
		{
			Token retVal = default(Token), tok;
			string iterativeElementName;

			tok = PopToken();
			if (tok.tt != TokenType.Name)
			{
				Error.ReportError("Iterative element must be a name, you have '" + tok.token + "'");
				return default(Token);
			}

			iterativeElementName = tok.token;
			Token oldIterator = null;
			if (SymbolTable.IsInTable(iterativeElementName))
				oldIterator = SymbolTable.table[iterativeElementName];

			if (!Expected("in"))
				return default(Token);

			Token nameT = PeekToken();
			//So that we can save the (possibly changed) list at the end
			string name = "";
			if (nameT.tt == TokenType.Name)
				name = nameT.token;

			tok = GetExpressionValue();
			if (tok.vt != ValueType.List)
			{
				Error.ReportError("for loops can only iterate over lists, not single-values (unless it's a list with a single value)");
				return default(Token);
			}

			//Check for where (such a badass construct!)
			bool hasWhere = false;
			List<Token> tempBackup, condition = null;
			Token evaluatedCondition = null;
			if(PeekAndDestroy("where"))
			{
				hasWhere = true;
				tempBackup = new List<Token>(m_tokens.ToArray());
				
				//sadly, we have to create a dummy iterative element
				if (tok.list.Count > 0)
				{
					if (tok.list[0].vt == ValueType.List)
						SymbolTable.CreateListSymbol(iterativeElementName, 0, tok.list[0].list, tok.list[0].tuple, tok.list[0].baseClass);
					else
						SymbolTable.CreateSymbol(iterativeElementName, 0, tok.list[0].vt, tok.list[0].token);
				}

				evaluatedCondition = GetExpressionValue();
				condition = tempBackup.GetRange(0, tempBackup.Count - m_tokens.Count);
			}

			List<Token> block = ScanBlock();

			Token newList = new Token();
			newList.tt = TokenType.Value;
			newList.vt = ValueType.List;
			newList.list = new List<Token>();

			foreach (Token vs in tok.list)
			{
				if (vs.vt == ValueType.List)
					SymbolTable.CreateListSymbol(iterativeElementName, 0, vs.list, vs.tuple, vs.baseClass);
				else
					SymbolTable.CreateSymbol(iterativeElementName, 0, vs.vt, vs.token);

				if (hasWhere)
				{
					m_tokens.InsertRange(0, new List<Token>(condition.ToArray()));
					evaluatedCondition = comb.Not(comb.Not(GetExpressionValue()));

					if(evaluatedCondition.token != "false")
						retVal = ParseInternal(block);
				} 
				else 
					retVal = ParseInternal(block);

				newList.list.Add(SymbolTable.table[iterativeElementName]);

				if (returned)
					break;
				if (_timesToBreak > 0)
				{
					_timesToBreak -= 1;
					break;
				}
			}

			if (oldIterator != null)
				SymbolTable.table[iterativeElementName] = oldIterator;
			else
				SymbolTable.RemoveSymbol(iterativeElementName);

			if (!string.IsNullOrEmpty(name))
				SymbolTable.table[name] = newList.Clone();

			return retVal;
		}

		//while <condition>:
		//    <code>
		protected Token ParseWhile()
		{
			List<Token> condExp = new List<Token>();
			List<Token> loopOp;
			Token retVal = default(Token);

			//Again with the C# references
			List<Token> tempBackup = new List<Token>(m_tokens.ToArray());
			List<Token> condition = null;
			Token evaluatedCondition = comb.Not(comb.Not(GetExpressionValue()));

			condition = tempBackup.GetRange(0, tempBackup.Count - m_tokens.Count);
			tempBackup = null;

			loopOp = ScanBlock();

			while (evaluatedCondition.token != "false")
			{
				retVal = ParseInternal(loopOp);

				if (returned)
					break;

				if (_timesToBreak > 0)
				{
					_timesToBreak -= 1;
					break;
				}

				m_tokens.InsertRange(0, new List<Token>(condition.ToArray()));
				evaluatedCondition = comb.Not(comb.Not(GetExpressionValue()));
			}

			return retVal;
		}

		//try:
		//    <code>
		//catch <name>:
		//    <code>
		//[else:
		//    <code>]   For success condition
		//[finally:
		//    <code>]
		protected Token ParseTry()
		{
			List<Token> tryBlock = null;
			List<Token> catchBlock = null;
			List<Token> elseBlock = null;
			List<Token> finallyBlock = null;
			Token retVal = default(Token);
			string catchName;

			tryBlock = ScanBlock();
			if (!Expected("catch"))
				return default(Token);

			Token t = PopToken();
			if (t.tt != TokenType.Name)
			{
				Error.ReportError("catch must be followed by the name of the error-condition variable");
				return default(Token);
			}
			catchName = t.token;

			catchBlock = ScanBlock();
			if (PeekAndDestroy("else"))
			{
				elseBlock = ScanBlock();
			}

			if (PeekAndDestroy("finally"))
			{
				finallyBlock = ScanBlock();
			}

			bool failed = false;
			try
			{
				retVal = ParseInternal(tryBlock);
			}
			catch (ShiroException ex)
			{
				failed = true;
				SymbolTable.CreateSymbol(catchName, 0, ValueType.String, ex.Message);
				if(catchBlock != null && catchBlock.Count > 0)
					retVal = ParseInternal(catchBlock);
			}

			//else
			if (!failed && elseBlock != null && elseBlock.Count > 0)
			{
				retVal = ParseInternal(elseBlock);
			}

			//finally
			if (finallyBlock  != null && finallyBlock .Count > 0)
				retVal = ParseInternal(finallyBlock);

			return retVal;
		}

		//select <value>:
		//    option <value>:   <--  Note, option = scope +1
		//        <code>        <--         code = scope+2
		//...
		//[   option else:
		//        <code>]
		protected Token ParseSelect()
		{
			Token value = GetExpressionValue();
			Token retVal = default(Token);
			bool bFoundOption = false;
			
			Expected("{");

			while (PeekAndDestroy("option"))
			{
				if (PeekAndDestroy("else"))
				{
					List<Token> block = ScanBlock();
					if (!bFoundOption)
					{
						if (block != null)
							retVal = ParseInternal(block);
					}
				}
				else
				{
					Token compTo = GetExpressionValue();

					List<Token> block = ScanBlock();
					if (comb.EqualTo(value, compTo).token == "true")
					{
						bFoundOption = true;
						if (block != null)
							retVal = ParseInternal(block);
					}
				}
			}
			Expected("}");

			return retVal;
		}

		//break [twice | thrice]
		protected void ParseBreak()
		{
			int timesToBreak = 0;

			if (PeekAndDestroy("twice"))
				timesToBreak = 2;
			else if (PeekAndDestroy("thrice"))
				timesToBreak = 3;
			else
				timesToBreak = 1;

			if (SymbolTable.scope - timesToBreak < SymbolTable.Globalscope)
				Error.ReportError("Attempted to break beyond global scope.  Are you sure you needed a 'twice' or 'thrice' there?");

			_timesToBreak = timesToBreak;
		}

		#endregion

		#region Other Parse Routines

		//Person = {name: '', age: 0}
		//Dan = new Person
		protected Token ParseNew()
		{
			Token retVal = new Token();
			Token className = PopToken();

			if (className.tt != TokenType.Name)
				Error.ReportError("Cannot follow 'new' keyword with anything but a name");

			if (!SymbolTable.IsInTable(className.token))
				Error.ReportError("No class/object '" + className.token + "' found to instance");

			if (SymbolTable.GetSymbolType(className.token) != ValueType.List)
				Error.ReportError("No class/object'" + className.token + "' found to instance");

			List<Token> obj = SymbolTable.GetSymbolList(className.token);
			List<string> tuple = new List<string>(SymbolTable.table[className.token].tuple.ToArray());

			retVal.tt = TokenType.Value;
			retVal.vt = ValueType.List;
			retVal.list = new List<Token>(obj.ToArray());
			retVal.tuple = tuple;
			retVal.scope = SymbolTable.scope;
			retVal.baseClass = className.token;

			if (PeekAndDestroy("("))
			{
				List<Token> vals = new List<Token>();
				while (!PeekAndDestroy(")"))
				{
					vals.Add(GetExpressionValue());
					PeekAndDestroy(",");
				}

				if (retVal.tuple.Contains(className.token))
				{
					//We have a CTOR, create 'this'
					ThisHelper th = new ThisHelper(SymbolTable);

					SymbolTable.CreateListSymbol("this", 0, retVal.list, retVal.tuple, retVal.baseClass);
					ParseFunctionCall(obj[SymbolTable.GetTupleIndex("this", className.token)].token, vals);

					retVal = th.UnstoreThis(SymbolTable);
				}
				else
				{
					Error.ReportError("There is no Constructor to call for '" + className.token + "'");
				}
			}

			return retVal;
		}

		//convert <name>=<value>
		protected void ParseConvert()
		{
			Token name = PopToken();
			if (name.tt != TokenType.Name)
				Error.ReportError("convert must be followed by a variable name for assignment");

			Expected("=");
			int originalScope = SymbolTable.table[name.token].scope;
			Token value = GetExpressionValue();
			value.scope = originalScope;
			SymbolTable.table[name.token] = value;
		}

		protected Token ParseListAssignment(string listName, bool usedDotNotation)
		{
			Token retVal = default(Token);
			int index = 0;

			Token t = GetExpressionValue();
			//tuple indexes (also class members remember) whose name starts with an _ are protected
			if (usedDotNotation && t.vt == ValueType.String && t.token.StartsWith("_"))
				if (listName != "this")
					Error.ReportError("Cannot assign to member '" + t.token + "', it is protected.");

			switch (t.vt)
			{
				case ValueType.Number:
					index = int.Parse(t.token);
					break;
				case ValueType.String:
					index = SymbolTable.GetTupleIndex(listName, t.token);
					break;
				default:
					Error.ReportError("List indexer or class accessor in assignment must be a Number or a String");
					break;
			}

			Expected("]");

			if (PeekAndDestroy("("))
			{
				//Member function call
				List<Token> list = new List<Token>(SymbolTable.GetSymbolList(listName).ToArray());
				int count = list.Count;

				if (index < 0)
					index = count + (index % count);
				else
					index = index % count;

				if (list[index].vt != ValueType.Function)
					Error.ReportError("Cannot call member " + listName + "." + t.token);

				List<Token> vals = new List<Token>();
				while (!PeekAndDestroy(")"))
				{
					vals.Add(GetExpressionValue());
					PeekAndDestroy(",");
				}

				//Create 'this'
				ThisHelper th = new ThisHelper(SymbolTable);

				SymbolTable.CreateListSymbol("this", 0, list, SymbolTable.table[listName].tuple, SymbolTable.table[listName].baseClass);

				retVal = ParseFunctionCall(list[index].token, vals);
				
				SymbolTable.table[listName] = th.UnstoreThis(SymbolTable);
			}
			else
			{
				Expected("=");

				List<Token> list = new List<Token>(SymbolTable.GetSymbolList(listName).ToArray());
				int count = list.Count;

				if (index < 0)
					index = count + (index % count);
				else
					index = index % count;

				retVal = GetExpressionValue();
				Token vs = retVal.Clone();

				list[index] = vs;
				SymbolTable.CreateListSymbol(listName, SymbolTable.scope, list, SymbolTable.table[listName].tuple, SymbolTable.table[listName].baseClass);
			}

			return retVal;
		}

		//define <name> ([<name list>]):
		//    <code>
		protected void ParseDefine()
		{
			Token name = PopToken();
			if (name.tt != TokenType.Name)
				Error.ReportError("Invalid function name: '" + name.token + "'");
			
			//DLL -- I've decided this isn't really an error.  LOL
			if (SymbolTable.IsInFTab(name.token))
				SymbolTable.RemoveFunction(name.token);

			Expected("(");
			List<ArgSym> varNames = new List<ArgSym>();
			while (!PeekAndDestroy(")"))
			{
				Token argName = PopToken();
				ValueType vt = ValueType.NotAValue;
				ArgConstraintType act = ArgConstraintType.None;
				string baseClass = "";

				if (argName.tt != TokenType.Name)
				{
					if (argName.tt == TokenType.Symbol && argName.token == "...")
						argName.token = "args";
					else
						Error.ReportError("Invalid argument name: '" + argName.token + "'");
				}
				else
				{
					//Typed arguments (ie:  def increment(n is Number))
					if (PeekAndDestroy("is"))
					{
						string argType = PopToken().token;
						vt = Token.GetVTFromString(argType);
						act = ArgConstraintType.IsOperator;

						//It's a base class!
						if (vt == ValueType.NotAValue)
							baseClass = argType;
					}
					else if (PeekAndDestroy("=?"))
					{
						string argType = PopToken().token;
						vt = Token.GetVTFromString(argType);
						act = ArgConstraintType.Equivalence;

						//It's a base class!
						if (vt == ValueType.NotAValue)
							baseClass = argType;
					}
				}

				ArgSym asm = new ArgSym(argName.token);
				asm.act = act;
				asm.baseClass = baseClass;
				asm.vt = vt;

				varNames.Add(asm);

				if (argName.token != "args")
					PeekAndDestroy(",");
			}
			List<Token> body = ScanBlock();

			SymbolTable.AddFunction(name.token, body, varNames);
		}

		//forAll({1,2,3,4}, def(it):
		//  print it
		//)
		protected void ParseAnonymousFunctionDefinition(string autoGenName, int declScope)
		{
			if (SymbolTable.IsInFTab(autoGenName))
				Error.ReportError("Duplicate anonymous function id, '" + autoGenName + "' this is impossible unless Guids are duped");

			Expected("(");
			List<ArgSym> varNames = new List<ArgSym>();
			while (!PeekAndDestroy(")"))
			{
				Token argName = PopToken();
				ArgConstraintType act = ArgConstraintType.None;
				ValueType vt = ValueType.NotAValue;
				string baseClass = "";

				if (argName.tt != TokenType.Name)
				{
					if (argName.tt == TokenType.Symbol && argName.token == "...")
						argName.token = "args";
					else
						Error.ReportError("Invalid argument name: '" + argName.token + "'");
				}
				else
				{
					//Typed arguments (ie:  def increment(n is Number))
					if (PeekAndDestroy("is"))
					{
						string argType = PopToken().token;
						vt = Token.GetVTFromString(argType);
						act = ArgConstraintType.IsOperator;

						//It's a base class!
						if (vt == ValueType.NotAValue)
							baseClass = argType;
					}
					else if (PeekAndDestroy("=?"))
					{
						string argType = PopToken().token;
						vt = Token.GetVTFromString(argType);
						act = ArgConstraintType.Equivalence;

						//It's a base class!
						if (vt == ValueType.NotAValue)
							baseClass = argType;
					}
				}

				ArgSym asm = new ArgSym(argName.token);
				asm.baseClass = baseClass;
				asm.act = act;
				asm.vt = vt;

				varNames.Add(asm);

				if (argName.token != "args")
					PeekAndDestroy(",");
			}

			List<Token> body = ScanParameterizedBlock();

			//Process Body to be 0-based Scope;
			if (body.Count > 0)
			{
				int scopeMode = body[0].scope;

				if (scopeMode != 0)
					for (int i = 0; i < body.Count; i++)
						body[i].scope -= scopeMode;
			}

			SymbolTable.AddFunction(autoGenName, body, varNames);
		}


		public Token ParseFunctionCall(string name, List<Token> args)
		{
			Token retVal = default(Token);
			string helpfulName = name;

			//We can't get into this function if name is not one of three things:
			//  1)  A function
			//  2)  A function variable
			//  3)  A function accessible via implicit this
			if (!SymbolTable.IsInFTab(name))
				if (SymbolTable.IsInTable(name) && SymbolTable.table[name].vt == ValueType.Function)
					name = SymbolTable.GetSymbolValue(name);        //Function type variable
				else
				{
					if (SymbolTable.IsInTable("this"))
					{
						Token vs = SymbolTable.table["this"].Clone();
						int index = SymbolTable.GetTupleIndexInline(vs.list, vs.tuple, name, false);
						if(index == -1)
							Error.ReportError("Internal System Problem:  A bad member name snuck through to ParseFunctionCall: '" + helpfulName + "'");
						name = vs.list[index].token;
					} else
						Error.ReportError("Internal System Problem:  There is no 'this' in ParseFunctionCall and one was expected, call to: '" + helpfulName + "'");
				}


			if(SymbolTable.IsLibraryFunction(name))
				retVal = SymbolTable.CallLibraryFunction(name, args.ToArray());
			else 
			{
				List<Token> body;
				body = Token.CloneList(SymbolTable.GetFBody(name));
				Dictionary<string, Token> oldArgs = new Dictionary<string, Token>();
				bool hasArgsParm = false;

				foreach (ArgSym arg in SymbolTable.GetFArgs(name))
				{
					string argName = arg.Name;
					if (argName == "args")
					{
						hasArgsParm = true;
						break;
					}
					if (args.Count == 0)
						Error.ReportError("Too few arguments passed to function '" + helpfulName + "'");

					if (SymbolTable.IsInTable(argName))
						oldArgs.Add(argName, SymbolTable.table[argName]);

					if (arg.vt == ValueType.NotAValue)
					{
						if (!string.IsNullOrEmpty(arg.baseClass))
						{
							Token type = new Token();
							type.token = arg.baseClass;

							switch (arg.act)
							{
								case ArgConstraintType.IsOperator:
									if ("false" == comb.Is(args[0].Clone(), type, SymbolTable).token)
										Error.ReportError("Argument type mismatch, function '" + helpfulName + "', argument '" + arg.Name + ", expected class '" + arg.baseClass + "', actual value: '" + args[0].ToString() + "'");
									break;

								case ArgConstraintType.Equivalence:
									if ("false" == comb.ClassEquivalence(args[0].Clone(), type, SymbolTable).token)
										Error.ReportError("Argument type mismatch, function '" + helpfulName + "', argument '" + arg.Name + ", expected equivalance to '" + arg.baseClass + "', not found in: '" + args[0].ToString() + "'");
									break;

								default:
									Error.ReportError("Bad Argument Constraint Type.  Internal problem");
									break;
							}
						}
					}
					else
						if(args[0].vt != arg.vt)
							Error.ReportError("Argument type mismatch, function '" + helpfulName + "', argument '" + arg.Name + ", actual value: '" + args[0].ToString() + "'");
					
					if (args[0].vt == ValueType.List)
						SymbolTable.CreateListSymbol(argName, SymbolTable.scope, args[0].list, args[0].tuple, args[0].baseClass);
					else
						SymbolTable.CreateSymbol(argName, SymbolTable.scope, args[0].vt, args[0].token);

					args.RemoveAt(0);
				}

				if (args.Count != 0)
				{
					if (!hasArgsParm)
						Error.ReportError("Too many arguments passed to function '" + helpfulName + "'");
					else
					{
						List<Token> argsValue = new List<Token>();
						foreach (Token arg in args)
						{
							Token addMe;

							if (arg.vt == ValueType.List)
							{
								addMe = new Token(arg.list);
								addMe.baseClass = arg.baseClass;
								addMe.tuple = arg.tuple;
							}
							else
								addMe = new Token(arg.token, arg.vt);

							argsValue.Add(addMe);
						}
						SymbolTable.CreateListSymbol("args", SymbolTable.scope, argsValue);
					}
				}

				for (int i = 0; i < body.Count; i++)
				    body[i].scope += SymbolTable.scope + 1;

				retVal = ParseInternal(body, true);

				foreach (string argName in oldArgs.Keys)
					SymbolTable.table[argName] = oldArgs[argName];


				returned = false;
			}

			return retVal;
		}

		#endregion

		#region Top level functions

		public string BackDoorParse(string code)
		{
			Token[] old = m_tokens.ToArray();
			List<Token>[] oldPD = m_pushDown.ToArray();

			string retVal = Parse(new Scanner().Scan(Token.Tokenize(code))).ToString();

			m_tokens = new List<Token>(old);
			m_pushDown = new Stack<List<Token>>(oldPD);

			return retVal;
		}

		protected Token ParseRoot()
		{
			Token rootToken;
			Token retVal = new Token();

			rootToken = PopToken();
			if (rootToken.Equals(default(Token)))
				return rootToken;

			if (rootToken.tt == TokenType.Name)
			{
				retVal = ParseName(rootToken);
			}
			else if (rootToken.tt == TokenType.Keyword)
			{
				switch (rootToken.token)
				{
					#region Out-Of-Function cases
					case "if":
						retVal = ParseIf();
						break;

					case "for":
						retVal = ParseFor();
						break;

					case "while":
						retVal = ParseWhile();
						break;

					case "try":
						retVal = ParseTry();
						break;

					case "select":
						retVal = ParseSelect();
						break;

					case "break":
						ParseBreak();
						break;

					case "convert":
						ParseConvert();
						break;

					case "def":
						ParseDefine();
						break;

					#endregion

					#region In-Line cases
					
					case "class":
						Token next = PopToken();
						if (next.tt != TokenType.Name)
							Error.ReportError("class definition must be followed by a name, not, '" + next.token + "'");

						retVal = ParseName(next);
						SymbolTable.table[next.token].isClass = true;
						SymbolTable.table[next.token].scope = 0;        //Classes, like static functions, always occur at global scope
						return retVal;
					
					case "return":
						retVal = GetExpressionValue();
						
						//return val [then { <do stuff> }]
						if (PeekAndDestroy("then"))
						{
							retVal = retVal.Clone();        //just in case
							List<Token> block= ScanBlock();
							ParseInternal(block);   //note:  the return value of the post-return clause is always ignored.
						}
						returned = true;
						return retVal;

					case "run":
						Token t = PopToken();
						Parser p = new Parser();
						if (t.vt != ValueType.String)
							Error.ReportError("run must be followed by a string containing a file name");

						try
						{
							StreamReader reader = new StreamReader(t.token);
							if (reader == null)
								throw new FileNotFoundException();
							string code = reader.ReadToEnd();
							reader.Close();
							reader = null;

							List<Token> tokes = Token.Tokenize(code);
							tokes = new Scanner().Scan(tokes);
							retVal = p.ParseInternal(tokes);
						}
						catch (Exception)
						{
							Error.ReportError("File not found: " + t.token);
						}
						break;

					case "use":
						Token t2 = PopToken();
						if (t2.tt != TokenType.Name)
							Error.ReportError("use must be followed by the name of the library to load, not '" + t2.token + "'");

						if (!PeekAndDestroy("in"))
						{
							if (!SymbolTable.LoadLibrary(t2.token))
								Error.ReportError("Error loading library '" + t2.token + "', please be sure it exists");
						}
						else
						{
							Token file = PopToken();
							if (file.vt != ValueType.String)
								Error.ReportError("use keyword can only load a library from a file contained in a string when using 'in', not " + file.vt);

							if (!SymbolTable.LoadLibrary(t2.token, file.token))
								Error.ReportError("Error loading library '" + t2.token + "' from '" + file.token + "' please be sure it exists");
						}
						break;

					case "throw":
						retVal = GetExpressionValue();
						if (retVal.vt == ValueType.List)
							throw new ShiroException("Cannot throw a List (well, you can, but you won't see what's in it)");
						else
							throw new ShiroException(retVal.token);

					case "print":
						retVal = GetExpressionValue();
						if (retVal.vt == ValueType.List)
						{
							StringBuilder sb = new StringBuilder();
							foreach (Token vs in retVal.list)
								sb.Append(vs.token + ",");

							Error.StandardOut(sb.ToString().TrimEnd(','));
						}
						else
						{
							Error.StandardOut(retVal.token);
						}
						break;
					
					case "where":
						Error.ReportError("where is not allowed as a top-level construct");     //Yet, bitches
						break;
					#endregion

					default:
						Error.ReportError("Unknown Keyword error -- this is more my problem than yours.");
						return default(Token);
				}
			}
			else
			{
				Error.ReportError("Cannot handle token in this context:  '" + rootToken.token + "'");
				return default(Token);
			}

			return retVal;
		}

		protected Token ParseInternal(List<Token> tokes)
		{
			return ParseInternal(tokes, false);
		}

		protected int _timesToBreak = -1;
		protected Token ParseInternal(List<Token> argTokes, bool isFunctionCall)
		{
			Token ret = Token.FromString("");
			SymbolTable.scope++;

			m_pushDown.Push(m_tokens);
			m_tokens = Token.CloneList(argTokes);

			try
			{
				if (isFunctionCall)
					returned = false;

				while (m_tokens != null && m_tokens.Count > 0 && !returned)
				{
					if (_timesToBreak > 0)
						m_tokens.Clear();

					if(m_tokens.Count > 0)
						ret = ParseRoot();
				}

				if (returned)
					m_tokens.Clear();
			}
			catch (Exception ex)
			{
				if (ex is NullReferenceException)
					throw new ShiroException("Unexpected end of input");
				else if (ex is ShiroException)
					throw ex;
				else
					throw new ShiroException(".NET Exception: " + ex.Message);
			}
			finally
			{
				SymbolTable.scope--;
				if(m_pushDown.Count > 0)
					m_tokens = m_pushDown.Pop();
			}
			return ret;
		}

		public Token Parse(List<Token> input)
		{
			Token ret = Token.FromString("");
			m_pushDown.Clear();
			List<Token> useThisInput = Token.CloneList(input);
			m_tokens = useThisInput;

			while (m_tokens.Count > 0 && !returned)
				ret = ParseRoot();

			if (returned)
				returned = false;
			return (ret == null) ? new Token() : ret;
		}
		#endregion
	}
}
