using System;
using System.Collections.Generic;
using System.Text;

using Shiro.Interpreter;
using Shiro.Interop;

namespace Shiro.Libraries
{
    class TokenComparer : IComparer<Token>
    {
        public int Compare(Token x, Token y)
        {
            return x.token.CompareTo(y.token);
        }
    }
    
    [ShiroClass("Core", KeepClassLoaded=true)]
    public class Core : ShiroBase
    {
        private Random random = new Random();

        [ShiroMethod("abs", 1)]
        public string Abs(double value)
        {
			return System.Math.Abs(value).ToString();
        }

        [ShiroMethod("base", 1)]
        public string baseCls(Token value)
        {
            return value.baseClass;
        }

        [ShiroMethod("setBase", 2)]
        public Token setBase(Token t, string cl)
        {
            t.baseClass = cl;
            return t;
        }

        [ShiroMethod("createA", 1)]
        public Token createA(string type)
        {
            Token retVal = new Token();
            retVal.tt = TokenType.Value;
            retVal.tuple = null;
            retVal.list = new List<Token>();
            retVal.token = "";

            switch (type)
            {
                case "String":
                    retVal.vt = Interpreter.ValueType.String;
                    break;
                case "Number":
                case "Num":
                    retVal.vt = Interpreter.ValueType.Number;
                    break;
                case "Bool":
                case "Boolean":
                    retVal.vt = Interpreter.ValueType.Bool;
                    break;

                case "Function":
                    retVal.vt = Interpreter.ValueType.Function;
                    break;
                
                case "Object":
                    retVal.tuple = new List<string>();
                    retVal.vt = Interpreter.ValueType.List;
                    break;

                case "Class":
                    retVal.tuple = new List<string>();
                    retVal.vt = Interpreter.ValueType.List;
                    retVal.isClass = true;
                    break;

                case "List":
                    retVal.vt = Interpreter.ValueType.List;
                    break;
            }
            return retVal;
        }

        [ShiroMethod("getGlobal", 0)]
        public Token getGlob()
        {
            Token retVal = new Token();
            retVal.list = new List<Token>();
            retVal.tt = TokenType.Value;
            retVal.vt = Interpreter.ValueType.List;

            foreach (string key in Parser.SymbolTable.table.Keys)
            {
                retVal.list.Add(Parser.SymbolTable.table[key].Clone());
                retVal.tuple.Add(key);
            }
            return retVal;
        }

        [ShiroMethod("globalize", 1)]
        public Token Globalize(Token item)
        {
            item.scope = 0;
            return item;
        }

        [ShiroMethod("insertAt", 3)]
        public Token insertAt(string lst, int position, string value)
        {
            Token listToke = Token.FromString(lst);
            if (listToke.vt != Shiro.Interpreter.ValueType.List)
                throw new ShiroException("First Argument to insertAt must be a list");

            int count = listToke.list.Count;
            if (position < 0)
                position = count + (position % count);
            else
                position = position % count;

            Token insVal = Token.FromString(value);
            listToke.list.Insert(position, insVal);

            return listToke;
        }

        [ShiroMethod("sort", 1)]
        public Token sortList(string lst)
        {
            Token listToke = Token.FromString(lst);
            if (listToke.vt != Shiro.Interpreter.ValueType.List)
                throw new ShiroException("Argument to sort must be a List");

            listToke.list.Sort(new TokenComparer());
            return listToke;
        }


        [ShiroMethod("eval", 1)]
        public Token EvalMethod(string code)
        {
            List<Token> tokes = Token.Tokenize(code);
            tokes = new Scanner().Scan(tokes);
            return new Parser().Parse(tokes);
        }

        [ShiroMethod("free", 1)]
        public string FreeMethod(string name)
        {
			if (Parser.SymbolTable.IsInTable(name))
				Parser.SymbolTable.RemoveSymbol(name);
			else if (Parser.SymbolTable.IsInFTab(name))
				Parser.SymbolTable.RemoveFunction(name);
            return name;
        }

        [ShiroMethod("getUid", 0)]
        public string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }

        [ShiroMethod("inject", 3)]
        public Token inject(Token list, string name, Token val)
        {
            Token retVal = list.Clone();
            Token vs = val.Clone();

            retVal.list.Add(vs);
            retVal.tuple.Add(name);
            return retVal;
        }

        [ShiroMethod("len", 1)]
        public string LenMethod(Token toke)
        {
            if (toke.tt != TokenType.Value)
                throw new ShiroException("Invalid item passed to len()");

            switch (toke.vt)
            {
                case Shiro.Interpreter.ValueType.Bool:
                    return "1";
                case Shiro.Interpreter.ValueType.List:
                    return toke.list.Count.ToString();
                default:
                    return toke.token.Length.ToString();
            }
        }

        [ShiroMethod("ms", 0)]
        public string getMilliseconds()
        {
            DateTime Now = DateTime.Now;
            long dayMs = Now.Millisecond +
                (Now.Second * 1000) +
                (Now.Minute * 60000) +
                (Now.Hour * 24 * 60000);

            return dayMs.ToString();
        }

        [ShiroMethod("sleep", 1)]
        public string sleep(int dur)
        {
            System.Threading.Thread.Sleep(dur);
            return dur.ToString();
        }

        [ShiroMethod("rand", 1)]
        public string rand(int maxVal)
        {
            return random.Next(0, maxVal).ToString();
        }
        [ShiroMethod("rseed", 1)]
        public string reSeedRandom(int newSeed)
        {
            random = new Random(newSeed);
            return newSeed.ToString();
        }

        [ShiroMethod("tuple", 1)]
        public Token GetTuple(Token list)
        {
			if (list.vt == Shiro.Interpreter.ValueType.List)
				return Token.FromString(list.tuple);
			else
				return Token.FromString("");
        }
        
        [ShiroMethod("type", 1)]
        public string getType(Token t)
        {
            return t.vt.ToString();
        }

        [ShiroMethod("executeApp", 1)]
        public string ExecuteApp(string app)
        {
            System.Diagnostics.Process Proc = new System.Diagnostics.Process();
            Proc.StartInfo.FileName = app;
            return Proc.Start().ToString().ToLower();
        }
    }
}
