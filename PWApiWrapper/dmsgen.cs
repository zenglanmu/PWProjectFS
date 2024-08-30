using System;
using System.Runtime.InteropServices;

namespace PWProjectFS.PWApiWrapper
{

    public class dmsgen
    {
        static dmsgen()
        {
            Util.AppendProjectWiseDllPathToEnvironmentPath();
        }

        [DllImport("dmsgen.dll", CharSet = CharSet.Unicode)]
        public static extern int aaApi_GetLastErrorId();

        [DllImport("dmsgen.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetLastErrorMessage")]
        public static extern IntPtr unsafe_aaApi_GetLastErrorMessage();

        public static string aaApi_GetLastErrorMessage()
        {
            return Util.ConvertIntPtrToStringUnicode(unsafe_aaApi_GetLastErrorMessage());
        }

        [DllImport("dmsgen.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetLastErrorDetail")]
        public static extern IntPtr unsafe_aaApi_GetLastErrorDetail();

        public static string aaApi_GetLastErrorDetail()
        {
            return Util.ConvertIntPtrToStringUnicode(unsafe_aaApi_GetLastErrorDetail());
        }

        [DllImport("dmsgen.dll", CharSet = CharSet.Unicode, EntryPoint = "aaApi_GetMessageByErrorId")]
        private static extern IntPtr unsafe_aaApi_GetMessageByErrorId(int errorID);

        public static string aaApi_GetMessageByErrorId(int errorID)
        {
            return Util.ConvertIntPtrToStringUnicode(unsafe_aaApi_GetMessageByErrorId(errorID));
        }

        [DllImport("dmsgen.dll", CharSet = CharSet.Unicode)]
        public static extern bool aaApi_Free(IntPtr pointer);
    }

}
