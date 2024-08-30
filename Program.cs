using System;
using System.Windows.Forms;
using PWProjectFS.UI;

namespace PWProjectFS
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            ErrorHandler.bindErrorHandler();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }        
    }
}
