using System;
using System.Runtime.InteropServices;

namespace PWProjectFS.PWApiWrapper
{
    public class ActiveDocumentSymbology : IDisposable
    {
        private IntPtr m_pNativeObject;

        [DllImport("geoapi.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr CreateActiveDocumentSymbologyClass(int viewId);

        [DllImport("geoapi.dll", CharSet = CharSet.Unicode)]
        private static extern void DisposeActiveDocumentSymbologyClass(IntPtr pObject);

        [DllImport("geoapi.dll", CharSet = CharSet.Unicode)]
        private static extern int ActiveDocumentSymbologyCallGetSymbologiesCount(IntPtr pObject);

        [DllImport("geoapi.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr ActiveDocumentSymbologyCallGetSymbology(IntPtr pObject, int idx);

        public ActiveDocumentSymbology(int viewId)
        {
            m_pNativeObject = CreateActiveDocumentSymbologyClass(viewId);
        }

        public ActiveDocumentSymbology(IntPtr pObject)
        {
            m_pNativeObject = pObject;
        }

        public void Dispose()
        {
            Dispose(bDisposing: true);
        }

        protected virtual void Dispose(bool bDisposing)
        {
            if (m_pNativeObject != IntPtr.Zero)
            {
                DisposeActiveDocumentSymbologyClass(m_pNativeObject);
                m_pNativeObject = IntPtr.Zero;
            }
            if (bDisposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        ~ActiveDocumentSymbology()
        {
            Dispose(bDisposing: false);
        }

        public int GetSymbologiesCount()
        {
            return ActiveDocumentSymbologyCallGetSymbologiesCount(m_pNativeObject);
        }

        public GeoSymbology GetSymbology(int idx)
        {
            return new GeoSymbology(ActiveDocumentSymbologyCallGetSymbology(m_pNativeObject, idx));
        }
    }
}

