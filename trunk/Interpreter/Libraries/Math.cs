using System;
using System.Collections.Generic;
using System.Text;

using Shiro.Interop;

namespace Shiro.Libraries
{
    [ShiroClass("Math")]
    public class Math
    {
        [ShiroMethod("cos", 1)]
        public string cos(double l)
        {
            return System.Math.Cos(l).ToString();
        }
        [ShiroMethod("fact", 1)]
		public string factorial(double l)
        {
            return innerFactorial(l).ToString();
        }
        protected double innerFactorial(double d)
        {
            if (d == 1)
                return 1;
            else
                return d * (innerFactorial(d - 1));
        }
        [ShiroMethod("sin", 1)]
		public string sin(double l)
        {
            return System.Math.Sin(l).ToString();
        }
        [ShiroMethod("sqrt", 1)]
        public string sqrt(double l)
        {
            return System.Math.Sqrt(l).ToString();
        }
        [ShiroMethod("tan", 1)]
        public string tan(double l)
        {
            return System.Math.Tan(l).ToString();
        }
    }
}
