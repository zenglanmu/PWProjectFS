using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PWProjectFS.PWApiWrapper;
using PWProjectFS.PWApiWrapper.CommonDlg;

namespace PWProjectFS.PWProvider
{
    public class PWStorageInfo
    {
        public string Name;
        public string Desc;
        public ulong FreeSpace;
        public ulong TotalSpace;
    }
    public class ProjectHelper
    {

        /// <summary>
        /// 获取目录对应存储区容量
        /// </summary>
        /// <param name="projectno"></param>
        /// <returns></returns>
        public PWStorageInfo GetStorageInfo(int projectno)
        {
            dmscli.aaApi_SelectProject(projectno);
            int lStorageId = dmscli.aaApi_GetProjectNumericProperty(dmscli.ProjectProperty.StorageID, 0);
            dmscli.aaApi_SelectStorage(lStorageId);
            var info = new PWStorageInfo();
            info.Name = dmscli.aaApi_GetStorageStringProperty(dmscli.StorageProperty.Name, 0);
            info.Desc = dmscli.aaApi_GetStorageStringProperty(dmscli.StorageProperty.Desc, 0);
            var storageAttrs = dmscli.aaApi_GetStorageAttributes(lStorageId);
            info.FreeSpace = storageAttrs.ullUserFreeSpace;
            info.TotalSpace = storageAttrs.ullUserVolumeSize;
            return info;
        }

        public int SelectProject()
        {
            var prev_projectno_hint = 0;
            var projectno = FmSelectProjectDlg.ShowDlgToNum("选择要映射的pw目录", null, prev_projectno_hint);
            return projectno;
        }
    }
}
