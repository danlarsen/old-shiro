using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ShiroChan
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ShiroChan.Forms.SplashScreen splash = new ShiroChan.Forms.SplashScreen();

            splash.Show();

            MainForm main = splash.InitAndGetMainForm();
			splash = null;
            Application.Run(main);
        }
    }
}