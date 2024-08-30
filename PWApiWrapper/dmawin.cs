using System;
using System.Runtime.InteropServices;
using System.Text;

namespace PWProjectFS.PWApiWrapper
{
    public class dmawin
    {
        public enum DataSourceType
        {
            Unknown = 0,
            RIS = 1,
            ODBC = 2,
            Informix = 3,
            Ingres = 4,
            Oracle = 5,
            SqlAnywhere = 6,
            SqlServer = 7,
            Sybase = 8,
            DB2 = 9,
            Optinet = 10,
            Solid = 11,
            ODBC_Informix = 12,
            ODBC_Ingres = 16,
            ODBC_Oracle = 20,
            ODBC_SqlAnywhere = 24,
            ODBC_SqlServer = 28,
            ODBC_Sybase = 32,
            ODBC_DB2 = 36,
            ODBC_Solid = 44
        }

        public struct AaDocItem
        {
            public int lProjectId;

            public int lDocumentId;
        }

        static dmawin()
        {
            Util.AppendProjectWiseDllPathToEnvironmentPath();
        }

        [DllImport("dmawin.dll", CharSet = CharSet.Unicode)]
        public static extern bool aaApi_GetDescriptionUsage();

        [DllImport("dmawin.dll", CharSet = CharSet.Unicode)]
        public static extern uint aaApi_LoginDlgExt(IntPtr hWndParent, string lpctstrTitle, uint ulFlags, StringBuilder lptstrDataSource, uint lDSLength, string lpctstrUsername, string lpctstrPassword, string lpctstrSchema);

        [DllImport("DMAWIN.DLL", CharSet = CharSet.Unicode)]
        public static extern int aaApi_SelectProjectDlg2(IntPtr hWndParent, string lpctstrTitle, string lpctstrRootText, int ulFlags, IntPtr hIcon, ref int lplProjectId);
    }

}
