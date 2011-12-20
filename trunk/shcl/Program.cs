using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Shiro;
using Shiro.Interop;
using Shiro.GhettoCompiler;

namespace shcl
{
    [ShiroClass("Console", KeepClassLoaded = false)]
    class Program
    {
        static bool _exit = false;

        [ShiroMethod("exit", 0)]
        public static void ExitApp()
        {
            _exit = true;
        }

        [ShiroMethod("cls", 0)]
        public static void Cls()
        {
            Console.Clear();
        }

        [ShiroMethod("input", 1)]
        public static string GetInput()
        {
            return Console.ReadLine();
        }

        [ShiroMethod("emit", 1)]
        public static string MsgBox(string msg)
        {
            Console.WriteLine(msg);
            return msg;
        }

        static void Main(string[] args)
        {
            StringBuilder code = new StringBuilder();
            string line = "";
            ShiroInterpret interpreter = new ShiroInterpret();

            interpreter.Execute("use Std");
            interpreter.Execute("use Console");

            if (args.Length == 0 || args[0].StartsWith("-i"))
            {
                Console.WriteLine("shcl running in interactive mode.  Pass filename(s) to execute to run files.");
                Console.WriteLine("Shiro interpreter version: " + interpreter.ShiroVersion);
                Console.WriteLine(" (use the exit() function to leave, double-tap Return to run buffered code)");

                while (!_exit)
                {
                    do
                    {
                        line = Console.ReadLine();
                        code.AppendLine(line);
                    } while (!string.IsNullOrEmpty(line));

                    try
                    {
                        Console.WriteLine("result: " + interpreter.Execute(code.ToString()));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }

                    code = new StringBuilder();
                    line = "";
                }
            }
            else
            {
                bool compile = new List<string>(args).Contains("-c");
                StringBuilder codeToCompile = new StringBuilder();
                string compileFile = "";
                foreach (string fileName in args)
                {
                    if (fileName == "-c")
                        continue;

                    if(compile && string.IsNullOrEmpty(compileFile))
                        compileFile = fileName.Replace(".ml", ".exe");

                    FileStream fs = new FileStream(fileName, FileMode.Open);
                    string codeFromFile = new StreamReader(fs).ReadToEnd();

                    try
                    {
                        if (compile)
                            codeToCompile.AppendLine(codeFromFile);
                        else
                            interpreter.Execute(codeFromFile);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error in file '" + fileName + ": " + ex.Message);
                    }
                }
                if (compile)
                {
                    GhettoCompiler gc = new GhettoCompiler();
                    AssemblyName[] a = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
                    string path = compileFile.Substring(0, compileFile.LastIndexOf("\\"));
                    foreach (AssemblyName an in a)
                        if (an.FullName.ToLower().Contains("interpreter"))
                        {
                            if (!File.Exists(path + "\\Interpreter.dll"))
                                File.Copy(Assembly.Load(an).Location, path + "\\Interpreter.dll");
                        }

                    System.CodeDom.Compiler.CompilerError ce;
                    if (gc.Compile(codeToCompile.ToString(), out ce, compileFile, path))
                        Console.WriteLine("Compile success");
                    else
                        Console.WriteLine("Compile failed");
                }
            }
        }
    }
}