using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

using Microsoft.CSharp;
using System.CodeDom.Compiler;

namespace Shiro.GhettoCompiler
{
    public class GhettoCompiler
    {
        private string CreateCSharpString(string code)
        {
            StringBuilder sb = new StringBuilder();

            code = code.Replace("\r\n", "\n").Replace("\t", " ");
            string[] lines = code.Split(new char[] { '\n', '\r' }, StringSplitOptions.None);

            string lineTemplate = "@\"##\" + Environment.NewLine + ";

            foreach(string line in lines)
                sb.AppendLine(lineTemplate.Replace("##", line.Replace("\"", "\"+DQ+\"")));

            sb.Append("\"\"");
            return sb.ToString();
        }

        // Compile the specified source code to an in-memory assembly.
        public bool Compile(string shiroSource, out CompilerError compilerError, string outFile, string path)
        {
            bool result = true;
            compilerError = null;

			Stream st = Assembly.GetExecutingAssembly().GetManifestResourceStream("Shiro.GhettoCompiler.compilerMerge.txt");
			st.Seek(0, SeekOrigin.Begin);
			StreamReader sr = new StreamReader(st);
			string sourceCode = sr.ReadToEnd();

            sourceCode = sourceCode.Replace("##CODE##", CreateCSharpString(shiroSource));

            CSharpCodeProvider cSharpProvider = new CSharpCodeProvider();
            ICodeCompiler codeCompiler = cSharpProvider.CreateCompiler();

            // Create a CompilerParameters object that specifies assemblies referenced
            //  by the source code and the compiler options chosen by the user.
            CompilerParameters cp = new CompilerParameters(new string[] { "System.dll", path  + @"\Interpreter.dll" }, outFile);

            cp.GenerateExecutable = true;
            cp.IncludeDebugInformation = false;
            cp.GenerateInMemory = false;
            cp.TreatWarningsAsErrors = false;
            cp.CompilerOptions = "/optimize";
            cp.MainClass = "SemiCompiled.Program";

            try
            {
                // Compile the source code.
                CompilerResults compilerResults = codeCompiler.CompileAssemblyFromSource(cp, sourceCode);
                
                // Check for errors.
                if (compilerResults.Errors.Count > 0)
                {
                    compilerError = compilerResults.Errors[0];
                    result = false;
                }
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }
    }
}
