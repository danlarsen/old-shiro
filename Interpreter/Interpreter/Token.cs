using System;
using System.Collections.Generic;
using System.Text;

namespace Shiro.Interpreter
{
	public enum TokenType
	{
		Name,
		Keyword,
		Operator,
		CondOp,
		Symbol,
		Value,
	}
	public enum ValueType
	{
		NotAValue,
		String,
		Bool,
		Number,
		Function,
		List
	}

	public class Token
	{
		public int scope = 0;
		public int codeLine = 0, codePos = 0;
		public string token = "";
		public List<string> tuple = new List<string>();
		public string baseClass = "";

		public bool isClass = false;
		public bool isNumericLong = false;

		public List<Token> list = null;
		public TokenType tt = TokenType.Value;
		public ValueType vt = ValueType.NotAValue;

		public bool IsObject
		{
			get
			{
				return !(tuple == null || tuple.Count == 0);
			}
		}

		public Token GetObjectPropertyValue(string propName)
		{
			if (!IsObject || !tuple.Contains(propName))
				return Token.FromString("");
			return list[tuple.IndexOf(propName)].Clone();
		}

		public Token() {  }
		public Token(string val, ValueType vtIn)
		{
			token = val;
			tt = TokenType.Value;
			vt = vtIn;
		}
		public Token(List<Token> vals)
		{
			list = vals;
			tt = TokenType.Value;
			vt = ValueType.List;
		}

		public override string ToString()
		{
			if (tt == TokenType.Value)
				if (vt == ValueType.List)
				{
					StringBuilder sb = new StringBuilder();
					foreach (Token vs in list)
						if (vs.vt == ValueType.List)
							sb.Append("LIST | ");
						else 	
							sb.Append(vs.token + " | ");
					
					return sb.ToString().TrimEnd(new char[]{' ', '|'});
				}
				else
				{
					return token;
				}
			else
				return token;
		}

		
		#region Statics (The Tokenizer)

		public static ValueType GetVTFromString(string s)
		{
			switch (s)
			{
				case "Bool":
					return ValueType.Bool;

				case "String":
					return ValueType.String;

				case "Num":
				case "Number":
					return ValueType.Number;

				case "Function":
					return ValueType.Function;

				case "List":
				case "Object":
					return ValueType.List;

				default:
					return ValueType.NotAValue;
			}
		}

		protected static char[] Terminators = { '+', '-', '*', '/', '^', 
										 '(', ')', '[', ']', '{', '}', '|', '=', '%',
										 '@', '!', '~', ':', ';', '?', '$', '#', '&',
										 '\\', '>', '<', '.', ',', '"', '\'', ' ', '\t', '`' };

		public static Token FromString(List<string> list)
		{
			Token retVal = new Token();
			retVal.tt = TokenType.Value;
			retVal.vt = ValueType.List;
			retVal.list = new List<Token>();
			foreach(string s in list)
				retVal.list.Add(Token.FromString(s));
			retVal.token = "";
			return retVal;
		}

		public static Token FromString(string s)
		{
			Token retVal = new Token();
			double l;
			long l2;
			retVal.tt = TokenType.Value;
			retVal.vt = ValueType.String;

			if (double.TryParse(s, out l))
			{
				retVal.vt = ValueType.Number;
				retVal.isNumericLong = false;
			}
			else if (long.TryParse(s, out l2))
			{
				retVal.vt = ValueType.Number;
				retVal.isNumericLong = true;
			}
			else if (s == "true" || s == "false")
				retVal.vt = ValueType.Bool;
			else if (s == "}" || s == "{")
				retVal.tt = TokenType.Symbol;
			else if (s.Contains(" | "))
			{
				retVal.vt = ValueType.List;
				retVal.list = new List<Token>();
				string[] eles = s.Split(new string[] { " | " }, StringSplitOptions.RemoveEmptyEntries);
				foreach (string ele in eles)
				{
					retVal.list.Add(new Token(ele, ValueType.String));
				}

				return retVal;
			}

			retVal.token = s;
			return retVal;
		}

		public static List<Token> Tokenize(string code)
		{
			List<Token> retVal = new List<Token>();
			code = code.Replace("\r\n", "\n").Replace("\t", " ");
			string[] lines = code.Split(new char[] {'\n', '\r'}, StringSplitOptions.None);
			bool scanToString = false, inComment = false;
			char stringChar = '\"';
			string work = "";
			List<char> charsToBreakOn = new List<char>(Terminators);

			int lineNo = 0;
			foreach (string line in lines)
			{
				lineNo++;
				if (string.IsNullOrEmpty(line.TrimEnd()))
					continue;

				if(!scanToString)
					work = "";

				for (int i = 0; i < line.Length; i++)
				{
					if(inComment)
						if (line[i] == '<' && line.Length > i + 1 && line[i + 1] == '#')
						{
							inComment = false;
							i++;
							continue;
						}
						else
						{
							continue;
						}

					if (scanToString)
					{
						if (line[i] == stringChar)
						{
							Token toke = new Token();
							toke.codeLine = lineNo;
							toke.codePos = i;
							toke.token = work;
							toke.vt = ValueType.String;
							work = "";
							retVal.Add(toke);
							scanToString = false;
							continue;
						}
						else
						{
							work += line[i];
						}
					}
					else
					{
						if (charsToBreakOn.Contains(line[i]))
						{
							//Special case:  '.' in a number is a valid decimal
							long temp;  //Use long to avoid double decimals
							if (line[i] == '.' && long.TryParse(work, out temp))
							{
								work += ".";
								continue;
							}
							
							if (!string.IsNullOrEmpty(work.Trim()))
							{
								Token toke = new Token();
								toke.codeLine = lineNo;
								toke.codePos = i;
								toke.token = work;
								work = "";
								if(toke.token != " ")
									retVal.Add(toke);
							}

							//Comments = # to end of line or #> ... <#
							if (line[i] == '#')
							{
								if (line.Length > i + 1 && line[i+1] == '>')
								{
									inComment = true;
									i += 1;
									continue;
								}
								else
									break;
							}							

							if (line[i] == '\"' || line[i] == '\'')
							{
								scanToString = true;
								stringChar = line[i];
							}
							else
							{
								Token breaker = new Token();
								breaker.codeLine = lineNo;
								breaker.codePos = i;
								breaker.token = line[i].ToString();
								if (breaker.token != " ")
									retVal.Add(breaker);
							}
						}
						else
						{
							work += line[i];
						}
					}
				}
				if(work != "" && !scanToString)
				{
					Token toke = new Token();
					toke.token = work;
					toke.codeLine = lineNo;
					toke.codePos = 0;
					retVal.Add(toke);
					work = "";
				}
				else if (scanToString)
				{
					work += Environment.NewLine;
				}
			}

			if (scanToString)
				Error.ReportError("Unterminated string!");

			return retVal;
		}

		#endregion

		internal Token Clone()
		{
			Token t = new Token();
			t.codeLine = codeLine;
			t.codePos = codePos;
			t.list = Token.CloneList(list);
			t.scope = scope;
			t.token = token;
			t.tt = tt;
			t.tuple = tuple;
			t.baseClass = baseClass;
			t.vt = vt;
			t.isClass = isClass;

			return t;
		}

		internal static List<Token> CloneList(List<Token> tokes)
		{
			List<Token> retVal = new List<Token>();

			if (tokes == null)
				return null;

			foreach (Token toke in tokes)
				retVal.Add(toke.Clone());

			return retVal;
		}

		internal void SetBooleanValue(bool val)
		{
			if (val)
				token = "true";
			else
				token = "false";
		}
	}
}
