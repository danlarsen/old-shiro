using System;
using System.Collections.Generic;
using System.Text;
using Shiro.Libraries;
using Shiro.Interop;

namespace Shiro.Interpreter
{
	#region Structs and Enums

	public enum ArgConstraintType
	{
		None,
		IsOperator,
		Equivalence
	}
	public class ArgSym
	{
		public string Name;
		public Token DefaultValue;
		public bool IsCatchAll = false;
		public ValueType vt = ValueType.NotAValue;
		public ArgConstraintType act = ArgConstraintType.None;

		public string baseClass = "";

		public ArgSym(string name)
		{
			Name = name;
		}
		public ArgSym(string name, string val)
		{
			Name = name;
			DefaultValue = Token.FromString(val);
		}
	}

	public class FuncSym
	{
		public List<ArgSym> args;
		public List<Token> body;

		public bool isLibraryFunction;

		public FuncSym Clone()
		{
			FuncSym fs = new FuncSym();
			fs.args = args;
			fs.body = Token.CloneList(body);
			fs.isLibraryFunction = isLibraryFunction;
			return fs;
		}
	}

	public struct Namespace
	{
		public Dictionary<string, Token> Symbols;
		public Dictionary<string, FuncSym> Functions;
		internal LibraryTable Libraries;
	}
	#endregion

	public class SymbolTable
	{
		public int Globalscope = 0;
		private int _scope = 0;
		public int scope
		{
			get
			{
				return _scope;
			}
			set
			{
				_scope = value;
				KillToscope(_scope);
			}
		}

		protected Parser CurrentParser = null;
		public SymbolTable(Parser p)
		{
			CurrentParser = p;
			libTab = new LibraryTable(CurrentParser);
		}

		public Dictionary<string, Token> table = new Dictionary<string, Token>();
		public Dictionary<string, FuncSym> fTab = new Dictionary<string, FuncSym>();
		public LibraryTable libTab = null;

		#region Namespace

		public Dictionary<string, Namespace> _spaces = new Dictionary<string, Namespace>();

		public void SaveNamespace(string name)
		{
			if (_spaces.ContainsKey(name))
				throw new ShiroException("Namespace " + name + " already exists");

			Namespace ns;
			ns.Functions = fTab;
			ns.Libraries = libTab;
			ns.Symbols = table;
			_spaces.Add(name, ns);
		}
		public void LoadNamespace(string name)
		{
			if (!_spaces.ContainsKey(name))
				throw new ShiroException("Namespace " + name + " not found");

			Namespace ns = _spaces[name];
			fTab = ns.Functions;
			libTab = ns.Libraries;
			table = ns.Symbols;
		}
		public void RemoveNamespace(string name)
		{
			if (_spaces.ContainsKey(name))
				_spaces.Remove(name);
		}

		#endregion

		#region Intermediate/Debug

		public string GetSymbolDump()
		{
			StringBuilder ret = new StringBuilder();
			ret.AppendLine("NAME (type/scope): value" + Environment.NewLine);

			foreach (string key in table.Keys)
				ret.AppendLine(key + " (" + table[key].vt + "/" + table[key].scope + "): " + table[key].token);

			return ret.ToString();
		}

		public void Reset()
		{
			table = new Dictionary<string, Token>();
			fTab = new Dictionary<string, FuncSym>();
			libTab = new LibraryTable(CurrentParser);
		}

		#endregion

		#region Variable Stuff

		public bool IsInTable(string name)
		{
			return table.ContainsKey(name);
		}

		public ValueType GetSymbolType(string name)
		{
			if (!IsInTable(name))
				return default(ValueType);

			return table[name].vt;
		}
		public string GetSymbolValue(string name)
		{
			if (!IsInTable(name))
				return string.Empty;

			return table[name].token;
		}
		public List<Token> GetSymbolList(string name)
		{
			if (!IsInTable(name))
				return null;

			return table[name].list;
		}

		public bool SetSymbolValue(string name, string val)
		{
			if (!IsInTable(name))
				return false;

			Token vs = table[name];
			vs.token = val;
			table[name] = vs;
			return true;
		}

