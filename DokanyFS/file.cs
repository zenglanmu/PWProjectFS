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
        public void Cleanup(string filename, IDokanFileInfo info)
        {

        }

        public void CloseFile(string filename, IDokanFileInfo info)
        {
        }

        public NtStatus CreateFile(
            string fileName,
            FileAccess access,
            FileShare share,
            FileMode mode,
            FileOptions options,
            FileAttributes attributes,
            IDokanFileInfo info)
        {
            this.provider.Activate();
            var filePath = GetPath(fileName);
            if (info.IsDirectory)
            {
                if (info.IsDirectory)
                {
                    if (mode == FileMode.Open)
                        return this.OpenDirectory(filePath, info);
                    if (mode == FileMode.CreateNew)
                        return this.CreateDirectory(fileName, info);

                    return DokanResult.NotImplemented;
                }
            }
                
            return DokanResult.Success;
        }


        public NtStatus DeleteFile(string filename, IDokanFileInfo info)
        {
            return DokanResult.NotImplemented;
        }


        public NtStatus FlushFileBuffers(
            string filename,
            IDokanFileInfo info)
        {
            return DokanResult.NotImplemented;
        }

        public NtStatus FindFiles(
            string filename,
            out IList<FileInformation> files,
            IDokanFileInfo info)
        {
            // This function is not called because FindFilesWithPattern is implemented
            // Return DokanResult.NotImplemented in FindFilesWithPattern to make FindFiles called
            files = new List<FileInformation>();
            return DokanResult.Success;
        }

        public NtStatus GetFileInformation(
            string filename,
            out FileInformation fileinfo,
            IDokanFileInfo info)
        {            
            // 给个默认值
            fileinfo = new FileInformation { FileName = filename };
            fileinfo.Attributes = FileAttributes.Directory;
            fileinfo.LastAccessTime = DateTime.Now;
            fileinfo.LastWriteTime = null;
            fileinfo.CreationTime = null;

            if (filename == "\\" || filename.EndsWith("\\uwstconfig"))
            {
                return DokanResult.Success;
            }
            this.provider.Activate();
            var fPath = this.GetPath(filename);
            var pw_doc = this.provider.DocumentHelper.GetDocumentByNamePath(fPath);
            if (pw_doc != null)
            {
                fileinfo = pw_doc.toFileInformation();
                return DokanResult.Success;
            }
            else
            {
                var pw_proj = this.provider.ProjectHelper.GetProjectByNamePath(fPath);
                if (pw_proj == null)
                {
                    return DokanResult.FileNotFound;
                }
                else
                {
                    fileinfo = pw_proj.toFileInformation();
                    return DokanResult.Success;
                }
                
            }

        }

        public NtStatus LockFile(
            string filename,
            long offset,
            long length,
            IDokanFileInfo info)
        {
            return DokanResult.Success;
        }

        public NtStatus MoveFile(
            string filename,
            string newname,
            bool replace,
            IDokanFileInfo info)
        {
            this.provider.Activate();
            var oldpath = this.GetPath(filename);
            var oldprojectid = this.provider.ProjectHelper.GetProjectIdByNamePath(oldpath);
            if (oldprojectid > 0 || info.IsDirectory)
            {
                // 是移动目录的清空，info.IsDirectory传递的参数不准
                return this.MoveDirectory(filename, newname, replace, info);
            }
            else
            {
                var old_pwdoc = this.provider.DocumentHelper.GetDocumentByNamePath(oldpath);
                if (old_pwdoc == null)
                {
                    return DokanResult.FileNotFound;
                }
                else
                {
                    // TODO, rename document
                    return DokanResult.NotImplemented;
                }
            }          
        }

        public NtStatus ReadFile(
            string filename,
            byte[] buffer,
            out int readBytes,
            long offset,
            IDokanFileInfo info)
        {
            readBytes = 0;
            return DokanResult.NotImplemented;
        }

        public NtStatus SetEndOfFile(string filename, long length, IDokanFileInfo info)
        {
            return DokanResult.NotImplemented;
        }

        public NtStatus SetAllocationSize(string filename, long length, IDokanFileInfo info)
        {
            return DokanResult.NotImplemented;
        }

        public NtStatus SetFileAttributes(
            string filename,
            FileAttributes attr,
            IDokanFileInfo info)
        {
            return DokanResult.NotImplemented;
        }

        public NtStatus SetFileTime(
            string filename,
            DateTime? ctime,
            DateTime? atime,
            DateTime? mtime,
            IDokanFileInfo info)
        {
            return DokanResult.NotImplemented;
        }

        public NtStatus UnlockFile(string filename, long offset, long length, IDokanFileInfo info)
        {
            return DokanResult.Success;
        }

        public NtStatus Mounted(string mountPoint, IDokanFileInfo info)
        {
            return DokanResult.Success;
        }

        

        public NtStatus WriteFile(
            string filename,
            byte[] buffer,
            out int writtenBytes,
            long offset,
            IDokanFileInfo info)
        {
            writtenBytes = 0;
            return DokanResult.NotImplemented;
        }

        

        public NtStatus GetFileSecurity(string fileName, out FileSystemSecurity security, AccessControlSections sections,
            IDokanFileInfo info)
        {
            security = null;
            return DokanResult.NotImplemented;
        }

        public NtStatus SetFileSecurity(string fileName, FileSystemSecurity security, AccessControlSections sections,
            IDokanFileInfo info)
        {
            return DokanResult.NotImplemented;
        }

        public NtStatus EnumerateNamedStreams(string fileName, IntPtr enumContext, out string streamName,
            out long streamSize, IDokanFileInfo info)
        {
            streamName = string.Empty;
            streamSize = 0;
            return DokanResult.NotImplemented;
        }

        public NtStatus FindStreams(string fileName, out IList<FileInformation> streams, IDokanFileInfo info)
        {
            streams = new FileInformation[0];
            return DokanResult.NotImplemented;
        }


        /// <summary>
        /// 实际上对应列文件目录的功能
        /// 如果这个定义了，则FindFiles没用
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="searchPattern"></param>
        /// <param name="files"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public NtStatus FindFilesWithPattern(string fileName, string searchPattern, out IList<FileInformation> files,
            IDokanFileInfo info)
        {
            files = new List<FileInformation>();
            // 一些特殊情况的处理
            if(fileName=="\\")
            {
                if(searchPattern == "__drive_fs_keepalive" || searchPattern== "uwstconfig" || searchPattern== "drive_fs_notification")
                {
                    return DokanResult.Success;
                }                
            }
            if (searchPattern == "*")
            {
                // 列目录下所有内容的操作
                var filePath = this.GetPath(fileName);
                var projectId = this.provider.ProjectHelper.GetProjectIdByNamePath(filePath);
                var projects = this.provider.ProjectHelper.ReadByParent(projectId);
                foreach(var proj in projects)
                {
                    var file = proj.toFileInformation();
                    files.Add(file);
                }
                var docs = this.provider.DocumentHelper.ReadByParent(projectId);
                foreach(var doc in docs)
                {
                    var file = doc.toFileInformation();
                    files.Add(file);
                }
            }
            else
            {
                // TODO, 其他情况下的查询
                // 比如会filename和searchPattern拼接起来是一个,一般是目录的情况为多
                var fPath = this.GetPath(fileName + searchPattern);
                var pw_proj = this.provider.ProjectHelper.GetProjectByNamePath(fPath);
                if (pw_proj != null)
                {
                    files.Add(pw_proj.toFileInformation());
                    return DokanResult.Success;
                }
                else
                {
                    var pw_doc = this.provider.DocumentHelper.GetDocumentByNamePath(fPath);
                    if (pw_doc != null)
                    {
                        files.Add(pw_doc.toFileInformation());
                        return DokanResult.Success;
                    }
                }
            }
            return DokanResult.Success;
        }
    }
}
