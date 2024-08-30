using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PWProjectFS.Log;

namespace PWProjectFS
{
    public class ErrorHandler
    {
        public static void bindErrorHandler()
        {
            // 最后的异常兜底，但是目前子线程的exception还是catch不到

            // 不SetUnhandledExceptionMode也能报错倒是，加了在pw环境跑会报invalid operation异常
            // Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            // 先减再加，避免多次加的时候重复了
            Application.ThreadException -= Application_ThreadException;
            Application.ThreadException += Application_ThreadException;

            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            var ex = e.Exception;
            checkAndShowSelfException(ex);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            checkAndShowSelfException(ex);
        }

        private static void checkAndShowSelfException(Exception ex)
        {
            // 处理不了什么异常，只能弹框和记录
            ErrorLog.WriteErrorLog(ex.ToString());
            MessageBox.Show(ex.Message);
            throw ex;
        }
    }
}
