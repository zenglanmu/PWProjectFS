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
            if (projectId == 0)
            {
                return null;
            }
            // use lock to ensure thread safe calling pw apis
            lock (this._lock)
            {
                var proj = this._Read(projectId);
                var cache_key = $"ReadProjectsByParentProjectId:{proj.parentid}";
                Func<List<PWProject>> get_value_func = () =>
                {
                    return this._ReadByParent(proj.parentid);

                };
                // update cache
                var projs = this.m_cache.TryGet(cache_key, get_value_func);
                var exist = projs.Where(x => x.id == proj.id).FirstOrDefault();
                if (exist != null)
                {
                    exist = proj; // update exist
                }
                else
                {
                    projs.Add(proj);
                }
                return proj;
                
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
                var cache_key = $"ReadProjectsByParentProjectId:{parentProjectId}";
                Func<List<PWProject>> get_value_func = () =>
                {
                    return this._ReadByParent(parentProjectId);

                };
                return this.m_cache.TryGet(cache_key, get_value_func);
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

        public PWProject Create(int parentProjectId, string name, string description)
        {
            // use lock to ensure thread safe calling pw apis
            lock (this._lock)
            {
                var projectno = this._Create(parentProjectId, name, description);
                return this.Read(projectno);
            }
        }

        public PWProject CreateByFullPath(string projFullPath)
        {
            if (projFullPath.Length == 0)
            {
                throw new IOException("create new dir on root");
            }
            var parentProjectId = 0;
            if (projFullPath.Contains("\\"))
            {
                var parentPath = projFullPath.Substring(0, projFullPath.LastIndexOf("\\"));
                parentProjectId = this.GetProjectIdByNamePath(parentPath);
                if (parentProjectId == -1)
                {
                    // 父目录不存在，好让上层调用方知道
                    return null;
                }
            }

            // use lock to ensure thread safe calling pw apis
            lock (this._lock)
            {                
                // 拼接上pw的父路径，获取完整的pw父路径
                var basename = projFullPath.Substring(projFullPath.LastIndexOf("\\") + 1);
                var proj = this.Create(parentProjectId, basename, basename);

                return proj;
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

        public void Delete(PWProject proj)
        {
            // use lock to ensure thread safe calling pw apis
            lock (this._lock)
            {
                this._Delete(proj.id);
                // update cache
                var projs = this.ReadByParent(proj.parentid);
                var exist = projs.Where(x => x.id == proj.id).FirstOrDefault();
                if (exist != null)
                {
                    projs.Remove(exist);
                }
                else
                {
                    // nothing
                }
            }
        }       

        /// <summary>
        /// 根据label名称取值
        /// </summary>
        /// <param name="label"></param>
        /// <param name="parentProjectId"></param>
        /// <returns></returns>
        private PWProject GetProjectByNameAndProjectId(string label, int parentProjectId)
        {
            // aaApi_SelectProjectsByStruct 这个方法一直报缓冲区大小不够的错误，所以只好都出子目录再匹配
            var projects = this.ReadByParent(parentProjectId);
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
                return 0;
            }
            var lastPart = lpctstrPath.Substring(lpctstrPath.LastIndexOf("\\") + 1);
            if (lastPart == "desktop.ini")
            {
                // 特殊处理，上游会调用来判断有没有这个特殊文件，即使有，也应该是文件不是目录
                return -1;
            }

            lock (this._lock)
            {
                // 递归找
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
                    
                    var proj = this.GetProjectByNameAndProjectId(label, lastProjectId);
                    if (proj != null)
                    {
                        lastProjectId = proj.id;
                    }
                    else
                    {
                        lastProjectId = -1;
                    }
                    if (lastProjectId == -1)
                    {
                        // 因为是从上到下找的，如果某层级找不到，说明确实没有
                        return -1;
                    }
                }
                return lastProjectId;                
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
                    var proj = this.Read(projectno);
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

        // 根据id反推路径，应该就给获取基路径用
        public string GetNamePathByProjectId(int projectId)
        {
            if (projectId == 0)
            {
                return "";
            }
            // 逐级往上递归获取
            var projs = new List<PWProject>();
            var cur_projectid = projectId;
            var proj = this.Read(cur_projectid);
            while (proj != null)
            {
                projs.Add(proj);
                if (proj.parentid == 0)
                {
                    break;
                }
                proj = this.Read(proj.parentid);                
            }
            projs.Reverse();
            var names = projs.Select(x => x.label);
            var fullPath = string.Join("\\", names);
            return fullPath;            
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
                var new_parentid = this.GetProjectIdByNamePath(parentPath);
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
                var cache_key = $"ReadProjectsByParentProjectId:{proj.parentid}";
                this.m_cache.Delete(cache_key);
                // 防止前面获取过newpath，返回空的情况
                cache_key = $"ReadProjectsByParentProjectId:{new_parentid}";
                this.m_cache.Delete(cache_key);
            }
        }
    }
}
