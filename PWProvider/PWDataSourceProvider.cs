using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using System.IO;
using PWProjectFS.Log;
using PWProjectFS.PWApiWrapper;

namespace PWProjectFS.PWProvider
{
    public class PWDataSourceProvider
    {
		private object _lock =null;

		//private bool m_LoginStatus=false;

		/* 表示登陆的状态 */
		private string m_userName;
		/* 登陆用户名 */
		private string m_password;
		/* 登陆的密码 */
		private IntPtr m_dsHandle= IntPtr.Zero;
		/* 登陆后的数据库链接 */
		private PWResourceCache m_cache;
		/* 缓存资源 */
		private readonly ProjectHelper m_projectHelper;
		public ProjectHelper ProjectHelper => m_projectHelper;

		private readonly DocumentHelper m_documentHelper;
		public DocumentHelper DocumentHelper => m_documentHelper;

		

		public PWDataSourceProvider()
        {
			this._lock = new object();
			// 默认60秒过期
			this.m_cache = new PWResourceCache(60);
			this.m_projectHelper = new ProjectHelper(this._lock, this.m_cache);
			this.m_documentHelper = new DocumentHelper(this._lock, this.m_cache, this.m_projectHelper);

		}

		public void Initialize()
        {
			Util.AppendProjectWiseDllPathToEnvironmentPath();
			InitializePW();
        }		

		private void InitializePW()
		{
			dmscli.aaApi_Initialize(128);
		}

		public void Uninitialize()
		{
			dmscli.aaApi_Uninitialize();
		}

		public bool Login()
        {
			StringBuilder ds = new StringBuilder(255);
			string username = string.Empty;
			string password = string.Empty;
			string schema = string.Empty;
			long lRetVal = 0;

			lRetVal = dmawin.aaApi_LoginDlgExt(IntPtr.Zero, "ProjectWise登录", 32u, ds, (UInt32)ds.Capacity, username, password, schema);
			if(lRetVal == 1)
            {
				//this.m_LoginStatus = true;
				this.m_dsHandle = dmscli.aaApi_GetActiveDatasource();
				int curloginId = dmscli.aaApi_GetCurrentUserId();
				dmscli.aaApi_SelectUser(curloginId);
				username = dmscli.aaApi_GetUserStringProperty(dmscli.UserProperty.Name, 0);
				var secprovider = dmscli.aaApi_GetUserStringProperty(dmscli.UserProperty.SecProvider, 0);
				var usertype = dmscli.aaApi_GetUserStringProperty(dmscli.UserProperty.Type, 0);
				if(usertype=="W" && !string.IsNullOrWhiteSpace(secprovider))
                {
					this.m_userName = secprovider + "\\" + username;
                }
                else
                {
					this.m_userName = username;
                }
				this.m_password = dmscli.aaApi_GetUserPassword(this.m_userName);
				return true;
            }
            else
            {
				return false;
            }
		}


		/// <summary>
		/// 激活数据源链接，每次调用pw函数时使用
		/// </summary>
		public object Activate()
		{
            lock (_lock)
            {
				IntPtr intPtr = dmscli.aaApi_ActivateDatasourceByHandle(m_dsHandle);
				if (intPtr == IntPtr.Zero)
				{
					ErrorLog.WriteErrorLog("Failed to activate PW DS by handle.");
				}
				else
				{
					dmscli.aaApi_InvalidateConnectionCache();
				}
			}			
			return _lock;
		}

		public bool IsOpen()
		{
			if (m_dsHandle == IntPtr.Zero)
			{
				return false;
			}
			bool isDisconnected = true;
			dmscli.aaApi_DatasourceIsDisconnected(m_dsHandle, ref isDisconnected);
			return !isDisconnected;
		}

		public void InvalidateCache()
        {
			this.m_cache.Invalidate();
        }
	}

	
	/// <summary>
	/// 扩展缓存类，将一些常用的功能放里面
	/// </summary>
	public static class PWCacheExtensions
    {
		public static int GetTimeZoneMinutes(this PWResourceCache cache)
        {
			var cache_key = "GetTimeZoneMinutes";
			int expire_seconds = 3600*24; // 数据库时区，这个基本不会变的
			Func<int> get_value_func = () =>
			{
				return Util.GetTimeZoneMinutes();
			};
			return cache.TryGet(cache_key, get_value_func, expire_seconds);
        }

		public static bool GetDescriptionUsage(this PWResourceCache cache)
        {
			// 用户使用描述还是名称
			var cache_key = "GetDescriptionUsage";
			Func<bool> get_value_func = () =>
			{
				return dmawin.aaApi_GetDescriptionUsage();
			};
			return cache.TryGet(cache_key, get_value_func);
		}

		public static int GetCurrentUserId(this PWResourceCache cache)
		{
			int expire_seconds = 3600 * 24; // 当前登陆用户的id
			var cache_key = "GetCurrentUserId";
			Func<int> get_value_func = () =>
			{
				return dmscli.aaApi_GetCurrentUserId();
			};
			return cache.TryGet(cache_key, get_value_func, expire_seconds);
		}

	}
}
