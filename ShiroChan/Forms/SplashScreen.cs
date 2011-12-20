using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Shiro;

namespace ShiroChan.Forms
{
    public partial class SplashScreen : Form
    {
        public SplashScreen()
        {
            InitializeComponent();
        }

        protected void UpdateStatus(int pctg, string status)
        {
            Status.Text = status;
            progress.Value = pctg;
            Application.DoEvents();
            System.Threading.Thread.Sleep(350);
        }

        internal MainForm InitAndGetMainForm()
        {
            MainForm ret = null;
            this.TopMost = true;

            UpdateStatus(20, "Loading main form");
            ret = new MainForm();
            System.Threading.Thread.Sleep(0);

            UpdateStatus(40, "Loading Shiro");
            MainForm.ShiroInt = new Shiro.ShiroInterpret();
            ret.Text += ", Shiro Version " + MainForm.ShiroInt.ShiroVersion;
            System.Threading.Thread.Sleep(0);

            UpdateStatus(60, "Loading Shiro Standard Library");
            MainForm.ShiroInt.Execute("use Std");
            System.Threading.Thread.Sleep(0);

            UpdateStatus(80, "Loading Shiro-chan Interop Libraries into Shiro");
            MainForm.ShiroInt.Execute("use Console");
            System.Threading.Thread.Sleep(0);

            System.Threading.Thread.Sleep(50);
            UpdateStatus(90, "Configuring Scintilla Editor");
			MainForm.ActiveForm.TBSetup();
            System.Threading.Thread.Sleep(0);

            Hide();
            return ret;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}