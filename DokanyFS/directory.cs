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
            features = FileSystemFeatures.CasePreservedNames | FileSystemFeatures.CaseSensitiveSearch |
                       FileSystemFeatures.PersistentAcls | FileSystemFeatures.SupportsRemoteStorage |
                       FileSystemFeatures.UnicodeOnDisk;
            return DokanResult.Success;
        }

        private NtStatus OpenDirectory(string fileName, IDokanFileInfo info)
        {
            return NtStatus.Success;
        }

        private NtStatus CreateDirectory(string fileName, IDokanFileInfo info)
        {
            throw new UnauthorizedAccessException();
        }
    }
}
