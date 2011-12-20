using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Reflection;
using System.IO;
using Shiro.Interpreter;

namespace Shiro.Interop
{
    public struct ExternFunc
    {
        public string fName;
        public int argCount;
        public MethodInfo method;
        
        public bool usesLoadedClass;
        public string classLibraryName;
    }
    
    public class LibraryTable
    {
        public Dictionary<string, object> Libraries = new Dictionary<string, object>();
        public Dictionary<string, ExternFunc> Functions = new Dictionary<string, ExternFunc>();
		protected Parser Parser = null;

		public LibraryTable(Parser p)
		{
			Parser = p;
		}

        #region Loader

        public bool LoadLibrary(string lib)
        {
            if(LoadAssembly(lib, Assembly.GetExecutingAssembly()))
				return true;

			if (null != Assembly.GetEntryAssembly())
				if (LoadAssembly(lib, Assembly.GetEntryAssembly()))
					return true;

			Error.ReportError("Could not load library: " + lib);
            return false;
        }
        
        public bool LoadLibrary(string library, string file)
        {
            try
            {
                if(!LoadAssembly(library, Assembly.LoadFile(file)))
					Error.ReportError("Could not load library: " + library + ", from file: " + file);
            }
            catch(Exception ex)
            {
                throw new ShiroException("Could not load from file: " + file);
            }
            return true;
        }

        private bool LoadAssembly(string lib, Assembly assembly)
        {
			List<string> reses = new List<string>(assembly.GetManifestResourceNames());
			bool foundAndAdded = false;
			
			//Check Embedded Resources
            foreach(string res in reses)
                if(res.ToLower() == ("Shiro.ShiroLib." + lib + ".src").ToLower())
			    {
				    Stream st = assembly.GetManifestResourceStream(res);
				    st.Seek(0, SeekOrigin.Begin);
				    StreamReader sr = new StreamReader(st);
				    string code = sr.ReadToEnd();
				    Parser.BackDoorParse(code);
				    foundAndAdded = true;
			    }
						
			//Check classes w/ attributes
			foreach (Type cl in assembly.GetTypes())
            {
                object[] attrs = cl.GetCustomAttributes(typeof(ShiroClass), false);
                if (attrs.Length > 0)
                {
                    ShiroClass attr = (ShiroClass)attrs[0];
                    if (attr.libName.ToLower() == lib.ToLower())
                    {
						foundAndAdded = true;
						MethodInfo[] methods = cl.GetMethods();

                        if (attr.KeepClassLoaded)
                        {
                            if (Libraries.ContainsKey(attr.libName))
                                continue;
                            //Get the empty constructor and stick the object in our table
                            object o = cl.GetConstructor(new Type[] { }).Invoke(new object[] { });

							if (o is ShiroBase)
								((ShiroBase)o).RegisterParser(Parser);
							
							Libraries.Add(attr.libName, o);
                        }
                        else
                        {
                            if (!Libraries.ContainsKey(attr.libName))
                                Libraries.Add(attr.libName, null);
                        }

                        foreach (MethodInfo mi in methods)
                        {
                            object[] mattrs = mi.GetCustomAttributes(typeof(ShiroMethod), false);
                            if (mattrs.Length > 0)
                            {
                                ShiroMethod mattr = (ShiroMethod)mattrs[0];
                                ExternFunc ef = new ExternFunc();
                                ef.argCount = mattr.numArgs;
                                ef.fName = mattr.functionName;
                                ef.method = mi;

                                if (ef.usesLoadedClass = attr.KeepClassLoaded)
                                    ef.classLibraryName = attr.libName;

                                if (!Functions.ContainsKey(ef.fName))
                                    Functions.Add(ef.fName, ef);
                            }
                        }
                    }
                }
            }

			return foundAndAdded;
        }
        #endregion

		#region Argument Processors

		private object GetStringArgument(string fName, Token value)
		{
			if (value.vt == Shiro.Interpreter.ValueType.String || value.vt == Shiro.Interpreter.ValueType.Number || value.vt == Shiro.Interpreter.ValueType.Bool)
				return (object)value.token;
			else if (value.vt == Shiro.Interpreter.ValueType.List)
				return (object)value.ToString();
			else 
				Error.ReportError("Error calling interop function '" + fName + "': expected string for argument");

			return null;
		}

		private object GetIntArgument(string fName, Token value)
		{
            if (value.vt == Shiro.Interpreter.ValueType.Number)
            {
                int i;
                if(int.TryParse(value.token, out i))
                    return (object)i;
                else
                    Error.ReportError("Error calling interop function '" + fName + "': underlying function expects int, not double");
            }
            else
                Error.ReportError("Error calling interop function '" + fName + "': expected number for argument");

