using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using DokanNet;
using Microsoft.Win32;
using FileAccess = DokanNet.FileAccess;

namespace PWProjectFS.DokanyFS
{
    internal partial class PWFSOperations
    {
        public NtStatus Unmounted(IDokanFileInfo info)
        {
            this.provider.InvalidateCache();
            this.provider.Uninitialize();
            return DokanResult.Success;
        }

        public NtStatus GetDiskFreeSpace(
            out long freeBytesAvailable,
            out long totalBytes,
            out long totalFreeBytes,
            IDokanFileInfo info)
        {
            var _lock = this.provider.Activate();
            var storageInfo = this.provider.ProjectHelper.GetStorageInfo(this.base_pw_projectno);
            freeBytesAvailable = (long)storageInfo.FreeSpaceAvailable;
            totalBytes = (long)storageInfo.TotalSpace;
            totalFreeBytes = (long)storageInfo.FreeSpace;

            //freeBytesAvailable = 512 * 512 * 100;
            //totalBytes = 512 * 512 * 200;
            //totalFreeBytes = 512 * 512 * 100;
            return DokanResult.Success;
        }

        public NtStatus GetVolumeInformation(out string volumeLabel, out FileSystemFeatures features,
            out string fileSystemName, out uint maximumComponentLength, IDokanFileInfo info)
        {
            volumeLabel = "ProjectWise";
            fileSystemName = string.Empty;
            maximumComponentLength = 128;
            features = FileSystemFeatures.CasePreservedNames |
                       FileSystemFeatures.PersistentAcls |FileSystemFeatures.UnicodeOnDisk;
            return DokanResult.Success;
        }

        private NtStatus OpenDirectory(string filePath, IDokanFileInfo info)
        {
            if (filePath == null)
            {
                return NtStatus.Success;
            }
            if (this.provider.ProjectHelper.IsNamePathExists(filePath))
            {
                return NtStatus.Success;
            }
            else
            {
                // 必须返回NotADirectory,要不然新建文件夹的时候会先调用这个判断状态
                return DokanResult.NotADirectory;
            }
            
        }

        private NtStatus CreateDirectory(string fileName, IDokanFileInfo info)
        {
            this.provider.Activate();
            if (fileName == null)
            {
                return NtStatus.Success;
            }
            var filePath = GetPath(fileName);
            if (this.provider.ProjectHelper.IsNamePathExists(filePath))
            {
                return DokanResult.AlreadyExists;
            }
            var projectId = this.provider.ProjectHelper.CreateByFullPath(filePath);
            if (projectId == -1)
            {
                // 父目录不存在
                return DokanResult.PathNotFound;
            }
            else
            {
                return DokanResult.Success;
            }            
        }

        public NtStatus DeleteDirectory(string fileName, IDokanFileInfo info)
        {
            this.provider.Activate();
            var filePath = this.GetPath(fileName);
            var projectId = this.provider.ProjectHelper.GetProjectIdByNamePath(filePath);
            if (projectId == -1)
            {
                return DokanResult.PathNotFound;
            }
            var projects = this.provider.ProjectHelper.ReadByParent(projectId);
            var docs = this.provider.DocumentHelper.ReadByParent(projectId);
            if(projects.Count>0 || docs.Count > 0)
            {
                return DokanResult.DirectoryNotEmpty;
            }
            // if dir is not empty it can't be deleted
            this.provider.ProjectHelper.Delete(projectId);
            return DokanResult.Success;
        }

        private NtStatus MoveDirectory(string filename,
            string newname,
            bool replace,
            IDokanFileInfo info)
        {            
            var oldpath = this.GetPath(filename);
            var newpath = this.GetPath(newname);

            if (this.provider.ProjectHelper.IsNamePathExists(newpath) && !replace)
            {
                return DokanResult.AlreadyExists;
            }
            var oldprojectid = this.provider.ProjectHelper.GetProjectIdByNamePath(oldpath);
            if (oldprojectid == -1)
            {
                return DokanResult.PathNotFound;
            }
            this.provider.ProjectHelper.MoveDirectory(oldpath, newpath);
            return DokanResult.Success;
        }
    }
}
