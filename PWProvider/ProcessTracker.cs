using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using PWProjectFS.PWApiWrapper;

namespace PWProjectFS.PWProvider
{

    /// <summary>
    /// 记录有哪些流程打开了pw上对应的文件，如果文件没有被进程占用了，则释放pw文件占用
    /// 因为从文件系统的驱动层面，不好获知程序是否关闭，是否该释放文件占用
    /// </summary>
    public class PWDocProcessTracker
    {
        public readonly int scanInterval; // 以毫秒记录的扫描间隔
        private bool scanning; // 正在扫描中，防止多线程冲突多次扫描
        private readonly PWDataSourceProvider provider;
        private object _lock = null; // 为了线程安全，得锁定
        private CancellationToken cancelToken; // cancel token

        private Dictionary<string, List<int>> docToProcessId;

        public PWDocProcessTracker(int scanInterval, PWDataSourceProvider provider, CancellationToken cancelToken)
        {
            this._lock = new object();
            this.docToProcessId = new Dictionary<string, List<int>>();
            this.scanning = false;
            this.scanInterval = scanInterval;
            this.provider = provider;
            this.cancelToken = cancelToken;
        }

        public void Update(string documentId, int processId)
        {
            lock (this._lock)
            {
                if (this.docToProcessId.ContainsKey(documentId))
                {
                    if (this.docToProcessId[documentId].Contains(processId))
                    {
                        // 已加过
                    }
                    else
                    {
                        this.docToProcessId[documentId].Add(processId);
                    }
                }
                else
                {
                    this.docToProcessId.Add(documentId, new List<int>() { processId });
                }
            }
        }

        public void Scan()
        {
            lock (this._lock)
            {
                var keys = this.docToProcessId.Keys.ToList();
                foreach(var key in keys)
                {
                    var values = this.docToProcessId[key];
                    this.docToProcessId[key] = new List<int>();
                    foreach (var value in values)
                    {
                        try
                        {
                            var process = Process.GetProcessById(value);
                            if (process != null)
                            {
                                this.docToProcessId[key].Add(value);
                            }
                        }
                        catch (ArgumentException)
                        {
                            // 进程不存在
                        }
                        catch (InvalidOperationException)
                        {
                            // 没有权限访问的进程，不跟踪占用了，也即会清除文件占用
                        }
                    }
                }

                List<string> idsToFree = new List<string>();

                foreach (var pair in this.docToProcessId)
                {
                    if (pair.Value.Count == 0)
                    {
                        idsToFree.Add(pair.Key);
                    }
                }

                foreach (var documentId in idsToFree)
                {
                    this.docToProcessId.Remove(documentId);
                    try
                    {
                        this.provider.Activate();
                        this.provider.DocumentHelper.Free(documentId);
                    }catch(PWException e)
                    {
                        Console.WriteLine($"free doc failed for id {documentId}");

                    }
                    
                }
            }
        }


        /// <summary>
        /// 死循环持续扫描，除非被cancel
        /// </summary>
        public Task ContinousScan()
        {
            var task = Task.Run(() =>
            {
                if (!this.scanning)
                {
                    this.scanning = true;
                    while (!cancelToken.IsCancellationRequested)
                    {
                        this.Scan();
                        Thread.Sleep(this.scanInterval);
                    }
                    this.scanning = false;
                }

            });
            return task;
        }


        /// <summary>
        /// 如果是删除文件的场景，除了调用删除方法外，还应该这里去除
        /// </summary>
        /// <param name="documentId"></param>
        public void Delete(string documentId)
        {
            lock (this._lock)
            {
                if (this.docToProcessId.ContainsKey(documentId))
                {
                    this.docToProcessId.Remove(documentId);
                }
            }
        }
    }
}
