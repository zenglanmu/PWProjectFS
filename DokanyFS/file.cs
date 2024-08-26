using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Security.AccessControl;
using DokanNet;
using Microsoft.Win32;
using static DokanNet.FormatProviders;
using FileAccess = DokanNet.FileAccess;
using PWProjectFS.PWApiWrapper;

namespace PWProjectFS.DokanyFS
{
    internal partial class PWFSOperations
    {
        public void Cleanup(string fileName, IDokanFileInfo info)
        {
#if TRACE
            if (info.Context != null)
                Console.WriteLine(DokanFormat($"{nameof(Cleanup)}('{fileName}', {info} - entering"));
#endif

            (info.Context as PWFileContext)?.Dispose();
            info.Context = null;

            var filePath = this.GetPath(fileName);
            if (info.DeleteOnClose)
            {
                if (info.IsDirectory)
                {
                    var projectId = this.provider.ProjectHelper.GetProjectIdByNamePath(filePath);
                    if (projectId > 0)
                    {
                        this.provider.ProjectHelper.Delete(projectId);
                    }
                    
                }
                else
                {
                    var pw_doc = this.provider.DocumentHelper.GetDocumentByNamePath(filePath);
                    if (pw_doc!= null){
                        this.provider.DocumentHelper.Delete(pw_doc.id);
                    }                    
                }
            }
            Trace(nameof(Cleanup), fileName, info, DokanResult.Success);
        }

