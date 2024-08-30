using DokanNet;
using DokanNet.Logging;
using PWProjectFS.PWProvider;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static DokanNet.FormatProviders;
using FileAccess = DokanNet.FileAccess;

namespace PWProjectFS.DokanyFS
{
    //internal class PWFSOperations
    internal partial class PWFSOperations: IDokanOperations
    {
        private int base_pw_projectno;
        private PWDataSourceProvider provider;

        private readonly ILogger _logger;
        private const FileAccess DataAccess = FileAccess.ReadData | FileAccess.WriteData | FileAccess.AppendData |
                                              FileAccess.Execute |
                                              FileAccess.GenericExecute | FileAccess.GenericWrite |
                                              FileAccess.GenericRead;

        private const FileAccess DataWriteAccess = FileAccess.WriteData | FileAccess.AppendData |
                                                   FileAccess.Delete |
                                                   FileAccess.GenericWrite;

        public PWFSOperations(ILogger logger, int base_pw_projectno, PWDataSourceProvider provider)
        {
            this._logger = logger;
            this.base_pw_projectno = base_pw_projectno;
            this.provider = provider;
        }

        protected NtStatus Trace(string method, string fileName, IDokanFileInfo info, NtStatus result,
            params object[] parameters)
        {
#if TRACE
            var extraParameters = parameters != null && parameters.Length > 0
                ? ", " + string.Join(", ", parameters.Select(x => string.Format(DefaultFormatProvider, "{0}", x)))
                : string.Empty;

            _logger.Debug(DokanFormat($"{method}('{fileName}', {info}{extraParameters}) -> {result}"));
#endif

            return result;
        }

        private NtStatus Trace(string method, string fileName, IDokanFileInfo info,
            FileAccess access, FileShare share, FileMode mode, FileOptions options, FileAttributes attributes,
            NtStatus result)
        {
#if TRACE
            _logger.Debug(
                DokanFormat(
                    $"{method}('{fileName}', {info}, [{access}], [{share}], [{mode}], [{options}], [{attributes}]) -> {result}"));
#endif

            return result;
        }



        /// <summary>
        /// wrapperred mount operation
        /// </summary>
        /// <param name="projectno"></param>
        /// <param name="mountPath"></param>
        /// <param name="provider"></param>
        public static void Mount(int projectno, string mountPath, PWDataSourceProvider provider)
        {
            //Didn't get mount as network to work
            bool useNetWork = false;
            using (var dokanLogger = new ConsoleLogger("[ProjectWise] "))
            using(var dokan = new Dokan(dokanLogger))
            {  
                var fs = new PWFSOperations(dokanLogger, projectno, provider);
                var dokanBuilder = new DokanInstanceBuilder(dokan)
                        .ConfigureLogger(() => dokanLogger)
                        .ConfigureOptions(options =>
                        {
                            options.Options = DokanOptions.CaseSensitive | DokanOptions.MountManager | DokanOptions.CurrentSession;
                            if (useNetWork)
                            {
                                options.Options = options.Options | DokanOptions.NetworkDrive | DokanOptions.EnableNetworkUnmount;
                                options.UNCName = @"\\myfs\dokan";
                            }
                            else
                            {
                                options.Options = options.Options | DokanOptions.RemovableDrive;
                            }
                            
                            //options.Options = DokanOptions.DebugMode | DokanOptions.EnableNotificationAPI;
#if DEBUG
                            options.Options = options.Options | DokanOptions.DebugMode;
                            // single thread mode, easy for debug, but bad for performance
                            options.SingleThread = true;
#endif
                            options.MountPoint = mountPath;
                        });
                using (DokanInstance dokanInstance = dokanBuilder.Build(fs))
                {
                    var mountTask = dokanInstance.WaitForFileSystemClosedAsync(uint.MaxValue)
                        .ContinueWith(t=> { 
                            provider.CancelProcessScan(); 
                        });
                    var scanProcessTask = provider.PWDocProcessTracker.ContinousScan();
                    // 两个任务同时后台进行，等待两个任务都完成。理论上不mount时，scanProcessTask也会自动退出
                    var allTasks = Task.WhenAll(mountTask, scanProcessTask);
                    allTasks.Wait();
                }
            }
        }


        /// <summary>
        /// 获取完整的路径，拼接上父路径
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private string GetPath(string filename)
        {
            var _base_pw_path = "";
            if (this.base_pw_projectno == 0)
            {
                _base_pw_path = "";
            }
            else
            {
                _base_pw_path = this.provider.ProjectHelper.GetNamePathByProjectId(this.base_pw_projectno);
            }

            _base_pw_path = _base_pw_path.TrimEnd('\\');
            filename = filename.TrimStart('\\');

            if (filename == "\\" || string.IsNullOrWhiteSpace(filename))
            {
                // base pw dir
                return _base_pw_path;
            }
            else
            {
                return _base_pw_path + "\\" + filename;
            }            
        }
    }
}
