using System;
using System.IO;
using System.Diagnostics;
using DokanNet;
using PWProjectFS.PWProvider;
using PWProjectFS.PWApiWrapper;

namespace PWProjectFS.DokanyFS
{

    /// <summary>
    /// 封装对pw的io操作
    /// </summary>
    public class PWFileContext : IDisposable
    {
        private bool _disposed = false;

        /// <summary>
        /// 是不是每次执行写入相关操作时，对本地副本io完后，更新服务器副本
        /// </summary>
        private bool updateServerCopyOnWrite = true;
        
        private PWDataSourceProvider provider { get; set; }

        /// <summary>
        /// 文件在pw上的完整路径
        /// </summary>
        private string docFullPath { get;  set; }

        /// <summary>
        /// 获取本地副本时，是用检出的方式还是复制出的方式
        /// </summary>
        private bool isDocCheckOut = true;

        /// <summary>
        /// 是否有explorer.exe，即资源管理器打开
        /// </summary>
        private bool OpenByExplorer = false;

        /// <summary>
        /// 对应的info
        /// </summary>
        private IDokanFileInfo info { get; set; }

        private FileMode mode { get; set; }
        private System.IO.FileAccess access { get; set; }
        private FileShare share { get; set; }
        private FileOptions options { get; set; }

        public PWFileContext(PWDataSourceProvider provider, string docFullPath, IDokanFileInfo info, FileMode mode, System.IO.FileAccess access, FileShare share, FileOptions options)
        {
            this.provider = provider;
            this.docFullPath = docFullPath;
            this.info = info;
            this.mode = mode;
            this.access = access;
            this.share = share;
            this.options = options;
            this.updateServerCopyOnWrite = true;
            this.OpenByExplorer = false;
        }

        public static PWFileContext CreateFileContext(PWDataSourceProvider provider, string docFullPath, IDokanFileInfo info, FileMode mode, System.IO.FileAccess access, FileShare share = FileShare.None, FileOptions options = FileOptions.None)
        {
            // 原来是创建时获取文件对象。但是在多线程环境下，可能原来路径上文件被别的进程删除
            // 所以每次做文件读取写入等操作时重新获取
            // var pw_doc = provider.DocumentHelper.GetDocumentByNamePath(docFullPath);
            return new PWFileContext(
                provider,
                docFullPath,
                info,
                mode,
                access,
                share,
                options
            );
        }

        /// <summary>
        /// Read data into given buffer at given offset
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public int Read(byte[] buffer, long offset)
        {
            var bytesRead = 0;
            var localWorkDirPath = this.GetPWDocLocalWorkPath();
            if (localWorkDirPath != null)
            {
                using (var stream = new FileStream(localWorkDirPath, this.mode, this.access))
                {
                    stream.Position = offset;
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                }
            }
            return bytesRead;
        }

        /// <summary>
        /// Write data from given buffer at given offset
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public void Write(byte[] buffer, long offset)
        {
            var localWorkDirPath = this.GetPWDocLocalWorkPath();
            if (localWorkDirPath != null)
            {
                using (var stream = new FileStream(localWorkDirPath, FileMode.Open, System.IO.FileAccess.Write))
                {
                    stream.Position = offset;
                    var bytesToCopy = GetNumOfBytesToCopy(buffer.Length, offset, stream);
                    //var bytesToCopy = buffer.Length;
                    stream.Write(buffer, 0, bytesToCopy);
                }
                this.UpdateServerCopy();
            }
        }

        /// <summary>
        /// Append data in given buffer to end of file
        /// </summary>
        /// <param name="buffer"></param>
        public void Append(byte[] buffer)
        {
            var localWorkDirPath = this.GetPWDocLocalWorkPath();
            if (localWorkDirPath != null)
            {
                using (var stream = new FileStream(localWorkDirPath, FileMode.Open, System.IO.FileAccess.Write))
                {
                    // Offset of -1 is an APPEND: https://docs.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-writefile
                    var offset = -1;
                    stream.Position = offset;
                    var bytesToCopy = GetNumOfBytesToCopy(buffer.Length, offset, stream);
                    //var bytesToCopy = buffer.Length;
                    stream.Write(buffer, 0, bytesToCopy);
                }
                this.UpdateServerCopy();
            }
        }

        /// <summary>
        /// Flush the internal buffer, called before file closes
        /// </summary>
        public void Flush()
        {
            var localWorkDirPath = this.GetPWDocLocalWorkPath();
            if (localWorkDirPath != null)
            {
                using (var stream = new FileStream(localWorkDirPath, FileMode.Open, System.IO.FileAccess.Write))
                {
                    stream.Flush();
                }
                this.UpdateServerCopy();
            }
        }

        /// <summary>
        /// Set Length of file
        /// </summary>
        /// <param name="length"></param>
        public void SetLength(long length)
        {
            var localWorkDirPath = this.GetPWDocLocalWorkPath();
            if (localWorkDirPath != null)
            {
                using (var stream = new FileStream(localWorkDirPath, FileMode.Open, System.IO.FileAccess.Write))
                {
                    stream.SetLength(length);
                }
                this.UpdateServerCopy();
            }
        }

        /// <summary>
        /// Lock given part of the file
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public void Lock(long offset, long length)
        {
            var localWorkDirPath = this.GetPWDocLocalWorkPath();
            if (localWorkDirPath != null)
            {
                using (var stream = new FileStream(localWorkDirPath, FileMode.Open, System.IO.FileAccess.Write))
                {
                    stream.Lock(offset, length);
                }
            }            
        }

