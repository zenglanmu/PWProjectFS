using DokanNet;
using DokanNet.Logging;
using PWProjectFS.PWProvider;
using System.IO;
using System.Linq;
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

        public static void Mount(int projectno, string mountPath, PWDataSourceProvider provider)
        {
            using (var dokanLogger = new ConsoleLogger("[ProjectWise] "))
            using(var dokan = new Dokan(dokanLogger))
            {
                var fs = new PWFSOperations(dokanLogger, projectno, provider);
                var dokanBuilder = new DokanInstanceBuilder(dokan)
                        .ConfigureLogger(() => dokanLogger)
                        .ConfigureOptions(options =>
                        {
                            options.Options = DokanOptions.DebugMode | DokanOptions.EnableNotificationAPI;
                            options.MountPoint = mountPath;
                        });
                using (var dokanInstance = dokanBuilder.Build(fs))
                {
                    var task = dokanInstance.WaitForFileSystemClosedAsync(uint.MaxValue);
                    task.Wait();
                }
            }
        }


        private string _base_pw_path = null;
        /// <summary>
        /// 获取完整的路径，拼接上父路径
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private string GetPath(string filename)
        {
            if (filename == "\\")
            {
                // base pw dir
                return this._base_pw_path;
            }
            if (this._base_pw_path == null)
            {
                if (this.base_pw_projectno == 0)
                {
                    this._base_pw_path = "";
                }
                else
                {
                    this._base_pw_path = this.provider.ProjectHelper.GetNamePathByProjectId(this.base_pw_projectno);
                }
            }
            return this._base_pw_path + "\\" + filename;
        }
    }
}
