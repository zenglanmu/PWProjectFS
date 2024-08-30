#define TRACE
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace PWProjectFS.PWApiWrapper
{
    internal class ProcessUtil
    {
        private struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;

            public IntPtr AllocationBase;

            public int AllocationProtect;

            public uint RegionSize;

            public int State;

            public int Protect;

            public int Type;
        }

        private enum MemState
        {
            MEM_COMMIT = 0x1000,
            MEM_PRIVATE = 0x20000,
            MEM_MAPPED = 0x40000,
            MEM_IMAGE = 0x1000000
        }

        private static bool bPerfCtrDisabled;

        [DllImport("kernel32.dll")]
        private static extern int VirtualQuery(IntPtr lpAddress, ref MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [DllImport("kernel32.dll")]
        private static extern bool GetProcessTimes(IntPtr hProcess, ref long lpCreationTime, ref long lpExitTime, ref long lpKernelTime, ref long lpUserTime);

        public static void AppendToEnvironmentPath(string path)
        {
            int num = 32767;
            StringBuilder stringBuilder = new StringBuilder(num);
            string text;
            if (GetEnvironmentVariable("Path", stringBuilder, (uint)num) != 0)
            {
                if (stringBuilder.ToString().IndexOf(path) != -1)
                {
                    return;
                }
                text = stringBuilder.ToString() + ";" + path;
                if (text.Length >= num)
                {
                    throw new ApplicationException("Can not add to 'Path' environment variable because the resulting value would be too long.");
                }
            }
            else
            {
                text = path;
            }
            if (!SetEnvironmentVariable("Path", text))
            {
                throw new ApplicationException("Could not write to 'Path' environment variable.");
            }
        }

        public static uint GetProcessPrivateMemSize(Process process)
        {
            if (!bPerfCtrDisabled)
            {
                try
                {
                    return (uint)process.PrivateMemorySize64;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("ProcessUtil class is considering performance counters disabled because of the following exception in GetProcessPrivateMemSize: " + ex.Message);
                    bPerfCtrDisabled = true;
                }
            }
            MEMORY_BASIC_INFORMATION lpBuffer = default(MEMORY_BASIC_INFORMATION);
            uint num = 0u;
            IntPtr lpAddress = (IntPtr)0;
            uint num2 = (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION));
            while (VirtualQuery(lpAddress, ref lpBuffer, num2) == num2)
            {
                if (lpBuffer.State == 4096 && (lpBuffer.Type == 131072 || lpBuffer.Type == 262144))
                {
                    num += lpBuffer.RegionSize;
                }
                lpAddress = (IntPtr)(uint)((int)lpBuffer.BaseAddress + (int)lpBuffer.RegionSize);
            }
            return num;
        }

        public static string GetProcessName(Process process)
        {
            if (!bPerfCtrDisabled)
            {
                try
                {
                    return process.ProcessName;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("ProcessUtil class is considering performance counters disabled because of the following exception in GetProcessName: " + ex.Message);
                    bPerfCtrDisabled = true;
                }
            }
            string result = null;
            try
            {
                result = process.MainModule.ModuleName;
                return result;
            }
            catch (Exception ex2)
            {
                Trace.WriteLine("Exception throw while resolving ModuleName " + ex2.Message);
                return result;
            }
        }

        public static void GetProcessTimes(Process process, ref DateTime creationTime, ref DateTime exitTime, ref TimeSpan kernelTime, ref TimeSpan userTime)
        {
            if (!bPerfCtrDisabled)
            {
                try
                {
                    creationTime = process.StartTime;
                    exitTime = new DateTime(0L);
                    kernelTime = process.PrivilegedProcessorTime;
                    userTime = process.UserProcessorTime;
                    return;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("ProcessUtil class is considering performance counters disabled because of the following exception in GetProcessTimes: " + ex.Message);
                    bPerfCtrDisabled = true;
                }
            }
            long lpCreationTime = 0L;
            long lpExitTime = 0L;
            long lpKernelTime = 0L;
            long lpUserTime = 0L;
            GetProcessTimes(process.Handle, ref lpCreationTime, ref lpExitTime, ref lpKernelTime, ref lpUserTime);
            creationTime = DateTime.FromFileTime(lpCreationTime);
            exitTime = DateTime.FromFileTime(lpExitTime);
            kernelTime = new TimeSpan(lpKernelTime);
            userTime = new TimeSpan(lpUserTime);
        }

        [DllImport("KERNEL32.dll")]
        private static extern bool SetEnvironmentVariable(string name, string val);

        [DllImport("KERNEL32.dll")]
        private static extern uint GetEnvironmentVariable(string name, StringBuilder valueBuffer, uint bufferSize);
    }

}
