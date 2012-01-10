using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Shiro;
using Shiro.Interop;
using Shiro.Interpreter;

namespace Shiro.Libraries
{
    [ShiroClass("String", KeepClassLoaded=false)]
    public class Strings
    {
        [ShiroMethod("regex", 2)]
        public static string regex(string regEx, string value)
        {
            return Regex.IsMatch(value, regEx, RegexOptions.None) ? "true" : "false";
        }

        [ShiroMethod("contains", 2)]
        public static string Contains(string check, string checkFor)
        {
            return check.Contains(checkFor).ToString().ToLower();
        }

        [ShiroMethod("instr", 2)]
        public static string instr(string checkIn, string checkFor)
        {
            return checkIn.IndexOf(checkFor).ToString();
        }

        [ShiroMethod("lowercase", 1)]
        public static string lower(string str)
        {
            return str.ToLower();
        }
        [ShiroMethod("uppercase", 1)]
        public static string upper(string str)
        {
            return str.ToUpper();
        }

        [ShiroMethod("trim", 1)]
        public static string trim(string s)
        {
            return s.Trim();
        }

        [ShiroMethod("replace", 3)]
        public static string replace(string baseString, string checkFor, string replaceWith)
        {
            return baseString.Replace(checkFor, replaceWith);
        }

        [ShiroMethod("subStr", 3)]
        public static string subStr(string baseStr, int start, int len)
        {
            return baseStr.Substring(start, len);
        }

        #region JSON

        #region JSON to Token parser

        private static string jsonInput = "";
        private static char json_Peek()
        {
            return jsonInput[0];
        }
        private static char json_Pop()
        {
            char ret = json_Peek();
            jsonInput = jsonInput.Substring(1).Trim() + " ";
            return ret;
        }
        private static bool Expected(char c)
        {
            return (json_Pop() == c);
        }
        private static string json_ScanTo(char c)
        {
            string ret = "";
            while (json_Peek() != c)
                ret += json_Pop();
            json_Pop();
            return ret;
        }

        private static Token json_forList()
        {
            Token badReturn = Token.FromString("Bad JSON, sorry");
            List<Token> goodReturn = new List<Token>();

            if (!Expected('['))
                return badReturn;

            while (json_Peek() != ']')
            {
                switch (json_Peek())
                {
                    case '"':
                        json_Pop();
                        goodReturn.Add(Token.FromString(json_ScanTo('"')));
                        break;

                    case '[':
                        goodReturn.Add(json_forList());
                        break;

                    case '{':
                        goodReturn.Add(json_forObj());
                        break;

                    default:
                        return badReturn;
                }

                if (json_Peek() == ',')
                    json_Pop();
            }
            json_Pop();  //closing bracket

            Token ret = new Token(goodReturn);
            return ret;
        }


        private static Token json_forObj()
        {
            Token badReturn = Token.FromString(jsonInput);
            List<Token> goodReturn = new List<Token>();
            string tuple = "";
            
            if (!Expected('{'))
                return badReturn;

            while (json_Peek() != '}')
            {
                if (!Expected('"'))
                    return badReturn;

                tuple += json_ScanTo('"') + " | ";

                if (!Expected(':'))
                    return badReturn;

                switch (json_Peek())
                {
                    case '"':
                        json_Pop();
                        goodReturn.Add(Token.FromString(json_ScanTo('"')));
                        break;

                    case '[':
                        goodReturn.Add(json_forList());
                        break;

                    case '{':
                        goodReturn.Add(json_forObj());
                        break;

                    default:
                        return badReturn;
                }

                if(json_Peek() == ',')
                    json_Pop();
            }
            json_Pop();   //closing bracket

            Token ret = new Token(goodReturn);
            ret.tuple = new List<string>(tuple.Split(new string[] {" | "}, StringSplitOptions.RemoveEmptyEntries));
            return ret;
        }
        #endregion

        #region Token to JSON

        private static string json_fromList(Token t)
        {
            StringBuilder ret = new StringBuilder("[\"");

            foreach(Token item in t.list)
            {
                if (item.vt == Interpreter.ValueType.Function)
                    continue;

                if(item.vt == Interpreter.ValueType.List)
                {
                    if(!item.IsObject)
                        ret.Append(json_fromList(item));
                    else
                        ret.Append(json_fromToken(item));
                } else
                    ret.Append("\"" + item.token + "\"");
                ret.Append(",");
            }
            return ret.ToString().TrimEnd(',') + "]";
        }

        private static string json_fromToken(Token t)
        {
            StringBuilder ret = new StringBuilder("{\"");
            string[] tupe = t.tuple.ToArray();

            for(int i=0; i<tupe.Length;i++)
            {
                if (t.list[i].vt == Interpreter.ValueType.Function)
                    continue;

                ret.Append("\"" + tupe[i] + "\": ");
                if(t.list[i].vt == Interpreter.ValueType.List)
                {
                    if(!t.IsObject)
                        ret.Append(json_fromList(t.list[i]));
                    else
                        ret.Append(json_fromToken(t.list[i]));
                } else
                    ret.Append("\"" + t.list[i].token + "\"");
                
                ret.Append(",");
            }
            return ret.ToString().TrimEnd(',') + "}";
        }
        #endregion

        [ShiroMethod("jsonToObject", 1)]
        public static Token getTokenFromJson(string json)
        {
            jsonInput = json;
            Token ret = json_forObj();
            return ret;
        }
        
        [ShiroMethod("jsonFromObject", 1)]
        public static string getJson(Token obj)
        {
            return json_fromToken(obj).Replace("\"\"", "\"");
        }

        #endregion
    }
}