            return null;
		}

		private object GetDoubleArgument(string fName, Token value)
		{
			if (value.vt == Shiro.Interpreter.ValueType.Number)
				return (object)double.Parse(value.token);
			else
				Error.ReportError("Error calling interop function '" + fName + "': expected number for argument");

			return null;
		}
		private object GetBooleanArgument(string fName, Token value)
		{
			switch (value.vt)
			{
				case Shiro.Interpreter.ValueType.Bool:
					return (object)(value.token == "true");
				case Shiro.Interpreter.ValueType.Number:
					return (object)(value.token != "0");
				case Shiro.Interpreter.ValueType.String:
					return (object)(value.token != "");
				default:
					Error.ReportError("Error calling interop function '" + fName + "': expected boolean for argument");
					return null;
			}
		}
		private object GetNVCArgument(string fName, Token value)
		{
			NameValueCollection retVal = new NameValueCollection();
			if (value.vt != Shiro.Interpreter.ValueType.List)
				Error.ReportError("Error calling interop function '" + fName + "': expected object for argument");
			
			if(!value.IsObject)
				Error.ReportError("Error calling interop function '" + fName + "': expected object for argument (a list is not sufficient, try a tuple)");

			string[] tupes = value.tuple.ToArray();
			int i = 0;
			foreach (string tupe in tupes)
				retVal.Add(tupe, value.list[i++].token);

			return (object)retVal;
		}

		#endregion

		public Token CallLibraryFunction(string name, Token[] argsArray)
        {
            List<Token> args = new List<Token>(argsArray);
			if (!Functions.ContainsKey(name))
                throw new ShiroException("Library function '" + name + "' not found", Parser.CurrentLine, Parser.CurrentPos);

            ExternFunc ef = Functions[name];
            if (args.Count != ef.argCount)
                throw new ShiroException("Incorrect number of arguments passed to interop function '" + name + "'", Parser.CurrentLine, Parser.CurrentPos);

            try
            {
				List<object> processedArgs = new List<object>();

				foreach (ParameterInfo pi in ef.method.GetParameters())
				{
					if (pi.ParameterType == typeof(string))
						processedArgs.Add(GetStringArgument(name, args[0]));
					else if (pi.ParameterType == typeof(double))
						processedArgs.Add(GetDoubleArgument(name, args[0]));
                    else if (pi.ParameterType == typeof(int))
                        processedArgs.Add((GetIntArgument(name, args[0])));
					else if (pi.ParameterType == typeof(bool))
						processedArgs.Add(GetBooleanArgument(name, args[0]));
					else if (pi.ParameterType == typeof(Token))
						processedArgs.Add(args[0]);
					else if (pi.ParameterType == typeof(NameValueCollection))
						processedArgs.Add(GetNVCArgument(name, args[0]));
					else
						Error.ReportError("Invalid Interop Function '" + name + "': Arguments must be of type: string, int, double, bool, Token, NameValueCollection"); 

					args.RemoveAt(0);
				}
				
				if (ef.usesLoadedClass)
                {
                    if (ef.method.ReturnType == typeof(Token))
						return (Token)ef.method.Invoke(Libraries[ef.classLibraryName], processedArgs.ToArray());
                    if (ef.method.ReturnType == typeof(bool))
                        return Token.FromString(((bool)(ef.method.Invoke(Libraries[ef.classLibraryName], processedArgs.ToArray()))).ToString().ToLower());
                    if (ef.method.ReturnType == typeof(string))
                        return Token.FromString((string)ef.method.Invoke(Libraries[ef.classLibraryName], processedArgs.ToArray()));
                    else
                    {
                        ef.method.Invoke(Libraries[ef.classLibraryName], processedArgs.ToArray());
                        return Token.FromString("");
                    }
                }
                else
                {
                    if (ef.method.ReturnType == typeof(Token))
                        return (Token)ef.method.Invoke(null, processedArgs.ToArray());
                    if (ef.method.ReturnType == typeof(bool))
                        return Token.FromString( ((bool)(ef.method.Invoke(null, processedArgs.ToArray()))).ToString().ToLower() );
                    if (ef.method.ReturnType == typeof(string))
                        return Token.FromString((string)ef.method.Invoke(null, processedArgs.ToArray()));
                    else
                    {
                        ef.method.Invoke(null, processedArgs.ToArray());
                        return Token.FromString("");
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is ShiroException)
                    throw ex;
                else
                    //Reflection packages these interestingly
                    throw new ShiroException(ex.InnerException.Message);   
            }
        }
    }
}
