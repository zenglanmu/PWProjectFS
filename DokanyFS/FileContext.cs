using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DokanNet;
using Microsoft.Win32;
using static DokanNet.FormatProviders;
using FileAccess = DokanNet.FileAccess;
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
        private PWDataSourceProvider provider { get; set; }
        /// <summary>
        /// 关联的pw文件
        /// </summary>
        public PWDocument pw_doc { get; private set; }

        /// <summary>
        /// 从info里获取是否是按照pagingIo方式读取，关系到计算GetNumOfBytesToCopy
        /// </summary>
        private bool isPagingIo { get; set; }

        private FileMode mode { get; set; }
        private System.IO.FileAccess access { get; set; }
        private FileShare share { get; set; }
        private FileOptions options { get; set; }

        public PWFileContext(PWDataSourceProvider provider, PWDocument pw_doc, bool isPagingIo, FileMode mode, System.IO.FileAccess access, FileShare share, FileOptions options)
        {
            this.provider = provider;
            this.pw_doc = pw_doc;
            this.isPagingIo = isPagingIo;
            this.mode = mode;
            this.access = access;
            this.share = share;
            this.options = options;
        }

        public static PWFileContext CreateFileContext(PWDataSourceProvider provider, string docFullPath, bool isPagingIo, FileMode mode, System.IO.FileAccess access, FileShare share = FileShare.None, FileOptions options = FileOptions.None)
        {
            var pw_doc = provider.DocumentHelper.GetDocumentByNamePath(docFullPath);
            // 即使pw_doc为空也得返回。后面读取buffer的操作再处理
            return new PWFileContext(
                provider,
                pw_doc,
                isPagingIo,
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
            if (pw_doc != null)
            {
                var localWorkDirPath = this.GetPWDocLocalWorkPath();
                if (localWorkDirPath != null)
                {
                    using (var stream = new FileStream(localWorkDirPath, this.mode, this.access))
                    {
                        stream.Position = offset;
                        bytesRead = stream.Read(buffer, 0, buffer.Length);
                    }
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
            if (pw_doc != null)
            {
                var localWorkDirPath = this.GetPWDocLocalWorkPath();
                if (localWorkDirPath != null)
                {
                    using (var stream = new FileStream(localWorkDirPath, FileMode.Open, System.IO.FileAccess.Write))
                    {
                        stream.Position = offset;
                        var bytesToCopy = GetNumOfBytesToCopy(buffer.Length, offset, stream);
                        stream.Write(buffer, 0, bytesToCopy);
                    }
                }
            }            
        }

        /// <summary>
        /// Append data in given buffer to end of file
        /// </summary>
        /// <param name="buffer"></param>
        public void Append(byte[] buffer)
        {
            if (pw_doc != null)
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
                        stream.Write(buffer, 0, bytesToCopy);
                    }
                }
            }            
        }

        /// <summary>
        /// Flush the internal buffer, called before file closes
        /// TODO,是否要更新服务器副本，检入文件
        /// </summary>
        public void Flush()
        {
            if (pw_doc != null)
            {
                var localWorkDirPath = this.GetPWDocLocalWorkPath();
                if (localWorkDirPath != null)
                {
                    using (var stream = new FileStream(localWorkDirPath, FileMode.Open, System.IO.FileAccess.Write))
                    {
                        stream.Flush();
                    }
                }
            }
            
        }

        /// <summary>
        /// Set Length of file
        /// </summary>
        /// <param name="length"></param>
        public void SetLength(long length)
        {
            if (pw_doc != null)
            {
                var localWorkDirPath = this.GetPWDocLocalWorkPath();
                if (localWorkDirPath != null)
                {
                    using (var stream = new FileStream(localWorkDirPath, FileMode.Open, System.IO.FileAccess.Write))
                    {
                        stream.SetLength(length);
                    }
                }
            }            
        }

        /// <summary>
        /// Lock given part of the file
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public void Lock(long offset, long length)
        {
            if (pw_doc != null)
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
            
        }

        /// <summary>
        /// Unlock given part of the file
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        public void Unlock(long offset, long length)
        {
            if (pw_doc != null)
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
        }        

        protected Int32 GetNumOfBytesToCopy(Int32 bufferLength, long offset, FileStream stream)
        {
            if (this.isPagingIo)
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
            if (this._localWorkDirPath == null)
            {
                if (this.share.HasFlag(FileShare.Delete))
                {
                    // 如果是删除的情况，不应该去检出
                    return null;
                }
                // 只有当access和share都是Read时，才认为是只读打开
                var checkout = true;
                if (this.access == System.IO.FileAccess.Read)
                {
                    // 再检查share的情况，如果是要删除，自然不用checkout            
                    if(this.share == FileShare.Read )
                    {
                        checkout = false;
                    }
                }
                // TODO，不能checkout时但又调用了写打开方式的处理，抛UnauthorizedAccessException好像不对
                if (checkout && pw_doc.locked && !pw_doc.locked_by_me)
                {
                    // 导出或者在别的机器上检出了，但是要读写，则没有权限
                    throw new IOException("PW文件锁定");
                }
                try
                {
                    checkout = true; // 先强制按检出弄，测试写入的功能
                    this._localWorkDirPath = this.provider.DocumentHelper.OpenDocument(pw_doc, checkout);
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
                    if(this.pw_doc.locked && this.pw_doc.locked_by_me)
                    {
                        //this.provider.DocumentHelper.Free(pw_doc.id);
                    }
                }
                _disposed = true;
            }
        }
    }

}
