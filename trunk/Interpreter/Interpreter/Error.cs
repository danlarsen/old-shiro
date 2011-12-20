using System;
using System.Collections.Generic;
using System.Text;

using Shiro.Interpreter;

namespace Shiro
{
    public class ShiroException : Exception
    {
        public int Line, Pos;
        public ShiroException(string msg, int line, int pos)
            : base(msg)
        {
            Line = line;
            Pos = pos;
        }
        public ShiroException(string msg)
            : base(msg)
        {
            Line = 0;
            Pos = 0;
        }
    }

    public delegate void ShiroStdOut(string msg);

    public static class Error
    {
        public static void ReportError(string error)
        {
            throw new ShiroException(error, Parser.CurrentLine, Parser.CurrentPos);
        }
        public static void StandardOut(string text)
        {
            if (null == _out)
                Console.WriteLine(text);
            else
                _out(text);
        }
        public static void ErrorOut(string text)
        {
            Console.WriteLine(text);
        }

        public static void SetStandardOutput(ShiroStdOut handler)
        {
            _out = handler;
        }
        
        public static ShiroStdOut _out = null;
    }
}
