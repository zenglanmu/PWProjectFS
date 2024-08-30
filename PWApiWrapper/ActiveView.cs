using System;
using System.Runtime.InteropServices;

namespace PWProjectFS.PWApiWrapper
{
    public class ActiveView : IDisposable
    {
        private IntPtr m_pNativeObject;

        [DllImport("geoapi.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr CreateActiveViewClass();

        [DllImport("geoapi.dll", CharSet = CharSet.Unicode)]
        private static extern void DisposeActiveViewClass(IntPtr pObject);

        [DllImport("geoapi.dll", CharSet = CharSet.Unicode)]
        private static extern bool ActiveViewCallLoadView(IntPtr pObject, int viewId, int flags);

        [DllImport("geoapi.dll", CharSet = CharSet.Unicode)]
        private static extern int ActiveViewCallGetDocumentSymbologyID(IntPtr pObject, IntPtr buffer, int idx);

        [DllImport("geoapi.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr ActiveViewCallGetDocumentSymbology(IntPtr pObject);

        public ActiveView()
        {
            m_pNativeObject = CreateActiveViewClass();
        }

        public void Dispose()
        {
            Dispose(bDisposing: true);
        }

        protected virtual void Dispose(bool bDisposing)
        {
            if (m_pNativeObject != IntPtr.Zero)
            {
                DisposeActiveViewClass(m_pNativeObject);
                m_pNativeObject = IntPtr.Zero;
            }
            if (bDisposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        ~ActiveView()
        {
            Dispose(bDisposing: false);
        }

        public bool LoadView(int viewId, int flags)
        {
            return ActiveViewCallLoadView(m_pNativeObject, viewId, flags);
        }

        public int GetDocumentSymbologyID(IntPtr buffer, int idx)
        {
            return ActiveViewCallGetDocumentSymbologyID(m_pNativeObject, buffer, idx);
        }

        public ActiveDocumentSymbology GetDocumentSymbology()
        {
            return new ActiveDocumentSymbology(ActiveViewCallGetDocumentSymbology(m_pNativeObject));
        }
    }

}

