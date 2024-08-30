using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace PWProjectFS.PWApiWrapper
{

    public class SymbologyIcon : IDisposable
    {
        private IntPtr m_pNativeObject;

        [DllImport("geowin.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr CreateSymbologyIconClass(IntPtr pSymbology);

        [DllImport("geowin.dll", CharSet = CharSet.Unicode)]
        private static extern void DisposeSymbologyIconClass(IntPtr pObject);

        [DllImport("geowin.dll", CharSet = CharSet.Unicode)]
        private static extern int SymbologyIconCallExtract(IntPtr pObject, int flags);

        [DllImport("geowin.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SymbologyIconCallGetSmallIcon(IntPtr pObject);

        public SymbologyIcon(GeoSymbology symbology)
        {
            m_pNativeObject = CreateSymbologyIconClass(symbology.getNativeObject());
        }

        public void Dispose()
        {
            Dispose(bDisposing: true);
        }

        protected virtual void Dispose(bool bDisposing)
        {
            if (m_pNativeObject != IntPtr.Zero)
            {
                DisposeSymbologyIconClass(m_pNativeObject);
                m_pNativeObject = IntPtr.Zero;
            }
            if (bDisposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        ~SymbologyIcon()
        {
            Dispose(bDisposing: false);
        }

        public int Extract(int flags)
        {
            return SymbologyIconCallExtract(m_pNativeObject, flags);
        }

        public Icon GetSmallIcon()
        {
            return Icon.FromHandle(SymbologyIconCallGetSmallIcon(m_pNativeObject));
        }
    }
}

