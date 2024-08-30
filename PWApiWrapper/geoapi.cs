using System;
using System.Runtime.InteropServices;

namespace PWProjectFS.PWApiWrapper
{
    public class geoapi
    {
        public enum SpatialLocationStringProperties
        {
            SPATIALLOCATION_PROP_SRS_NAME = 4,
            SPATIALLOCATION_PROP_SRS_AUTHNAME,
            SPATIALLOCATION_PROP_SRS_DESCR,
            SPATIALLOCATION_PROP_SRS_ALIAS,
            SPATIALLOCATION_PROP_SRS_WKT,
            SPATIALLOCATION_PROP_SRS_GEOKEYS
        }

        public enum SpatialTypeSystem
        {
            SPATIAL_TYPE_SYSTEM_DMS = 1,
            SPATIAL_TYPE_SYSTEM_ODS
        }

        public enum SpatialDatasourceTypes
        {
            DSTYPE_UNKNOWN,
            DSTYPE_SPATIAL,
            DSTYPE_EXTENDED
        }

        static geoapi()
        {
            Util.AppendProjectWiseDllPathToEnvironmentPath();
        }

        [DllImport("geoapi.dll", CharSet = CharSet.Unicode)]
        public static extern bool aaGeoSpatial_IsSpatialDatasource();

        [DllImport("geoapi.dll", CharSet = CharSet.Unicode)]
        public static extern SpatialDatasourceTypes aaGeoSpatial_GetSpatialDatasourceType();

        [DllImport("geoapi.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr aaGeoSpatial_SelectViewsDataBufferByUser(int userid, bool RetrieveGlobalViews);

        [DllImport("geoapi.dll", CharSet = CharSet.Unicode)]
        public static extern int aaGeoSpatial_GetViewDataBufferNumericProperty(IntPtr buffer, int propertyId, int index);

        [DllImport("geoapi.dll", CharSet = CharSet.Unicode)]
        public static extern bool AddGeoSpatialCriterionToBuffer(ref IntPtr hCriteriaBuf, int lOrGroup, int lFlags, ref Guid lpcPropertySet, string lpctstrPropertyName, int lPropertyId, int lRelationId, int lFieldType, IntPtr geom, ref Guid guidSRS, bool need2edPass);

        [DllImport("geoapi.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr aaGeoSpatial_CreateSpatialLocation([In] ref Guid SRS, [In] IntPtr geometry);

        [DllImport("geoapi.dll", CharSet = CharSet.Unicode)]
        public static extern void aaGeoSpatial_DestroySpatialLocation([In] IntPtr handle);

        [DllImport("geoapi.dll", CharSet = CharSet.Unicode, EntryPoint = "aaGeoSpatial_GetSpatialLocationStringProperty")]
        private static extern IntPtr unsafe_aaGeoSpatial_GetSpatialLocationStringProperty([In] IntPtr handle, [In] SpatialLocationStringProperties propertyID);

        public static string aaGeoSpatial_GetSpatialLocationStringProperty([In] IntPtr handle, [In] SpatialLocationStringProperties propertyID)
        {
            return Util.ConvertIntPtrToStringUnicode(unsafe_aaGeoSpatial_GetSpatialLocationStringProperty(handle, propertyID));
        }

        [DllImport("geoapi.dll", CharSet = CharSet.Unicode)]
        public static extern bool aaGeoSpatial_TransformGeometry([In] ref Guid pSourceSRS, [In] ref Guid pDestinationSRS, [In] IntPtr pGeom, out IntPtr ppOutputGeom);
    }
}

