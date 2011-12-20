using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ShiroChan.Forms
{
	public partial class BreakPoint : Form
	{
		public BreakPoint()
		{
			InitializeComponent();
		}

		public void ExecuteAndWait(string val)
		{
			ValueLabel.Text = val;
			Continued = false;
			Show();

			while (!Continued)
			{
				Application.DoEvents();
				System.Threading.Thread.Sleep(250);
			}
		}

		protected bool Continued = false;


		private void BreakPoint_Load(object sender, EventArgs e)
		{

		}

		private void Continue_Click(object sender, EventArgs e)
		{
			Continued = true;
		}
	}
}