		public void CreateSymbol(string name, int scope, ValueType vt, string value)
		{
			Token vs = new Token();
			vs.token = value;
			vs.scope = scope;
			vs.vt = vt;

			if (IsInTable(name))
			{
				vs.scope = table[name].scope;
				table.Remove(name);
			}

			table.Add(name, vs);
		}
		public void CreateListSymbol(string name, int scope, List<Token> value)
		{
			Token vs = new Token();
			vs.token = "";
			vs.list = value;
			vs.scope = scope;
			vs.vt = ValueType.List;

			if (IsInTable(name))
			{
				vs.scope = table[name].scope;
				table.Remove(name);
			}

			table.Add(name, vs);
		}
		public void CreateListSymbol(string name, int scope, List<Token> value, string tuple, string baseClass)
		{
			CreateListSymbol(name, scope, value, new List<string>(tuple.Split(new string[] {" | "}, StringSplitOptions.RemoveEmptyEntries)), baseClass);
			
			Token vs = new Token();
			vs.token = "";
			vs.list = value;
			vs.scope = scope;
			vs.vt = ValueType.List;
			vs.tuple = new List<string>(tuple.Split(new string[] {" | "}, StringSplitOptions.RemoveEmptyEntries));
			vs.baseClass = baseClass;

			if (IsInTable(name))
			{
				vs.scope = table[name].scope;
				table.Remove(name);
			}
			table.Add(name, vs);
		}
		public void CreateListSymbol(string name, int scope, List<Token> value, List<string> tuple, string baseClass)
		{
			Token vs = new Token();
			vs.token = "";
			vs.list = value;
			vs.scope = scope;
			vs.vt = ValueType.List;
			vs.tuple = tuple;
			vs.baseClass = baseClass;

			if (IsInTable(name))
			{
				vs.scope = table[name].scope;
				table.Remove(name);
			}
			table.Add(name, vs);
		}
		public void CreateConstantSymbol(string name, int scope, ValueType vt, string value)
		{
			Token vs = new Token();
			vs.token = value;
			vs.scope = scope;
			vs.vt = vt;

			if (IsInTable(name))
				return;

			table.Add(name, vs);
		}

		public void RemoveSymbol(string name)
		{
			if(table.ContainsKey(name))
				table.Remove(name);
		}

		public int GetTupleIndexInline(List<Token> vals, List<string> tuple, string index)
		{
			return GetTupleIndexInline(vals, tuple, index, true);
		}
		public int GetTupleIndexInline(List<Token> vals, List<string> tuple, string index, bool throwIfNotFound)
		{
			Token vs = new Token(vals);
			vs.tuple = tuple;

			if (!vs.IsObject)
				if (throwIfNotFound)
					Error.ReportError("{Cannot Show} is not an object, cannot access member '" + index + "'");
				else
					return -1;

			if (!vs.tuple.Contains(index))
				if (throwIfNotFound)
					Error.ReportError("Member '" + index + "' not found.");
				else
					return -1;

			return vs.tuple.IndexOf(index);
		}

		public int GetTupleIndex(string name, string index)
		{
			if(!IsInTable(name))
				Error.ReportError("Symbol not found: " + name);

			Token vs = table[name];

			if (!vs.IsObject)
				Error.ReportError("'" + name + "' is not an object, cannot access member '" + index + "'");

			List<string> tups = new List<string>(vs.tuple.ToArray());
			if (!tups.Contains(index))
				Error.ReportError("Member '" + index + "' not found.");
			
			return tups.IndexOf(index);
		}
		public int GetTupleSubIndex(string name, int baseIndex, string index)
		{
			Token vs = table[name].list[baseIndex];

			if (!vs.IsObject)
				Error.ReportError("'" + name + "' is not a Class, cannot access member '" + index + "'");

			List<string> tups = new List<string>(vs.tuple.ToArray());
			if (!tups.Contains(index))
				Error.ReportError("Member '" + index + "' not found.");

			return tups.IndexOf(index);
		}

		public void KillToscope(int scope)
		{
			if (scope < 0)
				scope = 0;

			try
			{
				List<string> keysToDelete = new List<string>();

				foreach (string key in table.Keys)
					if (table[key].scope > scope)
						keysToDelete.Add(key);

				foreach (string delete in keysToDelete)
					table.Remove(delete);
			}
			catch (Exception)
			{

			}
		}
		#endregion

		#region Functions and Libraries (/sigh)

		public bool IsInFTab(string name)
		{
			return (fTab.ContainsKey(name) || libTab.Functions.ContainsKey(name));
		}
		public void AddFunction(string name, List<Token> body, List<ArgSym> args)
		{
			FuncSym fs = new FuncSym();
			fs.args = args;
			fs.body = body;
			fs.isLibraryFunction = false;

			fTab.Add(name, fs);
		}
		public void RemoveFunction(string name)
		{
			if (IsInFTab(name))
				fTab.Remove(name);
		}

		public bool IsLibraryFunction(string name)
		{
			if (!IsInFTab(name))
				return false;
			else
				return libTab.Functions.ContainsKey(name);
		}

		public bool LoadLibrary(string library)
		{
			return libTab.LoadLibrary(library);
		}
		
		public bool LoadLibrary(string library, string file)
		{
			return libTab.LoadLibrary(library, file);
		}

		public Token CallLibraryFunction(string name, Token[] args)
		{
			if (!IsInFTab(name))
				return default(Token);
			if (!IsLibraryFunction(name))
				return default(Token);

			try
			{
				return libTab.CallLibraryFunction(name, args);
			}
			catch (Exception ex)
			{
				Error.ReportError("Interop Exception calling '" + name + "':  " + ex.Message);
			}
			return default(Token);
		}

		public List<ArgSym> GetFArgs(string name)
		{
			return fTab[name].args;
		}
		public List<Token> GetFBody(string name)
		{
			return fTab[name].body;
		}
		#endregion

		public bool IsClass(string p)
		{
			if (!IsInTable(p))
				return false;
			else
				return table[p].isClass;
		}
	}
}
