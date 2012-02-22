using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Shiro.Interpreter;
using Shiro.Interop;

namespace Shiro.Libraries
{
	public class WorkerThreadParam
	{
		public string Code;
		public int Id;
		public ShiroInterpret Interpreter;
		public bool IsDone = false;
		public bool IsPaused = false;
		public bool Failed = false;
		public string FailReason = "";
		public Token Result;
		public Token LastUpdate = null;
	}
	
	[ShiroClass("Thread", KeepClassLoaded=true)]
	public class Threading : ShiroBase
	{
		protected Dictionary<int, WorkerThreadParam> threads = new Dictionary<int, WorkerThreadParam>();
		protected int LastIndex = 1;

		protected void Worker(object wtp)
		{
			if(! (wtp is WorkerThreadParam))
				throw new ArgumentException("wtp must be a WorkerThreadParam.  This means someone fucked up Thread.cs (probably me)");

			WorkerThreadParam typedWtp = (WorkerThreadParam)wtp;

			Token res = Token.FromString("");
			try
			{
				res = typedWtp.Interpreter.ExecuteAndGetToken(typedWtp.Code);
			}
			catch (Exception ex)
			{
				lock (threads[typedWtp.Id])
				{
					threads[typedWtp.Id].IsDone = true;
					threads[typedWtp.Id].Failed = true;
					threads[typedWtp.Id].FailReason = ex.Message + ((ex.InnerException != null) ? (" : " + ex.InnerException.Message) : "");
					threads[typedWtp.Id].Result = res;
				}
				
				return;
			}
			lock (threads[typedWtp.Id])
			{
				threads[typedWtp.Id].IsDone = true;
				threads[typedWtp.Id].Result = res.Clone();
			}
		}

		[ShiroMethod("updateThreadData", 2)]
		public Token updateThreadData(int internalThreadId, Token data)
		{
			//Note:  Don't call this with the wrong id please.
			lock (threads[internalThreadId])
			{
				threads[internalThreadId].LastUpdate = data.Clone();
			}
			return data;
		}

		[ShiroMethod("threadHasUpdate",1)]
		public bool threadHasUpdate(int internalThreadId)
		{
			lock (threads[internalThreadId])
			{
				return threads[internalThreadId].LastUpdate != null;
			}
		}
		
		[ShiroMethod("getThreadUpdate", 1)]
		public Token getThreadUpdate(int internalThreadId)
		{
			lock (threads[internalThreadId])
			{
				if (threads[internalThreadId].LastUpdate == null)
					return Token.FromString("");
				
				return threads[internalThreadId].LastUpdate.Clone();
			}
		}
		
		[ShiroMethod("threadStart", 2)]
		public string startT(string name, Token globalScope)
		{
			return startTCode(name + "()", globalScope);
		}

		[ShiroMethod("threadStartCode", 2)]
		public string startTCode(string code, Token globalScope)
		{
			ParameterizedThreadStart ts = new ParameterizedThreadStart(Worker);
			Thread t = new Thread(ts);

			WorkerThreadParam wtp = new WorkerThreadParam();
			wtp.Code = code;
			wtp.Id = LastIndex;
			wtp.Interpreter = new ShiroInterpret();

			//clone the symbol table (this is potentially a little scary in terms of performance but it's totally thread safe this way)
			string[] tuple = globalScope.tuple.ToArray();
			for(int i=0;i<tuple.Length; i++)
				wtp.Interpreter.Parser.SymbolTable.table.Add(tuple[i], globalScope.list[i].Clone());

			//Clone the ftab
			foreach (string key in Parser.SymbolTable.BackDoorFunctionTable.Keys)
				wtp.Interpreter.Parser.SymbolTable.BackDoorFunctionTable.Add(key, Parser.SymbolTable.BackDoorFunctionTable[key].Clone());

			wtp.Interpreter.Parser.SymbolTable.libTab = Parser.SymbolTable.libTab;

			//Add global thread id
			wtp.Interpreter.Parser.SymbolTable.CreateSymbol("GlobalThreadId", 0, Interpreter.ValueType.Number, LastIndex.ToString());

			threads.Add(LastIndex, wtp);
			t.Start(wtp);

			LastIndex += 1;
			return (LastIndex-1).ToString();
		}

		[ShiroMethod("threadDone", 1)]
		public bool threadDone(int id)
		{
			lock(threads[id])
			{
				return threads[id].IsDone;
			}
		}
		[ShiroMethod("threadFailed", 1)]
		public bool threadFailed(int id)
		{
			lock (threads[id])
			{
				return threads[id].Failed;
			}
		}
		[ShiroMethod("threadFailReason", 1)]
		public string threadFailReason(int id)
		{
			lock (threads[id])
			{
				return threads[id].FailReason;
			}
		}

		[ShiroMethod("threadResult", 1)]
		public Token threadResult(int id)
		{
			if (!threadDone(id))
				Error.ReportError("Attempt to get threadResult before thread was complete.  Check with threadDone(id)");

			lock(threads[id])
			{
				return threads[id].Result;
			}
		}
	}
}
