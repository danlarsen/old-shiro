using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Reflection;

using Shiro;
using Shiro.Interop;
using Shiro.Interpreter;
using Shiro.GhettoCompiler;

using ShiroChan.Properties;

using ScintillaNet;

using ShiroChan.Forms;

namespace ShiroChan
{
	public enum ActiveEdit
	{
		Left = 0,
		Right =1,
		None = 2
	}

	[ShiroClass("Console",KeepClassLoaded=false)]
	public partial class MainForm : Form
	{
		#region Statics / ShiroMethods

		public static ShiroInterpret ShiroInt = null;
		public static new MainForm ActiveForm = null;
		public static List<EditFrame> Frames = new List<EditFrame>();

		[ShiroMethod("exit", 0)]
		public static void ExitApp()
		{
			ActiveForm.Close();
		}

		[ShiroMethod("cls", 0)]
		public static string Cls()
		{
			ActiveForm.LeftText.Text = "";
			return "";
		}

		[ShiroMethod("dbg", 1)]
		public static Token Break(Token t)
		{
			BreakPoint bp = new BreakPoint();
			bp.ExecuteAndWait(t.ToString());
			bp.Hide();
			bp.Close();
			bp = null;
			return t;
		}

		[ShiroMethod("help", 0)]
		public static void showHelp()
		{
			ShiroInt.Execute("executeApp('http://www.shirodev.com/')");
		}

		[ShiroMethod("input", 0)]
		public static string GetInput()
		{
			return Microsoft.VisualBasic.Interaction.InputBox("", "Shiro Input",
				"", MainForm.ActiveForm.Left + 100, MainForm.ActiveForm.Top + 100);
		}

		[ShiroMethod("emit", 1)]
		public static string MsgBox(string msg)
		{
			MessageBox.Show(msg, "Shiro Message");
			return "";
		}

		[ShiroMethod("new", 1)]
		public static string NewFrame(string Title)
		{
			ActiveForm.Tabs.TabPages.Add(Title);
			EditFrame ef = new EditFrame(EditType.Output, EditType.Console);
			Frames.Add(ef);
			return "";
		}

		[ShiroMethod("open", 1)]
		public static string OpenFile(string file)
		{
			int index = 0;
			StreamReader sr = new StreamReader(file);

			if (ActiveForm.ActiveEditor == ActiveEdit.None)
			{
				ActiveForm.RightText.Focus();
				index = 1;
			}
			else if (ActiveForm.ActiveEditor == ActiveEdit.Left)
			{
				ActiveForm.LeftText.Text = sr.ReadToEnd();
				index = 0;
			}
			else
			{
				ActiveForm.RightText.Text = sr.ReadToEnd();
				index = 1;
			}

			Frames[ActiveForm.ActiveFrame].Panels[index].FileName = file;
			ActiveForm.Tabs.SelectedTab.Text = file;

			sr.Close();
			return file;
		}

		[ShiroMethod("save", 0)]
		public static string SaveEditor()
		{
			UpdateFrames();
			if (ActiveForm.ActiveEditor == ActiveEdit.None)
			{
				MessageBox.Show("Please select an editor to save, or use Save All");
				return "";
			}
			string buf = Frames[ActiveForm.ActiveFrame].Panels[(int)ActiveForm.ActiveEditor].Text;
			string file = Frames[ActiveForm.ActiveFrame].Panels[(int)ActiveForm.ActiveEditor].FileName;
			file = file.Trim();

			if (string.IsNullOrEmpty(file))
			{
				SaveFileDialog sfd = new SaveFileDialog();
				sfd.Filter = "Shiro Source (*.src)|*.src|Text Files (*.txt)|*.txt|All Files|*.*";
				DialogResult dr = sfd.ShowDialog(ActiveForm);
				if (!string.IsNullOrEmpty(sfd.FileName))
				{
					Frames[ActiveForm.ActiveFrame].Panels[(int)ActiveForm.ActiveEditor].FileName = sfd.FileName;
					file = sfd.FileName;
				}
				else
					return "";
			}

			StreamWriter sw = new StreamWriter(file, false);
			sw.Write(buf);
			sw.Close();
			return file;
		}