        public void CloseFile(string fileName, IDokanFileInfo info)
        {
#if TRACE
            if (info.Context != null)
                Console.WriteLine(DokanFormat($"{nameof(CloseFile)}('{fileName}', {info} - entering"));
#endif
            (info.Context as PWFileContext)?.Dispose();
            info.Context = null;
            Trace(nameof(CloseFile), fileName, info, DokanResult.Success);
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
            // only need to activate in CreateFile
            this.provider.Activate();
            var result = DokanResult.Success;
            var filePath = GetPath(fileName);            

            if (info.IsDirectory)
            {
                try
                {
                    // 不获取对象改获取id是因为挂载顶层目录情况时没有对象，proejctId为0
                    var projectId = this.provider.ProjectHelper.GetProjectIdByNamePath(filePath);                    
                    var pw_doc = this.provider.DocumentHelper.GetDocumentByNamePath(filePath);
                    var pathExists = projectId >=0 || pw_doc != null;
                    switch (mode)
                    {
                        case FileMode.Open:                            
                            if (!pathExists)
                            {
                                if (projectId <0)
                                {
                                    return Trace(nameof(CreateFile), fileName, info, access, share, mode, options,
                                        attributes, DokanResult.NotADirectory);
                                }

                                return Trace(nameof(CreateFile), fileName, info, access, share, mode, options,
                                    attributes, DokanResult.PathNotFound);
                            }

                            break;

                        case FileMode.CreateNew:
                            if (projectId >=0)
                                return Trace(nameof(CreateFile), fileName, info, access, share, mode, options,
                                    attributes, DokanResult.FileExists);

                            this.provider.ProjectHelper.CreateByFullPath(filePath);
                            break;
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    return Trace(nameof(CreateFile), fileName, info, access, share, mode, options, attributes,
                        DokanResult.AccessDenied);
                }
            }
            else
            {
                var pathExists = true;
                var pathIsDirectory = false;
                // TODO, 权限检查。对应路径下是否有读取或者写入的权限
                var readWriteAttributes = (access & DataAccess) == 0;
                var readAccess = (access & DataWriteAccess) == 0;

                if (fileName == "\\uwstconfig")
                {
                    // 一些特殊名称的处理，虽然很ugly
                    // 否则没必要去后端查，反而浪费时间
                    pathExists = false;
                    pathIsDirectory = false;
                }
                else
                {
                    // 和info.IsDirectory分开，是避免像"\\uwstconfig"跑到pw上查询
                    var projectId = this.provider.ProjectHelper.GetProjectIdByNamePath(filePath);
                    var pw_doc = this.provider.DocumentHelper.GetDocumentByNamePath(filePath);
                    pathExists = projectId >= 0 || pw_doc != null;
                    pathIsDirectory = pathExists ? projectId >= 0 : false;
                }

                switch (mode)
                {
                    case FileMode.Open:

                        if (pathExists)
                        {
                            // check if driver only wants to read attributes, security info, or open directory
                            if (readWriteAttributes || pathIsDirectory)
                            {
                                if (pathIsDirectory && (access & FileAccess.Delete) == FileAccess.Delete
                                    && (access & FileAccess.Synchronize) != FileAccess.Synchronize)
                                {
                                    //It is a DeleteFile request on a directory
                                    return Trace(nameof(CreateFile), fileName, info, access, share, mode, options,
                                        attributes, DokanResult.AccessDenied);
                                }

                                info.IsDirectory = pathIsDirectory;
                                info.Context = new object();
                                // must set it to something if you return DokanError.Success

                                // must set it to something if you return DokanError.Success

                                return Trace(nameof(CreateFile), fileName, info, access, share, mode, options,
                                    attributes, DokanResult.Success);
                            }
                        }
                        else
                        {
                            return Trace(nameof(CreateFile), fileName, info, access, share, mode, options, attributes,
                                DokanResult.FileNotFound);
                        }
                        break;

                    case FileMode.CreateNew:
                        if (pathExists)
                        {
                            return Trace(nameof(CreateFile), fileName, info, access, share, mode, options, attributes,
                                DokanResult.FileExists);
                        }
                        break;

                    case FileMode.Truncate:
                        if (!pathExists)
                            return Trace(nameof(CreateFile), fileName, info, access, share, mode, options, attributes,
                                DokanResult.FileNotFound);
                        break;
                }
      
                try
                {
                    bool fileCreated = mode == FileMode.CreateNew || mode == FileMode.Create || (!pathExists && mode == FileMode.OpenOrCreate);
                    if (fileCreated)
                    {
                        FileAttributes new_attributes = attributes;
                        new_attributes |= FileAttributes.Archive; // Files are always created as Archive
                        var pw_doc = this.provider.DocumentHelper.Touch(filePath);
                    }
                    System.IO.FileAccess streamAccess = readAccess ? System.IO.FileAccess.Read : System.IO.FileAccess.ReadWrite;
                    if (mode == System.IO.FileMode.CreateNew && readAccess) streamAccess = System.IO.FileAccess.ReadWrite;
                    // 针对打开？是否应该传检出后的文件路径
                    info.Context = PWFileContext.CreateFileContext(this.provider, filePath, info.PagingIo, mode,
                        readAccess ? System.IO.FileAccess.Read : System.IO.FileAccess.ReadWrite, share, options);

                    if (pathExists && (mode == FileMode.OpenOrCreate
                                       || mode == FileMode.Create))
                    {
                        result = DokanResult.AlreadyExists;
                    }
                }
                catch (UnauthorizedAccessException) // don't have access rights
                {
                    if (info.Context is IDisposable mx)
                    {
                        // returning AccessDenied cleanup and close won't be called,
                        // so we have to take care of the stream now
                        mx.Dispose();
                        info.Context = null;
                    }
                    return Trace(nameof(CreateFile), fileName, info, access, share, mode, options, attributes,
                        DokanResult.AccessDenied);
                }
                catch (DirectoryNotFoundException)
                {
                    return Trace(nameof(CreateFile), fileName, info, access, share, mode, options, attributes,
                        DokanResult.PathNotFound);
                }
                catch (Exception)
                {
                    return Trace(nameof(CreateFile), fileName, info, access, share, mode, options, attributes,
                                DokanResult.InternalError);
                }
            }
            return Trace(nameof(CreateFile), fileName, info, access, share, mode, options, attributes,
                result);
        }


        public NtStatus DeleteFile(string fileName, IDokanFileInfo info)
        {
            var filePath = GetPath(fileName);
            if (this.provider.ProjectHelper.IsNamePathExists(filePath))
            {
                // 假如是目录，但是调用了删除文件的方法
                return Trace(nameof(DeleteFile), fileName, info, DokanResult.AccessDenied);
            }
            var pw_doc = this.provider.DocumentHelper.GetDocumentByNamePath(filePath);
            if (pw_doc == null)
            {
                return Trace(nameof(DeleteFile), fileName, info, DokanResult.FileNotFound);
            }
            return Trace(nameof(DeleteFile), fileName, info, DokanResult.Success);
            // we just check here if we could delete the file - the true deletion is in Cleanup
        }


        public NtStatus FlushFileBuffers(
            string fileName,
            IDokanFileInfo info)
        {
            try
            {
                ((PWFileContext)(info.Context)).Flush();
                return Trace(nameof(FlushFileBuffers), fileName, info, DokanResult.Success);
            }
            catch (IOException)
            {
                return Trace(nameof(FlushFileBuffers), fileName, info, DokanResult.DiskFull);
            }
        }

        public NtStatus GetFileInformation(
            string fileName,
            out FileInformation fileinfo,
            IDokanFileInfo info)
        {            
            // 给个默认值
            fileinfo = new FileInformation { FileName = fileName };
            fileinfo.Attributes = FileAttributes.Directory;
            fileinfo.LastAccessTime = DateTime.Now;
            fileinfo.LastWriteTime = null;
            fileinfo.CreationTime = null;

            if (fileName == "\\" || fileName.EndsWith("\\uwstconfig"))
            {
                return DokanResult.Success;
            }
            var fPath = this.GetPath(fileName);
            var pw_doc = this.provider.DocumentHelper.GetDocumentByNamePath(fPath);
            if (pw_doc != null)
            {
                fileinfo = pw_doc.toFileInformation();
                return Trace(nameof(GetFileInformation), fileName, info, DokanResult.Success);
            }
            else
            {
                var pw_proj = this.provider.ProjectHelper.GetProjectByNamePath(fPath);
                if (pw_proj == null)
                {
                    return Trace(nameof(GetFileInformation), fileName, info, info.IsDirectory ? DokanResult.PathNotFound : DokanResult.FileNotFound);
                }
                else
                {
                    fileinfo = pw_proj.toFileInformation();
                    return Trace(nameof(GetFileInformation), fileName, info, DokanResult.Success);
                }
                
            }

        }        

        public NtStatus MoveFile(
            string oldName,
            string newName,
            bool replace,
            IDokanFileInfo info)
        {
            var oldpath = GetPath(oldName);
            var newpath = GetPath(newName);

            (info.Context as PWFileContext)?.Dispose();
            info.Context = null;

            var exist = false;
            if (info.IsDirectory)
            {
                exist = this.provider.ProjectHelper.IsNamePathExists(oldpath);
            }
            else
            {
                var new_pwdoc = this.provider.DocumentHelper.GetDocumentByNamePath(newpath);
                exist = new_pwdoc != null;
            }

            try
            {
                if (!exist)
                {
                    info.Context = null;
                    if (info.IsDirectory)
                    {
                        this.provider.ProjectHelper.MoveDirectory(oldpath, newpath);
                    }
                    else
                    {
                        this.provider.DocumentHelper.MoveFile(oldpath, newpath);
                    }
                    return Trace(nameof(MoveFile), oldName, info, DokanResult.Success, newName,
                        replace.ToString(CultureInfo.InvariantCulture));
                }
                else if (replace)
                {
                    info.Context = null;

                    if (info.IsDirectory) //Cannot replace directory destination - See MOVEFILE_REPLACE_EXISTING
                        return Trace(nameof(MoveFile), oldName, info, DokanResult.AccessDenied, newName,
                            replace.ToString(CultureInfo.InvariantCulture));

                    var old_pwdoc = this.provider.DocumentHelper.GetDocumentByNamePath(oldpath);
                    this.provider.DocumentHelper.Delete(old_pwdoc.id);
                    this.provider.DocumentHelper.MoveFile(oldpath, newpath);

                    return Trace(nameof(MoveFile), oldName, info, DokanResult.Success, newName,
                        replace.ToString(CultureInfo.InvariantCulture));
                }
            }
            catch (UnauthorizedAccessException)
            {
                return Trace(nameof(MoveFile), oldName, info, DokanResult.AccessDenied, newName,
                    replace.ToString(CultureInfo.InvariantCulture));
            }
            return Trace(nameof(MoveFile), oldName, info, DokanResult.FileExists, newName,
                replace.ToString(CultureInfo.InvariantCulture));
        }

        public NtStatus ReadFile(string fileName, byte[] buffer, out int bytesRead, long offset, IDokanFileInfo info)
        {
            try
            {
                if (info.Context == null) // memory mapped read
                {
                    using (var mx = PWFileContext.CreateFileContext(this.provider, GetPath(fileName), info.PagingIo, FileMode.Open, System.IO.FileAccess.Read))
                    {
                        bytesRead = mx.Read(buffer, offset);
                    }
                }
                else // normal read
                {
                    var mx = info.Context as PWFileContext;
                    lock (mx) //Protect from overlapped read
                    {
                        bytesRead = mx.Read(buffer, offset);
                    }
                }
            }
            catch(IOException e)
            {
                bytesRead = 0;
                if (e.Message== "PW文件锁定")
                {                    
                    return Trace(nameof(ReadFile), fileName, info, DokanResult.SharingViolation, "0",
                    offset.ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    return Trace(nameof(ReadFile), fileName, info, DokanResult.InvalidParameter, "0",
                    offset.ToString(CultureInfo.InvariantCulture));
                }
            }
            catch (Exception e)
            {
                bytesRead = 0;
                return Trace(nameof(ReadFile), fileName, info, DokanResult.InvalidParameter, "0",
                    offset.ToString(CultureInfo.InvariantCulture));
            }

            return Trace(nameof(ReadFile), fileName, info, DokanResult.Success, "out " + bytesRead.ToString(),
                offset.ToString(CultureInfo.InvariantCulture));
        }

        public NtStatus SetEndOfFile(string fileName, long length, IDokanFileInfo info)
        {
            try
            {
                ((PWFileContext)(info.Context)).SetLength(length);
                return Trace(nameof(SetEndOfFile), fileName, info, DokanResult.Success,
                    length.ToString(CultureInfo.InvariantCulture));
            }
            catch (IOException)
            {
                return Trace(nameof(SetEndOfFile), fileName, info, DokanResult.DiskFull,
                    length.ToString(CultureInfo.InvariantCulture));
            }            
        }

        public NtStatus SetAllocationSize(string fileName, long length, IDokanFileInfo info)
        {
            try
            {
                ((PWFileContext)(info.Context)).SetLength(length);
                return Trace(nameof(SetAllocationSize), fileName, info, DokanResult.Success,
                    length.ToString(CultureInfo.InvariantCulture));
            }
            catch (IOException)
            {
                return Trace(nameof(SetAllocationSize), fileName, info, DokanResult.DiskFull,
                    length.ToString(CultureInfo.InvariantCulture));
            }
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

        public NtStatus LockFile(
            string fileName,
            long offset,
            long length,
            IDokanFileInfo info)
        {
            try
            {
                ((PWFileContext)(info.Context)).Lock(offset, length);
                return Trace(nameof(LockFile), fileName, info, DokanResult.Success,
                    offset.ToString(CultureInfo.InvariantCulture), length.ToString(CultureInfo.InvariantCulture));
            }
            catch (IOException)
            {
                return Trace(nameof(LockFile), fileName, info, DokanResult.AccessDenied,
                    offset.ToString(CultureInfo.InvariantCulture), length.ToString(CultureInfo.InvariantCulture));
            }
        }

        public NtStatus UnlockFile(string fileName, long offset, long length, IDokanFileInfo info)
        {
            try
            {
                ((PWFileContext)(info.Context)).Unlock(offset, length);
                return Trace(nameof(UnlockFile), fileName, info, DokanResult.Success,
                    offset.ToString(CultureInfo.InvariantCulture), length.ToString(CultureInfo.InvariantCulture));
            }
            catch (IOException)
            {
                return Trace(nameof(UnlockFile), fileName, info, DokanResult.AccessDenied,
                    offset.ToString(CultureInfo.InvariantCulture), length.ToString(CultureInfo.InvariantCulture));
            }
        }

        public NtStatus WriteFile(
            string fileName,
            byte[] buffer,
            out int bytesWritten,
            long offset,
            IDokanFileInfo info)
        {
            var append = offset == -1;
            if (info.Context == null)
            {
                using (var mx = PWFileContext.CreateFileContext(this.provider, GetPath(fileName), info.PagingIo, append ? FileMode.Append : FileMode.Open, System.IO.FileAccess.Write))
                {
                    if (!append) // Offset of -1 is an APPEND: https://docs.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-writefile
                    {
                        mx.Write(buffer, offset);
                    }
                    else
                    {
                        mx.Append(buffer);
                    }
                    bytesWritten = buffer.Length;
                }
            }
            else
            {
                var mx = info.Context as PWFileContext;
                lock (mx) //Protect from overlapped write
                {
                    if (append)
                    {
                        mx.Append(buffer);
                    }
                    else
                    {
                        mx.Write(buffer, offset);
                    }
                }
                bytesWritten = buffer.Length;
            }
            return Trace(nameof(WriteFile), fileName, info, DokanResult.Success, "out " + bytesWritten.ToString(),
                offset.ToString(CultureInfo.InvariantCulture));
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


        public NtStatus FindStreams(string fileName, IntPtr enumContext, out string streamName, out long streamSize,
            IDokanFileInfo info)
        {
            streamName = string.Empty;
            streamSize = 0;
            return Trace(nameof(FindStreams), fileName, info, DokanResult.NotImplemented, enumContext.ToString(),
                "out " + streamName, "out " + streamSize.ToString());
        }

        public NtStatus FindStreams(string fileName, out IList<FileInformation> streams, IDokanFileInfo info)
        {
            streams = new FileInformation[0];
            return Trace(nameof(FindStreams), fileName, info, DokanResult.NotImplemented);
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
                    // 排除掉这种特殊类型不含文件的
                    if(doc.documentType!= dmscli.DocumentType.Abstract && doc.documentType != dmscli.DocumentType.Set)
                    {
                        var file = doc.toFileInformation();
                        files.Add(file);
                    }                    
                }
                return DokanResult.Success;
            }
            else
            {
                //return DokanResult.Success;
                // 好像实现这个逻辑会有问题，先提前返回取消掉
                // TODO, 其他情况下的查询
                // 比如会filename和searchPattern拼接起来是一个,一般是目录的情况为多
                var fileName1 = fileName.TrimEnd('\\');
                var fPath = this.GetPath(fileName1 + "\\"+ searchPattern);
                var pw_proj = this.provider.ProjectHelper.GetProjectByNamePath(fPath);
                if (pw_proj != null)
                {
                    files.Add(pw_proj.toFileInformation());                    
                }
                else
                {
                    var pw_doc = this.provider.DocumentHelper.GetDocumentByNamePath(fPath);
                    if (pw_doc != null)
                    {
                        files.Add(pw_doc.toFileInformation());
                    }
                }
                return DokanResult.Success;
            }            
        }
    }
}
