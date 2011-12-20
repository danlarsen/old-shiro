using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Shiro.Interpreter;

namespace Shiro
{
    public class ShiroInterpret
    {
        public string ShiroVersion = "0.9.0";
        
        public Parser Parser = new Parser();
        protected Scanner Scanner = new Scanner();

        private void SilentStdOut(string msg)
        {
            
        }

        public string IsValidOrGetError(string code)
        {
            List<Token> tokes = Token.Tokenize(code);

            ShiroStdOut sout = Error._out;
            Error.SetStandardOutput(SilentStdOut);

            try
            {
                tokes = Scanner.Scan(tokes);
                Parser.Parse(tokes);

                return "";
            }
            catch (ShiroException ex)
            {
                return ex.Message;
            }
            finally
            {
                Error.SetStandardOutput(sout);
            }

        }

        public string Execute(string code)
        {
            List<Token> tokes = Token.Tokenize(code);
            tokes = Scanner.Scan(tokes);
            return Parser.Parse(tokes).ToString();
        }
        public Token ExecuteAndGetToken(string code)
        {
            List<Token> tokes = Token.Tokenize(code);
            tokes = Scanner.Scan(tokes);
            return Parser.Parse(tokes);
        }

        public void ResetSymbols()
        {
            if (null == Parser)
                return;
            Parser.SymbolTable.Reset();
        }
        public void PushSymbols(string name)
        {
            if (null == Parser)
                return;
            Parser.SymbolTable.SaveNamespace(name);
            ResetSymbols();
        }
        public void PopSymbols(string name)
        {
            if (null == Parser)
                return;
            Parser.SymbolTable.LoadNamespace(name);
            Parser.SymbolTable.RemoveNamespace(name);
        }
    }
}
