using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DokanNet;
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
        public int parentid { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string label { get; set; } // 根据用户的设置，是显示name还是description
        public DateTime create_time { get; set; }
        public DateTime update_time { get; set; }

        public FileInformation toFileInformation()
        {
            var dir = new FileInformation
            {
                Attributes = FileAttributes.Directory,
                CreationTime = this.create_time,
                LastAccessTime = this.update_time,
                LastWriteTime = this.update_time,
                Length = 0,
                FileName = this.label
            };
            return dir;
        }
    }

    public class ProjectHelper
    {
        private object _lock = null;
        private PWResourceCache m_cache;
        /* 缓存资源 */
        public ProjectHelper(object _lock, PWResourceCache cache)
        {
            this._lock = _lock;
            this.m_cache = cache;
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
                var cache_key = "StorageInfo";
                Func<PWStorageInfo> get_value_func = () =>
                {
                    return this._GetStorageInfo(projectno);
                };
                // 容量大小的缓存可以保存时间长点
                return this.m_cache.TryGet(cache_key, get_value_func, 60 * 10);
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

        private PWProject PopulateProjectFromBuffer(int i)
        {
            int timezone = this.m_cache.GetTimeZoneMinutes();
            bool useDescriptions = this.m_cache.GetDescriptionUsage();
            var project = new PWProject();
            project.id = dmscli.aaApi_GetProjectNumericProperty(dmscli.ProjectProperty.ID, i);
            project.parentid = dmscli.aaApi_GetProjectNumericProperty(dmscli.ProjectProperty.ParentID, i);
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
            return PopulateProjectFromBuffer(0);
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
            int projnum = 0;
            if (parentProjectId > 0)
            {
                projnum = dmscli.aaApi_SelectChildProjects(parentProjectId);
            }
            else
            {
                projnum = dmscli.aaApi_SelectTopLevelProjects();
            }
            
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

            var projects = new List<PWProject>();
            for (int i = 0; i < projnum; i++)
            {
                var proj = this.PopulateProjectFromBuffer(i);
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

            projItem.ulFlags = (uint)(
                dmscli.AADMSProjFlags.ParentId |
                dmscli.AADMSProjFlags.Name |
                dmscli.AADMSProjFlags.Desc |                
                dmscli.AADMSProjFlags.ManagerId |
                dmscli.AADMSProjFlags.Mgrtype |
                dmscli.AADMSProjFlags.TypeId |
                dmscli.AADMSProjFlags.StorageId                
                );
            IntPtr pd = Util.StructureToPtr(projItem);

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

        public int CreateByFullPath(string projFullPath)
        {
            var parentPath = projFullPath.Substring(0, projFullPath.LastIndexOf("\\"));            
            var parentProjectId = this.GetProjectIdByNamePath(parentPath);
            if (parentProjectId == -1)
            {
                // 父目录不存在，好让上层调用方知道
                return -1;
            }
            // use lock to ensure thread safe calling pw apis
            lock (this._lock)
            {                
                // 拼接上pw的父路径，获取完整的pw父路径
                var basename = projFullPath.Substring(projFullPath.LastIndexOf("\\") + 1);
                var projectno = this._Create(parentProjectId, basename, basename);

                var cache_key = $"GetProjectIdByNamePath:{projFullPath}";
                // 更新缓存，防止创建完后调用获取api不存在
                // 因为创建前很可能调用了GetProjectIdByNamePath，设置了不存在的缓存
                this.m_cache.Set(cache_key, projectno);
                return projectno;
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
                var cache_key = $"GetNamePathByProjectId:{projectId}";
                this.m_cache.Delete(cache_key);
                var cache_key_pattern = "GetProjectIdByNamePath:*";
                this.m_cache.DeleteByValue(projectId, cache_key_pattern);
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
            // 如果lpctstrPath不存在，抛错误码50000
            if (projectno == -1)
            {
                throw PWException.GetPWLastException();
            }
            return projectno;
        }


        /// <summary>
        /// 根据label名称取值
        /// </summary>
        /// <param name="label"></param>
        /// <param name="parentProjectId"></param>
        /// <returns></returns>
        private PWProject _GetProjectByNameAndProjectId(string label, int parentProjectId)
        {
            // aaApi_SelectProjectsByStruct 这个方法一直报缓冲区大小不够的错误，所以只好都出子目录再匹配
            var projects = this._ReadByParent(parentProjectId);
            foreach(var proj in projects)
            {
                if (proj.label == label)
                {
                    return proj;
                }
            }
            return null;
        }

        /// <summary>
        /// 从路径反推文件夹id，如果不存在，返回-1
        /// aaApi_GetProjectIdByNamePath对于超长的会返回不正确的结果
        /// 因此递归查询，考虑到有缓存，这个代价也是可以接受的
        /// </summary>
        /// <param name="lpctstrPath"></param>
        /// <returns></returns>
        public int GetProjectIdByNamePath(string lpctstrPath)
        {
            lpctstrPath = lpctstrPath.TrimStart('\\');
            lpctstrPath = lpctstrPath.TrimEnd('\\');
            if (string.IsNullOrWhiteSpace(lpctstrPath))
            {
                return -1;
            }
            var lastPart = lpctstrPath.Substring(lpctstrPath.LastIndexOf("\\") + 1);
            if (lastPart == "desktop.ini")
            {
                // 特殊处理，上游会调用来判断有没有这个特殊文件，即使有，也应该是文件不是目录
                return -1;
            }
            // 先看看能不能直接找到
            var projectId = this.GetProjectIdByNamePathFailForToolong(lpctstrPath);
            if (projectId > -1)
            {
                return projectId;
            }
            lock (this._lock)
            {
                // 递归找
                var full_path_cache_key = $"GetProjectIdByNamePath:{lpctstrPath}";
                // 因为GetProjectIdByNamePathFailForToolong先获取了，已经会有不成功的缓存
                this.m_cache.Delete(full_path_cache_key);

                var subParts = lpctstrPath.Split('\\');
                var subPathNames = new List<string>(); // 逐级拼接出来
                for(int i=0; i < subParts.Length; i++)
                {
                    subPathNames.Add(string.Join("\\", subParts.Take(i + 1)));
                }
                var lastProjectId = 0;
                foreach(var subPathName in subPathNames)
                {
                    var label = subPathName.Substring(subPathName.LastIndexOf("\\") + 1);
                    var cache_key = $"GetProjectIdByNamePath:{subPathName}";
                    Func<int> get_value_func = () =>
                    {
                        var proj = this._GetProjectByNameAndProjectId(label, lastProjectId);
                        if (proj != null)
                        {
                            return proj.id;
                        }
                        else
                        {                            
                            return -1;
                        }
                    };
                    lastProjectId = this.m_cache.TryGet(cache_key, get_value_func);
                    if (lastProjectId == -1)
                    {
                        // 因为是从上到下找的，如果某层级找不到，说明确实没有
                        return -1;
                    }
                }
                return lastProjectId;                
            }            
            
        }

        /// <summary>
        /// 从路径反推文件夹id，如果不存在，返回-1
        /// 对于超长的会返回不正确的结果,也会返回-1
        /// </summary>
        /// <param name="lpctstrPath"></param>
        /// <returns></returns>
        private int GetProjectIdByNamePathFailForToolong(string lpctstrPath)
        {
            // use lock to ensure thread safe calling pw apis
            lock (this._lock)
            {
                var cache_key = $"GetProjectIdByNamePath:{lpctstrPath}";
                Func<int> get_value_func = () =>
                {
                    try
                    {
                        return this._GetProjectIdByNamePath(lpctstrPath);
                    }
                    catch(PWException e) 
                    { 
                        if(e.PWErrorId== 50000)
                        {
                            // 用-1来表示目录不存在情况
                            // 和原来aaApi_GetProjectIdByNamePath的返回值有所区分
                            return -1;
                        }
                        else
                        {
                            throw e;
                        }
                    }
                    
                };
                return this.m_cache.TryGet(cache_key, get_value_func);
            }
        }

        public PWProject GetProjectByNamePath(string lpctstrPath)
        {
            // use lock to ensure thread safe calling pw apis
            lock (this._lock)
            {                
                var projectno = this.GetProjectIdByNamePath(lpctstrPath);
                if (projectno <= 0)
                {
                    return null;
                }
                else
                {
                    var proj = this._Read(projectno);
                    return proj;
                }
            }
        }

        public bool IsNamePathExists(string lpctstrPath)
        {
            var projectId = this.GetProjectIdByNamePath(lpctstrPath);
            // 0 表示目录不存在？
            return projectId != -1;
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
                var cache_key = $"GetNamePathByProjectId:{projectId}";
                Func<string> get_value_func = () =>
                {
                    return this._GetNamePathByProjectId(projectId);
                };
                return this.m_cache.TryGet(cache_key, get_value_func);
            }
        }


        /// <summary>
        /// 更新项目信息
        /// </summary>
        /// <param name="proj"></param>
        /// <returns></returns>
        private void _Update(int projectId, string new_name)
        {            
            var projItem = new dmscli.AADMSPROJITEM();
            projItem.lptstrName = new_name;
            projItem.lptstrDesc = new_name;
            projItem.lProjectId = projectId;
            // 指定要更新的数据
            projItem.ulFlags = projItem.ulFlags = (uint)(
                dmscli.AADMSProjFlags.Name |
                dmscli.AADMSProjFlags.Desc
             );

            IntPtr lpProject = Util.StructureToPtr(projItem);
            var ret = dmscli.aaApi_ModifyProject2(lpProject);

            if (!ret)
            {
                Util.FreeHGlobal(ref lpProject);
                throw PWException.GetPWLastException();
            }
            projItem = Util.PtrToStructure<dmscli.AADMSPROJITEM>(lpProject);
            return;
        }


        /// <summary>
        /// 设置项目的父级。注意aaApi_ModifyProject2方法并不能修改ParentId
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="parentProjectId"></param>
        private void _SetProjectParent(int projectId, int parentProjectId)
        {
            // Current user must have rights to create and delete projects. See aaApi_GetUserNumericSetting() function for more information.
            var ret = dmscli.aaApi_SetParentProject(projectId, parentProjectId);
            if (!ret)
            {
                throw PWException.GetPWLastException();
            }
        }



        public void MoveDirectory(string oldpath, string newpath)
        {
            var oldprojectid = this.GetProjectIdByNamePath(oldpath);
            lock (this._lock)
            {                
                var proj = this._Read(oldprojectid);
                var parentPath = newpath.Substring(0, newpath.LastIndexOf("\\"));
                var new_parentid = this._GetProjectIdByNamePath(parentPath);
                var new_basename = newpath.Substring(newpath.LastIndexOf("\\") + 1);
                if (new_basename != proj.name)
                {
                    // 重命名的情况
                    this._Update(oldprojectid, new_basename);
                }
                if (proj.parentid != new_parentid)
                {
                    this._SetProjectParent(proj.id, new_parentid);
                }
                
                // 清空缓存，防止移动后老的还在
                var cache_key = $"GetNamePathByProjectId:{oldprojectid}";
                this.m_cache.Delete(cache_key);
                var cache_key_pattern = "GetProjectIdByNamePath:*";
                this.m_cache.DeleteByValue(oldpath, cache_key_pattern);
                // 防止前面获取过newpath，返回空的情况
                this.m_cache.DeleteByValue(newpath, cache_key_pattern);
            }
        }
    }
}