        /// <summary>
        /// Unlock given part of the file
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public void Unlock(long offset, long length)
        {
            var localWorkDirPath = this.GetPWDocLocalWorkPath();
            if (localWorkDirPath != null)
            {
                using (var stream = new FileStream(localWorkDirPath, FileMode.Open, System.IO.FileAccess.Write))
                {
                    stream.Unlock(offset, length);
                }
            }
        }        

        protected Int32 GetNumOfBytesToCopy(Int32 bufferLength, long offset, FileStream stream)
        {
            if (this.info.PagingIo)
            {
                var longDistanceToEnd = stream.Length - offset;
                var isDistanceToEndMoreThanInt = longDistanceToEnd > Int32.MaxValue;
                if (isDistanceToEndMoreThanInt) return bufferLength;
                var distanceToEnd = (Int32)longDistanceToEnd;
                if (distanceToEnd < bufferLength) return distanceToEnd;
                return bufferLength;
            }
            return bufferLength;
        }

        private string _localWorkDirPath { get; set; }

        /// <summary>
        /// 将文档检出到本地，返回工作目录下路径。
        /// 缓存了检出结果，在该对象的生命周期里，只检出一次
        /// </summary>
        /// <param name="checkout"></param>
        /// <returns></returns>
        private string GetPWDocLocalWorkPath()
        {
            var pw_doc = provider.DocumentHelper.GetDocumentByNamePath(docFullPath);
            if (pw_doc == null)
            {
                return null;
            }
            //if (this.share.HasFlag(FileShare.Delete))
            //{
            // 如果是删除的情况，不应该去检出
            // 但是从文件flag里无法判断。所以还是不能这样操作
            //    return null;
            //}

            this.isDocCheckOut = true;
            if (this.access == System.IO.FileAccess.Read)
            {
                // 再检查share的情况，如果是要删除，自然不用checkout            
                if (this.share == FileShare.Read)
                {
                    this.isDocCheckOut = false;
                }
            }
            
            if (this.isDocCheckOut && !pw_doc.locked)
            {
                // 可能前面操作中文件释放了，例如进行了移动操作，但又重新写入，则要重新检出
                this._localWorkDirPath = null;
            }


            if (this._localWorkDirPath == null)
            {                
                // TODO，不能checkout时但又调用了写打开方式的处理，抛UnauthorizedAccessException好像不对
                if (this.isDocCheckOut && pw_doc.locked && !pw_doc.locked_by_me)
                {
                    // 导出或者在别的机器上检出了，但是要读写，则没有权限
                    throw new IOException("PW文件锁定");
                }
                try
                {
                    this._localWorkDirPath = this.provider.DocumentHelper.OpenDocument(pw_doc, this.isDocCheckOut);
                    // 记录打开文件的进程id，以便进行资源回收
                    // 但是对于文件资源管理器，该对象的生命周期结束就释放文件
                    if (!this.IsOpenByExplorer())
                    {
                        this.provider.PWDocProcessTracker.Update(pw_doc.id, this.info.ProcessId);
                    }
                    
                }
                catch(PWException e)
                {
                    if (e.PWErrorId == 58085)
                    {
                        // 对于具有最终状态的文档，操作无法执行。,错误码:58085
                        // 然而也用了读写打开的方式
                        throw new IOException("PW文件处于最终状态");
                    }
                    else
                    {
                        throw e;
                    }                    
                }              
                
            }
            return this._localWorkDirPath;
        }

        private void UpdateServerCopy()
        {
            if (this.updateServerCopyOnWrite)
            {
                var pw_doc = provider.DocumentHelper.GetDocumentByNamePath(docFullPath);
                if (pw_doc != null)
                {
                    this.provider.DocumentHelper.UpdateCheckOutDocument(pw_doc);
                }                
            }
        }


        /// <summary>
        /// 针对explorer发起的文件请求，例如复制文件，在生命周期内explorer.exe是常驻的
        /// 因此不能走跟踪进程关闭，释放文件占用的方式。
        /// 同时explorer资源管理器不会修改文件内容，在一个请求里释放也问题不大
        /// </summary>
        /// <returns></returns>
        private bool IsOpenByExplorer()
        {
            try
            {
                var process = Process.GetProcessById(this.info.ProcessId);
                if (process != null && process.ProcessName == "explorer")
                {
                    this.OpenByExplorer = true;
                    return true;
                }
                else
                {
                    this.OpenByExplorer = false;
                    return false;
                }
            }
            catch (ArgumentException)
            {
                // 进程不存在
                this.OpenByExplorer = false;
                return false;
            }
            catch (InvalidOperationException)
            {
                // 没有权限访问的进程
                this.OpenByExplorer = false;
                return false;
            }
            
        }
        // Implement IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // TODO, 看要不要怎么做释放资源，比如是不是更新保存的副本，释放pw文件占用啥的
                    if (this.OpenByExplorer)
                    {
                        var pw_doc = provider.DocumentHelper.GetDocumentByNamePath(docFullPath);
                        if (pw_doc != null)
                        {
                            this.provider.DocumentHelper.Free(pw_doc.id);
                        }
                        
                    }
                }
                _disposed = true;
            }
        }
    }

}
