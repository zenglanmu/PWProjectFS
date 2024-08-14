using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace PWProjectFS.PWApiWrapper
{
	public class Util
	{
		public static string ConvertIntPtrToStringUnicode(IntPtr ptr)
		{
			return Marshal.PtrToStringUni(ptr);
		}

		public unsafe static string ConvertIntPtrToStringAnsi(IntPtr ptr)
		{
			if (ptr.ToInt32() == 0)
			{
				return null;
			}
			sbyte[] array = new sbyte[1024];
			array[0] = 0;
			fixed (sbyte* ptr3 = array)
			{
				sbyte* ptr2 = (sbyte*)ptr.ToPointer();
				int i;
				for (i = 0; ptr2[i] != 0; i++)
				{
					ptr3[i] = ptr2[i];
				}
				ptr3[i] = 0;
				return new string(ptr3);
			}
		}


		/// <summary>
		/// 将托管内存结构体转换到非托管内存
		/// 注意用完要释放，或者用PtrToStructure转换自动释放
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="structure"></param>
		/// <returns></returns>
		public static IntPtr StructureToPtr<T>(T structure) where T : new()
		{
			IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(structure));
			Marshal.StructureToPtr(structure, intPtr, false);
			return intPtr;
		}


		/// <summary>
		/// 将非托管内存里的指针转换成托管内存结构体，并free掉指针内存占用
		/// 因为一般来说转换完就不用指针
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ptr"></param>
		/// <returns></returns>
		public static T PtrToStructure<T>(IntPtr ptr) where T : new()
		{
			T result = (T)((object)Marshal.PtrToStructure(ptr, typeof(T)));
			Marshal.FreeHGlobal(ptr);
			return result;
		}

		public static void FreeHGlobal(ref IntPtr ptr)
		{
			Marshal.FreeHGlobal(ptr);
		}

		public static void AppendToEnvironmentPath(string path)
		{
			ProcessUtil.AppendToEnvironmentPath(path);
		}

		public static string GetProjectWisePath()
		{
			string text = null;
			try
			{
				string[] array = new string[3] { "SOFTWARE\\Bentley\\ProjectWise web services", "SOFTWARE\\Bentley\\ProjectWise Explorer", "SOFTWARE\\Bentley\\ProjectWise Administrator" };
				string[] array2 = array;
				foreach (string name in array2)
				{
					RegistryKey localMachine = Registry.LocalMachine;
					RegistryKey registryKey = localMachine.OpenSubKey(name);
					if (registryKey == null)
					{
						continue;
					}
					string[] subKeyNames = registryKey.GetSubKeyNames();
					if (subKeyNames == null)
					{
						continue;
					}
					for (int j = 0; j < subKeyNames.Length; j++)
					{
						RegistryKey registryKey2 = registryKey.OpenSubKey(subKeyNames[j]);
						if (registryKey2 != null)
						{
							text = (string)registryKey2.GetValue("PathName");
						}
					}
					if (text != null)
					{
						break;
					}
				}
				if (text == null)
				{
					throw new ApplicationException("Registry search could not find installation directory for a ProjectWise web services. \nMake sure a ProjectWise web services is installed on this system.");
				}
				return text;
			}
			catch (Exception ex)
			{
				try
				{
					EventLog eventLog = new EventLog("Application", ".", "ProjectWise .NET API Wrapper");
					eventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
					return text;
				}
				catch
				{
					return text;
				}
			}
		}

		public static void AppendProjectWiseDllPathToEnvironmentPath()
		{
			try
			{
				string projectWisePath = GetProjectWisePath();
				if (projectWisePath != null)
				{
					projectWisePath += "\\bin";
					ProcessUtil.AppendToEnvironmentPath(projectWisePath);
				}
			}
			catch (Exception ex)
			{
				try
				{
					EventLog eventLog = new EventLog("Application", ".", "ProjectWise .NET API Wrapper");
					eventLog.WriteEntry(ex.Message, EventLogEntryType.Error);
				}
				catch
				{
				}
			}
		}
	}
}


