using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Shiro.Interpreter;

namespace ShiroChan.Forms
{
	public partial class SymbolsWin : Form
	{
		protected Parser _parser = null;
		public SymbolsWin(Parser parser)
		{
			_parser = parser;
			InitializeComponent();
		}

		private string GetListContents(List<Token> vsList)
		{
			string s = "{ ";

			foreach (Token vs in vsList)
			{
				if (vs.vt != Shiro.Interpreter.ValueType.List)
					if (vs.vt == Shiro.Interpreter.ValueType.String)
                        s += "'" + vs.token + "'";
					else
                        s += vs.token;
				else
					s += GetListContents(vs.list);
				s += ", ";
			}
			
			return s.TrimEnd(new char[] {',', ' '}) + " }";
		}

		protected DataTable _dt;
		private void SymbolsWin_Load(object sender, EventArgs e)
		{
			_dt = new DataTable();
			_dt.Columns.Add("Symbol");
			_dt.Columns["Symbol"].ReadOnly = true;
			_dt.Columns.Add("Value");

			foreach(string key in _parser.SymbolTable.table.Keys)
				if (_parser.SymbolTable.table[key].vt != Shiro.Interpreter.ValueType.List)
					_dt.Rows.Add(new string[] { key, _parser.SymbolTable.GetSymbolValue(key) });
				else
					_dt.Rows.Add(new string[] {key, GetListContents(_parser.SymbolTable.table[key].list)});

			gridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
			gridView.DataSource = _dt;
		}

		private void gridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
		{
			string varName = _dt.Rows[e.RowIndex][0].ToString();
			string newValue = _dt.Rows[e.RowIndex][e.ColumnIndex].ToString();

			if (newValue.Trim().StartsWith("{") && newValue.Trim().EndsWith("}"))
			{
				List<string> tuple = _parser.SymbolTable.table[varName].tuple;
				_parser.BackDoorParse(varName + " = " + newValue.Trim());
				_parser.SymbolTable.table[varName].tuple = tuple;
			}
			else
			{
				Token t = Token.FromString(newValue);
				_parser.SymbolTable.CreateSymbol(varName, _parser.SymbolTable.table[varName].scope, t.vt, t.token);
			}

		}

		private void DoneBtn_Click(object sender, EventArgs e)
		{
			Hide();
		}

		private void ClearBtn_Click(object sender, EventArgs e)
		{
			FilterText.Text = "";
		}

		private void FilterText_TextChanged(object sender, EventArgs e)
		{
			if (FilterText.Text == "")
				SymbolsWin_Load(sender, e);
			else
			{
				_dt = new DataTable();
				_dt.Columns.Add("Symbol");
				_dt.Columns["Symbol"].ReadOnly = true;
				_dt.Columns.Add("Value");

				foreach (string key in _parser.SymbolTable.table.Keys)
					if(key.ToLower().Contains(FilterText.Text.ToLower()))
					{
						if (_parser.SymbolTable.table[key].vt != Shiro.Interpreter.ValueType.List)
							_dt.Rows.Add(new string[] { key, _parser.SymbolTable.GetSymbolValue(key) });
						else
							_dt.Rows.Add(new string[] { key, GetListContents(_parser.SymbolTable.table[key].list) });
					}

				gridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
				gridView.DataSource = _dt;
			}

		}
	}
}
