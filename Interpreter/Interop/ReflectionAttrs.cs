using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Shiro.Interop
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ShiroMethod : Attribute
    {
        public readonly string functionName;
        public readonly int numArgs;

        public ShiroMethod(string funcName, int arCnt)
        {
            this.functionName = funcName;
            this.numArgs = arCnt;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ShiroClass: Attribute
    {
        public readonly string libName;

        public bool KeepClassLoaded = true;

        public ShiroClass(string lib)
        {
            this.libName = lib;
        }
    }

	public abstract class ShiroBase
	{
		protected Shiro.Interpreter.Parser Parser;
		public void RegisterParser(Shiro.Interpreter.Parser p)
		{
			Parser = p;
		}
	}
}