		private static void UpdateFrames()
		{
			Frames[ActiveForm.ActiveFrame].Panels[0].Text = ActiveForm.LeftText.Text;
			Frames[ActiveForm.ActiveFrame].Panels[1].Text = ActiveForm.RightText.Text;
			Frames[ActiveForm.ActiveFrame].DividerScale = ActiveForm.splitter.SplitterDistance;
		}

		#endregion

		#region Properties and CTOR

		public int ActiveFrame = 0;

		public bool OnConsoleTab
		{
			get
			{
				return (Tabs.TabIndex == 0);
			}
		}
		public ActiveEdit ActiveEditor
		{
			get
			{
				if (RightText.Focused)
					return ActiveEdit.Right;
				else if (LeftText.Focused)
					return ActiveEdit.Left;
				else
					return ActiveEdit.None;
			}
		}

		public MainForm()
		{
			InitializeComponent();
			LeftText.GotFocus += new EventHandler(LeftText_GotFocus);
			Show();
			
			Error.SetStandardOutput(StdOut);
			ActiveForm = this;
			Frames.Add(new EditFrame(EditType.Output, EditType.Console));
			SetFrame(0);

			Tabs.Selected += new TabControlEventHandler(OnTabClicked);

			RightText.Focus();
		}

		public void TBSetup()
		{
			SetUpTextBox(RightText);
		}
		
		private void SetUpTextBox(Scintilla tb)
		{
			tb.Margins[0].Width = 30;
			tb.ConfigurationManager.Language = "cs";        //close enough for now
			tb.Lexing.SetKeywords(0, "if to for in else print while try catch where class finally throw select option break twice thrice convert def return use run new is");
			tb.Lexing.LineCommentPrefix = "#";
			//tb.Lexing.LineComment();
			tb.Indentation.ShowGuides = true;
			tb.Indentation.SmartIndentType = SmartIndent.CPP2;

			//config settings (this is as good a place as any)
			if (wordWrapToolStripMenuItem.Checked = Settings.Default.WordWrap)
				tb.LineWrap.Mode = WrapMode.Word;
			else
				tb.LineWrap.Mode = WrapMode.None;

			ErrorToOutputMenu.Checked = Settings.Default.ErrorInConsole;
			allowOutputSelectMenu.Checked = Settings.Default.AllowSelection;
			turnOffIntellisenseToolStripMenuItem.Checked = Settings.Default.TurnOffIntellisense;

			tb.AutoComplete.SingleLineAccept = false;
			tb.AutoComplete.FillUpCharacters = "";
			tb.MatchBraces = true;
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			Settings.Default.ErrorInConsole = ErrorToOutputMenu.Checked;
			Settings.Default.AllowSelection = allowOutputSelectMenu.Checked;
			Settings.Default.WordWrap = wordWrapToolStripMenuItem.Checked;
			Settings.Default.TurnOffIntellisense = turnOffIntellisenseToolStripMenuItem.Checked;
			Settings.Default.Save();
			
			base.OnClosing(e);
		}

		private void SetFrame(int frame)
		{
			UpdateFrames(); 
			ActiveFrame = frame;
			Tabs.SelectedIndex = frame;
			Frames[frame].SetActive(LeftText, RightText, RightTitle, splitter);
			LeftText.Text = Frames[frame].Panels[0].Text;
			RightText.Text = Frames[frame].Panels[1].Text;

			if (frame != 0)
				RightTitle.Text = (string.IsNullOrEmpty(Frames[frame].Panels[1].FileName)) ?
									"Editor Window - Untitled" :
									Frames[frame].Panels[1].FileName;
			else
				RightTitle.Text = "Console";
			
			RightText.Focus();
		}
		private void ConsoleExec(string code)
		{
			try
			{
				ShiroInt.Execute(code);
				symbols = null;
			}
			catch (ShiroException ex)
			{
				try
				{
					if (ErrorToOutputMenu.Checked)
					{
						StdOut(ex.Message);
						StdOut("Line " + ex.Line + ", Position " + ex.Pos);
					}
					else
					{
						MessageBox.Show(ex.Message + ", Line " + ex.Line + ", Position " + ex.Pos, "Shiro Exception");
					}
					RightText.Lines[ex.Line-1].Select ();
					RightText.Selection.Start += ex.Pos;
				}
				catch { }
			}
			finally
			{
				RightText.Focus();
			}
		}
		private StringBuilder _output = new StringBuilder();
		public void StdOut(string msg)
		{
			_output.AppendLine(msg);
			LeftText.Text += msg + Environment.NewLine;
			LeftText.Select(LeftText.Text.Length, 0);
			Application.DoEvents();
		}

