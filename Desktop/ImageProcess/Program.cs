using System;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;

namespace ImageProcess
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        [HandleProcessCorruptedStateExceptions]
        static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException +=
                (sender, args) => HandleUnhandledException(args.ExceptionObject as Exception);
            Application.ThreadException +=
                (sender, args) => HandleUnhandledException(args.Exception);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //try
            {
                Application.Run(new Form1());
            }
//            catch (Exception ex)
//            {
//                MessageBox.Show(ex.Message);
//            }
            
        }

        static void HandleUnhandledException(Exception e)
        {
            MessageBox.Show(e.Message);
        }
    }
}
