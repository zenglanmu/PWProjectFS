using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PWProjectFS.PWApiWrapper;

namespace PWProjectFS.PWApiWrapper.CommonDlg
{
	public class FmSelectProjectDlg
	{
		//public static dms_proj ShowDlg(string title, string rootText, int initProjectid)
		//{
		//	int num = initProjectid;
		//	ProjFun.aaApi_SelectProjectDlg2(IntPtr.Zero, title, rootText, 4, IntPtr.Zero, ref num);
		//	if (num > 0)
		//	{
		//		return BL_Proj.GetEntityById(num);
		//	}
		//	return null;
		//}

		public static int ShowDlgToNum(string title, string rootText, int initProjectid)
		{
			int num = initProjectid;
			dmawin.aaApi_SelectProjectDlg2(IntPtr.Zero, title, rootText, 4, IntPtr.Zero, ref num);
			return num;
		}
	}
}
