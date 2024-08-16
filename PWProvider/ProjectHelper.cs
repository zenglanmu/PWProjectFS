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
        public ulong FreeSpaceAvailable;
        public ulong TotalSpace;
    }

    public class PWProject
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string label { get; set; } // 根据用户的设置，是显示name还是description
        public DateTime create_time { get; set; }
        public DateTime update_time { get; set; }
    }

    public class ProjectHelper
    {
        private object _lock = null;

        public ProjectHelper(object _lock)
        {
            this._lock = _lock;
        }

        /// <summary>
        /// 获取目录对应存储区容量
        /// </summary>
        /// <param name="projectno"></param>
        /// <returns></returns>
        private PWStorageInfo _GetStorageInfo(int projectno)
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
            info.FreeSpaceAvailable = storageAttrs.ullTotalVolumeSize;
            return info;
        }

        public PWStorageInfo GetStorageInfo(int projectno)
        {
            // use lock to ensure thread safe calling pw apis
            lock (this._lock)
            {
                return this._GetStorageInfo(projectno);
            }
        }

        public int ShowSelectProjectDlg()
        {
            var prev_projectno_hint = 0;
            var projectno = FmSelectProjectDlg.ShowDlgToNum("选择要映射的pw目录", null, prev_projectno_hint);
            return projectno;
        }

        private void SelectProject(int projectId)
        {
            switch (dmscli.aaApi_SelectProject(projectId))
            {
                case 0:
                    throw new PWException($"{projectId}对应文件夹不存在");
                case -1:
                    throw PWException.GetPWLastException();
            }
        }

        private PWProject PopulateProjectFromBuffer(int i, int timezone, bool useDescriptions)
        {
            var project = new PWProject();
            project.id = dmscli.aaApi_GetProjectNumericProperty(dmscli.ProjectProperty.ID, i);
            project.name = dmscli.aaApi_GetProjectStringProperty(dmscli.ProjectProperty.Name, i);
            project.description = dmscli.aaApi_GetProjectStringProperty(dmscli.ProjectProperty.Desc, i);
            if (!useDescriptions)
            {
                project.label = project.name;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(project.description))
                {
                    project.label = project.description;
                }
                else
                {
                    project.label = project.name;
                }
            }
            project.create_time = Util.ToUtcTime(dmscli.aaApi_GetProjectStringProperty(dmscli.ProjectProperty.CreateTime, i), timezone);
            project.update_time = Util.ToUtcTime(dmscli.aaApi_GetProjectStringProperty(dmscli.ProjectProperty.UpdateTime, i), timezone);
            return project;
        }


        private PWProject _Read(int projectId)
        {
            SelectProject(projectId);
            int timeZoneMinutes = Util.GetTimeZoneMinutes();
            bool useDescriptions = dmawin.aaApi_GetDescriptionUsage();
            return PopulateProjectFromBuffer(0, timeZoneMinutes, useDescriptions);
        }

        public PWProject Read(int projectId)
        {
            // use lock to ensure thread safe calling pw apis
            lock (this._lock)
            {
                return this._Read(projectId);
            }
        }

        private List<PWProject> _ReadByParent(int parentProjectId)
        {
            
            var projnum = dmscli.aaApi_SelectChildProjects(parentProjectId);
            if (projnum == 0)
            {
                return new List<PWProject>(); // Failed to select properties of all documents. There are no documents in specified project.
            }
            else if (projnum == -1)
            {
                throw PWException.GetPWLastException();
            }
            else
            {
                //pass
            }
            int timeZoneMinutes = Util.GetTimeZoneMinutes();
            bool useDescriptions = dmawin.aaApi_GetDescriptionUsage();
            var projects = new List<PWProject>();
            for (int i = 0; i < projnum; i++)
            {
                var proj = this.PopulateProjectFromBuffer(i, timeZoneMinutes, useDescriptions);
                projects.Add(proj);
            }
            return projects;
        }

        public List<PWProject> ReadByParent(int parentProjectId)
        {
            // use lock to ensure thread safe calling pw apis
            lock (this._lock)
            {
                return this._ReadByParent(parentProjectId);
            }
        }


        /// <summary>
        /// 新建目录
        /// </summary>
        /// <param name="parentProjectId"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        private int _Create(int parentProjectId, string name, string description)
        {
            
            this.SelectProject(parentProjectId);
            var lStorageId = dmscli.aaApi_GetProjectNumericProperty(dmscli.ProjectProperty.StorageID, 0);
            var projItem = new dmscli.AADMSPROJITEM();
            projItem.lptstrName = name;
            projItem.lptstrDesc = description;
            projItem.lProjectId = -1;
            projItem.lParentId = parentProjectId;

            projItem.lManagerId = dmscli.aaApi_GetCurrentUserId();
            projItem.lManagerType = 1;

            //只创建文件夹，非项目，避免项目全局同名
            projItem.lTypeId = 0;
            //使用父目录的存储区设置
            projItem.lStorageId = lStorageId;
            projItem.ulFlags = (UInt32)(dmscli.AADMSPROJITEM_Flag.AADMSPROJF_PARENTID | dmscli.AADMSPROJITEM_Flag.AADMSPROJF_NAME
                    | dmscli.AADMSPROJITEM_Flag.AADMSPROJF_DESC | dmscli.AADMSPROJITEM_Flag.AADMSPROJF_STORAGEID |
                    dmscli.AADMSPROJITEM_Flag.AADMSPROJF_MANAGERID | dmscli.AADMSPROJITEM_Flag.AADMSPROJF_TYPEID |
                    dmscli.AADMSPROJITEM_Flag.AADMSPROJF_MGRTYPE);
            IntPtr pd = Util.StructureToPtr<dmscli.AADMSPROJITEM>(projItem);

            var ret = dmscli.aaApi_CreateProject2(pd, 0);
            
            if (!ret)
            {
                Util.FreeHGlobal(ref pd);
                throw PWException.GetPWLastException();
            }
            projItem = Util.PtrToStructure<dmscli.AADMSPROJITEM>(pd);
            return projItem.lProjectId;
        }

        public int Create(int parentProjectId, string name, string description)
        {
            // use lock to ensure thread safe calling pw apis
            lock (this._lock)
            {
                return this._Create(parentProjectId, name, description);
            }
        }

        private void _Delete(int projectId)
        {
            SelectProject(projectId);
            int lplCount = 0;
            // 删除文件夹以及文件夹下面的文件
            if (!dmscli.aaApi_DeleteProject(projectId, 0, null, 0, ref lplCount))
            {
                throw PWException.GetPWLastException();
            }
        }

        public void Delete(int projectId)
        {
            // use lock to ensure thread safe calling pw apis
            lock (this._lock)
            {
                this._Delete(projectId);
            }
        }

        /// <summary>
        /// 从路径转换成Project的id
        /// </summary>
        /// <param name="lpctstrPath">应该是不带数据源的，类似 aaa\bbb\ccc反斜杠且开始和结尾没有斜杠</param>
        /// <returns></returns>
        private int _GetProjectIdByNamePath(string lpctstrPath)
        {
            var projectno = dmscli.aaApi_GetProjectIdByNamePath(lpctstrPath);
            if (projectno == -1)
            {
                throw PWException.GetPWLastException();
            }
            return projectno;
        }

        public int GetProjectIdByNamePath(string lpctstrPath)
        {
            // use lock to ensure thread safe calling pw apis
            lock (this._lock)
            {
                return this._GetProjectIdByNamePath(lpctstrPath);
            }
        }

        private string _GetNamePathByProjectId(int projectId)
        {
            bool useDescriptions = dmawin.aaApi_GetDescriptionUsage();
            int BufferLen = 65535;
            StringBuilder PWPath = new StringBuilder(BufferLen);
            char tchSeparator = '\\';

            var ret = dmscli.aaApi_GetProjectNamePath2(projectId, useDescriptions, tchSeparator, PWPath, BufferLen);
            if (!ret)
            {
                throw PWException.GetPWLastException();
            }
            return PWPath.ToString();
        }

        public string GetNamePathByProjectId(int projectId)
        {
            // use lock to ensure thread safe calling pw apis
            lock (this._lock)
            {
                return this._GetNamePathByProjectId(projectId);
            }
        }
    }
}
