using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter;
using System;
using System.Data.OleDb;
using System.Windows.Forms;

namespace StonePlanner
{
    internal static class Program
    {
        private static bool _hideBug = true;
        public static bool EnableErrorCenter = true;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //Connect Microsoft app center
            AppCenter.Start("47eacc02-c48d-43a7-9295-aded8581daba",
                  typeof(Analytics), typeof(Crashes));

            AccessEntity.GetAccessEntityInstance($"{Application.StartupPath}\\data.mdb", "methodbox5");
            // Enable debug mode
            try
            {
                if (args[0] == "-test")
                {
                    _hideBug = false;
                }
            }
            catch(Exception ex) 
            {
                ErrorCenter.AddError(DataType.ExceptionsLevel.Error, ex);
            } 

            Application.EnableVisualStyles();

            if (!_hideBug)
            {
                //处理UI线程异常
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                //处理非UI线程异常
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            }

            Application.Run(new Login());
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ErrorCenter.AddError(DataType.ExceptionsLevel.Error, (Exception) e.ExceptionObject);
           new BugReporter(e.ExceptionObject.ToString()).Show();
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            ErrorCenter.AddError(DataType.ExceptionsLevel.Error, e.Exception);
            new BugReporter(e.Exception.ToString()).Show();
        }
    }
}