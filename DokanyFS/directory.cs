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
        public NtStatus Mounted(string mountPoint, IDokanFileInfo info)
        {
            return Trace(nameof(Mounted), null, info, DokanResult.Success);
        }

        public NtStatus Unmounted(IDokanFileInfo info)
        {
            this.provider.InvalidateCache();
            this.provider.Uninitialize();
            return Trace(nameof(Unmounted), null, info, DokanResult.Success);
        }

        public NtStatus GetDiskFreeSpace(
            out long freeBytesAvailable,
            out long totalBytes,
            out long totalFreeBytes,
            IDokanFileInfo info)
        {
            var storageInfo = this.provider.ProjectHelper.GetStorageInfo(this.base_pw_projectno);
            freeBytesAvailable = (long)storageInfo.FreeSpaceAvailable;
            totalBytes = (long)storageInfo.TotalSpace;
            totalFreeBytes = (long)storageInfo.FreeSpace;
            return Trace(nameof(GetDiskFreeSpace), null, info, DokanResult.Success, "out " + freeBytesAvailable.ToString(),
                "out " + totalBytes.ToString(), "out " + totalFreeBytes.ToString());
        }

        public NtStatus GetVolumeInformation(out string volumeLabel, out FileSystemFeatures features,
            out string fileSystemName, out uint maximumComponentLength, IDokanFileInfo info)
        {
            volumeLabel = "ProjectWise";
            fileSystemName = string.Empty;
            maximumComponentLength = 256;
            features = FileSystemFeatures.CasePreservedNames |
                       FileSystemFeatures.PersistentAcls |FileSystemFeatures.UnicodeOnDisk;
            return Trace(nameof(GetVolumeInformation), null, info, DokanResult.Success, "out " + volumeLabel,
                "out " + features.ToString(), "out " + fileSystemName);
        }       
       

        public NtStatus DeleteDirectory(string fileName, IDokanFileInfo info)
        {
            var filePath = this.GetPath(fileName);
            var projectId = this.provider.ProjectHelper.GetProjectIdByNamePath(filePath);
            if (projectId == -1)
            {
                return DokanResult.PathNotFound;
            }
            var projects = this.provider.ProjectHelper.ReadByParent(projectId);
            var docs = this.provider.DocumentHelper.ReadByParent(projectId);
            var notEmpty = false;
            if(projects.Count>0 || docs.Count > 0)
            {
                notEmpty = true;
            }
            // if dir is not empty it can't be deleted
            // just check here, if we could delete the directory - the true deletion is in Cleanup            
            return Trace(nameof(DeleteDirectory), fileName, info,
                notEmpty ? DokanResult.DirectoryNotEmpty: DokanResult.Success);
        }
        
    }
}