		#endregion

		#region Events

		protected void OnTabClicked(object sender, EventArgs e)
		{
			SetFrame(Tabs.SelectedIndex);
		}

		void LeftText_GotFocus(object sender, EventArgs e)
		{
			if(!allowOutputSelectMenu.Checked)
				RightText.Focus();
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter && e.Control)
			{
				runConsoleMenu_Click(this, e);
				e.Handled = true;
			} else 
				base.OnKeyUp(e);
		}

		#endregion

		#region Menu Handlers

		private int UntitledCount = 0;
		private void newFrameMenu_Click(object sender, EventArgs e)
		{
			if (UntitledCount == 0)
				NewFrame("Untitled");
			else
				NewFrame("Untitled-" + UntitledCount);
			UntitledCount++;
			SetFrame(Frames.Count - 1);
			RightText.Text = "";
			LeftText.Text = "";
			UpdateFrames();
		}

		private void runConsoleMenu_Click(object sender, EventArgs e)
		{
			ConsoleExec(RightText.Text);
		}

		private void saveEditorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveEditor();
		}

		private void toCurrToolStripMenuItem_Click(object sender, EventArgs e)
		{
			UpdateFrames(); 
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "Shiro Source (*.src)|*.src|Text Files (*.txt)|*.txt|All Files|*.*";
			ofd.ShowDialog();
			
			if (string.IsNullOrEmpty(ofd.FileName))
				return;
			
			OpenFile(ofd.FileName);
		}

		private void inConsoleToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (ActiveEditor == ActiveEdit.Left)
				ConsoleExec(LeftText.Text);
			else
				ConsoleExec(RightText.Text);
		}
		
		private void runInIsolationToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ShiroInt.PushSymbols("ShiroChan");
			ShiroInt.Execute("use Std");
				if (ActiveEditor == ActiveEdit.Left)
					ConsoleExec(LeftText.Text);
				else
					ConsoleExec(RightText.Text);
				ShiroInt.PopSymbols("ShiroChan");
		}

		private void buildexeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string code = ActiveForm.RightText.Text;
			string res = ShiroInt.IsValidOrGetError(code);
			if (!string.IsNullOrEmpty(res))
			{
				MessageBox.Show("Could not build exe, Shiro reported error in source: " + res);
				return;
			}

			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Filter = "Executable Files (*.exe)|*.exe";
			sfd.Title = "Generate Executable";
			sfd.ShowDialog(ActiveForm);

			if (!string.IsNullOrEmpty(sfd.FileName))
			{
				GhettoCompiler gc = new GhettoCompiler();
				string file = sfd.FileName;
				string path = file.Substring(0, file.LastIndexOf("\\"));

				//Find the Shiro Interpreter and get our new exe a copy of it
				AssemblyName[] a = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
				bool found = false;
				foreach (AssemblyName an in a)
					if (an.FullName.ToLower().Contains("interpreter"))
					{
						if (!File.Exists(path + "\\Interpreter.dll"))
							File.Copy(Assembly.Load(an).Location, path + "\\Interpreter.dll");
						found = true;
					}

				if (!found)
				{
					MessageBox.Show("Could now link to Interpreter.dll, aborting build");
					return;
				}

				System.CodeDom.Compiler.CompilerError ce;
				gc.Compile(code, out ce, file, path);

				if (ce != null)
					MessageBox.Show("Build exe failed: " + ce.ErrorText);
				else
					MessageBox.Show("Build complete!");
			}
		}

		private void runSelectionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (ActiveEditor == ActiveEdit.Left)
				ConsoleExec(LeftText.SelectedText);
			else
				ConsoleExec(RightText.Selection.Text);
		}
		private void runSelectionIsolatedToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ShiroInt.PushSymbols("ShiroChan");
			ShiroInt.Execute("use Std");
				if (ActiveEditor == ActiveEdit.Left)
					ConsoleExec(LeftText.SelectedText);
				else
					ConsoleExec(RightText.Selection.Text);
				ShiroInt.PopSymbols("ShiroChan");
		}
		
		private void clearAllSymbolsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DialogResult dr = MessageBox.Show("This will unload all libaries and uncreate all variables and functions, are you sure?", "Clear All?", MessageBoxButtons.YesNo);
			if(dr == DialogResult.Yes)
				ShiroInt.ResetSymbols();
		}

		private void nextToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				Tabs.SelectedIndex++;
				RightText.Focus();
			}
			catch { }
		}

		private void previousToolStripMenuItem_Click(object sender, EventArgs e)
		{
			try
			{
				Tabs.SelectedIndex--;
				RightText.Focus();
			}
			catch { Tabs.SelectedIndex = 0; }
		}

		private void gotoConsoleToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Tabs.SelectedIndex = 0;
			RightText.Focus();
		}

		private void nameFrameToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string name = Microsoft.VisualBasic.Interaction.InputBox("Enter name for Frame",
				"Name Frame", Tabs.SelectedTab.Text, MainForm.ActiveForm.Left + 100, MainForm.ActiveForm.Top + 100);
			if (!string.IsNullOrEmpty(name))
				Tabs.SelectedTab.Text = name;
		}

		private void wordWrapToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (wordWrapToolStripMenuItem.Checked = !wordWrapToolStripMenuItem.Checked)
				ActiveForm.RightText.LineWrap.Mode = WrapMode.Word;
			else
				ActiveForm.RightText.LineWrap.Mode = WrapMode.None;
		}

		private void ErrorToOutputMenu_Click(object sender, EventArgs e)
		{

		}

		private void allowOutputSelectMenu_Click(object sender, EventArgs e)
		{

		}

		public void helpToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			showHelp();
		}

		private void cutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveForm.RightText.Clipboard.Cut();
		}

		private string SelectedText
		{
			get
			{
				if (!string.IsNullOrEmpty(ActiveForm.RightText.Text))
					return ActiveForm.RightText.Text;
				else if (!string.IsNullOrEmpty(ActiveForm.LeftText.SelectedText))
					return ActiveForm.LeftText.SelectedText;
				return "";
			}
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveForm.RightText.Clipboard.Copy();
		}

		private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveForm.RightText.Clipboard.Paste();
		}

		private void undoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (ActiveForm.RightText.UndoRedo.CanUndo)
				ActiveForm.RightText.UndoRedo.Undo();
		}

		private void redoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (ActiveForm.RightText.UndoRedo.CanRedo)
				ActiveForm.RightText.UndoRedo.Redo();
		}

		private void findToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveForm.RightText.FindReplace.ShowFind();
		}

		private void findNextToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveForm.RightText.FindReplace.FindNext(ActiveForm.RightText.FindReplace.LastFindString);
		}

		private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ActiveForm.RightText.FindReplace.ShowReplace();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
			Application.Exit();
		}

		private SymbolsWin _sw = null;
		private void symbolsWindowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_sw = new SymbolsWin(ShiroInt.Parser);
			_sw.Show(this);
		}

		private bool _consoleVisible = true;
		private void toggleConsoleToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_consoleVisible = !(splitter.Panel2Collapsed = !splitter.Panel2Collapsed);
		}

		private void buildWithoutParseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string code = ActiveForm.RightText.Text;
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Filter = "Executable Files (*.exe)|*.exe";
			sfd.Title = "Generate Executable";
			sfd.ShowDialog(ActiveForm);

			if (!string.IsNullOrEmpty(sfd.FileName))
			{
				GhettoCompiler gc = new GhettoCompiler();
				string file = sfd.FileName;
				string path = file.Substring(0, file.LastIndexOf("\\"));

				//Find the Shiro Interpreter and get our new exe a copy of it
				AssemblyName[] a = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
				bool found = false;
				foreach (AssemblyName an in a)
					if (an.FullName.ToLower().Contains("interpreter"))
					{
						if (!File.Exists(path + "\\Interpreter.dll"))
							File.Copy(Assembly.Load(an).Location, path + "\\Interpreter.dll");
						found = true;
					}

				if (!found)
				{
					MessageBox.Show("Could now link to Interpreter.dll, aborting build");
					return;
				}

				System.CodeDom.Compiler.CompilerError ce;
				gc.Compile(code, out ce, file, path);

				if (ce != null)
					MessageBox.Show("Build exe failed: " + ce.ErrorText);
				else
					MessageBox.Show("Build complete!");
			}
		}

		#endregion

		#region Intellisense (hideous stuff)

		private string intellisenseBuffer = "";
		private static string backup = "";
		private bool dotNotation = false;
		private bool isFunctionCall = false;
		private bool atRoot = true;
		private List<string> dotNotationTuple = null;
		private List<string> symbols = null;
		private List<string> symbolsAndWords = null;

		private bool inBlock = false;
		private char blockEnd = ' ';
		private int lastCount = -1;

		private void RightText_CharAdded(object sender, CharAddedEventArgs e)
		{
			if (turnOffIntellisenseToolStripMenuItem.Checked)
				return;
			
			if (inBlock)
			{
				if (e.Ch == blockEnd)
					inBlock = false;
				
				return;
			}

			try
			{

				switch (e.Ch)
				{
					case '\r':
					case '\n':
						atRoot = true;
						dotNotation = false;
						intellisenseBuffer = "";
						dotNotationTuple = null;
						isFunctionCall = false;
						backup = "";
						ActiveForm.RightText.AutoComplete.Cancel();
						lastCount = -1;
						break;

					//case '{':
					//    inBlock = true;
					//    blockEnd = '}';
					//    intellisenseBuffer = "";
					//    dotNotation = false;
					//    isFunctionCall = false;
					//    ActiveForm.RightText.AutoComplete.Cancel();
					//    break;

					case '#':
						inBlock = true;
						blockEnd = '\n';
						intellisenseBuffer = "";
						dotNotation = false;
						isFunctionCall = false;
						ActiveForm.RightText.AutoComplete.Cancel();
						break;

					case ' ':
					case '\t':
					case '[':
					case ']':
					case '}':
					case '~':
					case '?':
					case '=':
						atRoot = false;
						intellisenseBuffer = "";
						dotNotation = false;
						isFunctionCall = false;
						ActiveForm.RightText.AutoComplete.Cancel();
						lastCount = -1;
						break;

					case '.':
						atRoot = false;
						if (string.IsNullOrEmpty(backup))
							backup = intellisenseBuffer;
						intellisenseBuffer = "";
						dotNotation = true;
						dotNotationTuple = null;
						lastCount = -1;
						isFunctionCall = false;
						ActiveForm.RightText.AutoComplete.Cancel();
						break;

					case '(':
						atRoot = false;
						if (string.IsNullOrEmpty(backup))
							backup = intellisenseBuffer;
						intellisenseBuffer = "";
						dotNotation = false;
						dotNotationTuple = null;
						lastCount = -1;
						isFunctionCall = true;
						ActiveForm.RightText.AutoComplete.Cancel();
						break;

					case ')':
						argumentLabel.Visible = false;
						atRoot = false;
						intellisenseBuffer = "";
						dotNotation = false;
						backup = "";
						ActiveForm.RightText.AutoComplete.Cancel();
						lastCount = -1;
						break;

					default:
						intellisenseBuffer += e.Ch.ToString();
						break;
				}

				//Lazy-load symbols
				if (symbols == null)
				{
					symbolsAndWords = new List<string>("if to for in else print while try catch class finally throw select option break twice thrice convert def return use run new is".Split(' '));
					symbols = new List<string>();

					//functions
					foreach (string function in ShiroInt.Parser.SymbolTable.BackDoorFunctionTable.Keys)
						if (!function.StartsWith("anon"))
							symbols.Add(function);

					//names
					foreach (string name in ShiroInt.Parser.SymbolTable.table.Keys)
						symbols.Add(name);

					//library functions
					foreach (string name in ShiroInt.Parser.SymbolTable.libTab.Functions.Keys)
						symbols.Add(name);

					symbolsAndWords.AddRange(symbols);
				}

				List<string> intellisenseList = new List<string>();
				if (!dotNotation)
				{
					List<string> check = (atRoot ? symbolsAndWords : symbols);
					foreach (string chk in check)
						if (chk.StartsWith(intellisenseBuffer))
							intellisenseList.Add(chk);
				}
				else
				{
					if (dotNotationTuple == null && ShiroInt.Parser.SymbolTable.table.ContainsKey(backup))
						dotNotationTuple = new List<string>(ShiroInt.Parser.SymbolTable.table[backup].tuple.ToArray());

					if (dotNotationTuple != null)
					{
						if (intellisenseBuffer.Length > 0)
						{
							foreach (string chk in dotNotationTuple)
								if (chk.StartsWith(intellisenseBuffer))
									intellisenseList.Add(chk);
						}
						else
						{
							intellisenseList.AddRange(dotNotationTuple);
						}
					}
				}

				if (!ActiveForm.RightText.AutoComplete.IsActive && intellisenseList.Count > 0 && intellisenseList.Count < 25)
				{
					if (lastCount != intellisenseList.Count)
					{
						ActiveForm.RightText.AutoComplete.Cancel();
						ActiveForm.RightText.AutoComplete.Show(intellisenseList);
						lastCount = intellisenseList.Count;
					}
				}

				//If it's a function call, show arguments (see how much I care about my hypothetical end users?)
				if (isFunctionCall && !argumentLabel.Visible)
				{
					string args = "";
					if (ShiroInt.Parser.SymbolTable.BackDoorFunctionTable.ContainsKey(backup))
					{
						FuncSym fs = ShiroInt.Parser.SymbolTable.BackDoorFunctionTable[backup];
						args = BuildArgList(fs.args);
					}
					else if (ShiroInt.Parser.SymbolTable.libTab.Functions.ContainsKey(backup))
					{
						ExternFunc ef = ShiroInt.Parser.SymbolTable.libTab.Functions[backup];
						args = "(";
						for(int i=0; i<ef.argCount; i++)
							args += i.ToString() + ",";

						args = args.TrimEnd(',');
						args += ")    external function";
					}
					else
						return;
					
					argumentLabel.Text = backup + args;
					argumentLabel.Visible = true;
				}
			}
			catch (Exception)
			{ }
		}

		private string GetVTName(Shiro.Interpreter.ValueType vt)
		{
			switch (vt)
			{
				case Shiro.Interpreter.ValueType.Bool:
					return "Bool";

				case Shiro.Interpreter.ValueType.String:
					return "String";

				case Shiro.Interpreter.ValueType.Number:
					return "Num";

				case Shiro.Interpreter.ValueType.Function:
					return "Function";

				case Shiro.Interpreter.ValueType.List:
					return "List";

				default:
					return "<unknown>";
			}
		}

		private string BuildArgList(List<ArgSym> las)
		{
			StringBuilder ret = new StringBuilder("(");

			foreach (ArgSym sym in las)
			{
				ret.Append(sym.Name);
				if (sym.act != ArgConstraintType.None)
				{
					if (sym.act == ArgConstraintType.IsOperator)
						ret.Append(" is ");
					else
						ret.Append(" =? ");

					if (!string.IsNullOrEmpty(sym.baseClass))
						ret.Append(sym.baseClass);
					else
						ret.Append(GetVTName(sym.vt));
				}
				ret.Append(",");
			}

			return ret.ToString().TrimEnd(',') + ")";
		}

		private void RightText_KeyDown(object sender, KeyEventArgs e)
		{
			if (turnOffIntellisenseToolStripMenuItem.Checked)
				return;

			if (e.KeyCode == Keys.Back)
				if (intellisenseBuffer.Length > 0)
					intellisenseBuffer = intellisenseBuffer.Substring(0, intellisenseBuffer.Length - 1);

			lastCount = -1;
		}

		private void RightText_DoubleClick(object sender, EventArgs e)
		{
			if (turnOffIntellisenseToolStripMenuItem.Checked)
				return;

			intellisenseBuffer = "";
			ActiveForm.RightText.AutoComplete.Cancel();
			dotNotation = false;
			intellisenseBuffer = "";
			dotNotationTuple = null;
			ActiveForm.RightText.AutoComplete.Cancel();
			lastCount = -1;
		}
		
		private void RightText_AutoCompleteAccepted(object sender, AutoCompleteAcceptedEventArgs e)
		{
			if (turnOffIntellisenseToolStripMenuItem.Checked)
				return;

			backup = RightText.AutoComplete.SelectedText; // e.Text;
			Text = backup;
		}
		#endregion

		private string lastFileName = "";
		private void toNewEditorToolStripMenuItem_Click_1(object sender, EventArgs e)
		{
			UpdateFrames();
			newFrameMenu_Click(sender, e);
			toCurrToolStripMenuItem_Click(sender, e);
		}
	}
}