using System;
using System.Runtime.InteropServices;

namespace PWProjectFS.PWApiWrapper
{
	public class GeoSymbology : IDisposable
	{
		private IntPtr m_pNativeObject;

		[DllImport("geoapi.dll", CharSet = CharSet.Unicode)]
		private static extern IntPtr CreateGeoSymbologyClass();

		[DllImport("geoapi.dll", CharSet = CharSet.Unicode)]
		private static extern void DisposeGeoSymbologyClass(IntPtr pObject);

		[DllImport("geoapi.dll", CharSet = CharSet.Unicode)]
		private static extern int GeoSymbologyCallGetNumericProperty(IntPtr pObject, int idx);

		public GeoSymbology()
		{
			m_pNativeObject = CreateGeoSymbologyClass();
		}

		public GeoSymbology(IntPtr pObject)
		{
			m_pNativeObject = pObject;
		}

		public IntPtr getNativeObject()
		{
			return m_pNativeObject;
		}

		public void Dispose()
		{
			Dispose(bDisposing: true);
		}

		protected virtual void Dispose(bool bDisposing)
		{
			if (m_pNativeObject != IntPtr.Zero)
			{
				DisposeGeoSymbologyClass(m_pNativeObject);
				m_pNativeObject = IntPtr.Zero;
			}
			if (bDisposing)
			{
				GC.SuppressFinalize(this);
			}
		}

		~GeoSymbology()
		{
			Dispose(bDisposing: false);
		}

		public int GetNumericProperty(int idx)
		{
			return GeoSymbologyCallGetNumericProperty(m_pNativeObject, idx);
		}
	}
}


