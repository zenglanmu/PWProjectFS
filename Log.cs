using System;
using System.IO;
using System.Threading;

namespace PWProjectFS.Log
{
    public class ErrorLog
    {
        //using lock here to ensure not conflict
        private static ReaderWriterLockSlim lock_ = new ReaderWriterLockSlim();

        public static void WriteErrorLog(string LogText)
        {
            lock_.EnterWriteLock();
            try
            {
                string AppPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string StorePath = AppPath + "\\Bentley\\Logs\\";
                Directory.CreateDirectory(StorePath);
                string logfile = StorePath + "DDMSLIB.log";
                if (File.Exists(logfile))
                {
                    FileInfo fileInfo = new FileInfo(logfile);
                    if (fileInfo.Length > (long)10485760)
                    {
                        string baseDirectory = StorePath;
                        DateTime now = DateTime.Now;
                        string str2 = string.Concat(baseDirectory, "DDMSLIB", now.ToString("yyMMddhhss"), ".log");
                        fileInfo.MoveTo(str2);
                        if (File.Exists(logfile))
                        {
                            File.Delete(logfile);
                        }
                        using (StreamWriter streamWriter = File.CreateText(logfile))
                        {
                            streamWriter.WriteLine("Hello,This is DDMSLIB ErrorLog !");
                            streamWriter.WriteLine(DateTime.Now.ToString());
                            streamWriter.WriteLine();
                            streamWriter.Close();
                        }
                    }
                    StreamWriter streamWriter1 = File.AppendText(logfile);
                    streamWriter1.WriteLine(DateTime.Now.ToString());
                    streamWriter1.WriteLine(LogText);
                    streamWriter1.WriteLine();
                    streamWriter1.Close();
                }
                else
                {
                    using (StreamWriter streamWriter2 = File.CreateText(logfile))
                    {
                        streamWriter2.WriteLine("Hello,This is DDMSLIB ErrorLog !");
                        streamWriter2.WriteLine(DateTime.Now.ToString());
                        streamWriter2.WriteLine(LogText);
                        streamWriter2.WriteLine();
                        streamWriter2.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lock_.ExitWriteLock();
            }
        }

        //accept log file name
        public static void WriteErrorLog(string logfilename, string LogText)
        {
            lock_.EnterWriteLock();
            try
            {
                string AppPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string StorePath = AppPath + "\\Bentley\\Logs\\";
                Directory.CreateDirectory(StorePath);
                string logfile = StorePath + logfilename;
                if (File.Exists(logfile))
                {
                    FileInfo fileInfo = new FileInfo(logfile);
                    if (fileInfo.Length > (long)10485760)
                    {
                        string baseDirectory = StorePath;
                        DateTime now = DateTime.Now;
                        string str2 = string.Concat(baseDirectory, logfilename, now.ToString("yyMMddhhss"), ".log");
                        fileInfo.MoveTo(str2);
                        if (File.Exists(logfile))
                        {
                            File.Delete(logfile);
                        }
                        using (StreamWriter streamWriter = File.CreateText(logfile))
                        {
                            streamWriter.WriteLine("Hello,This is ErrorLog:" + logfilename);
                            streamWriter.WriteLine(DateTime.Now.ToString());
                            streamWriter.WriteLine();
                            streamWriter.Close();
                        }
                    }
                    StreamWriter streamWriter1 = File.AppendText(logfile);
                    streamWriter1.WriteLine(DateTime.Now.ToString());
                    streamWriter1.WriteLine(LogText);
                    streamWriter1.WriteLine();
                    streamWriter1.Close();
                }
                else
                {
                    using (StreamWriter streamWriter2 = File.CreateText(logfile))
                    {
                        streamWriter2.WriteLine("Hello,This is ErrorLog:" + logfilename);
                        streamWriter2.WriteLine(DateTime.Now.ToString());
                        streamWriter2.WriteLine(LogText);
                        streamWriter2.WriteLine();
                        streamWriter2.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lock_.ExitWriteLock();
            }
        }

        //accept full file path string
        public static void WriteErrorLog1(string logpath, string LogText)
        {
            lock_.EnterWriteLock();
            try
            {
                int index = logpath.LastIndexOf("\\") + 1;

                string StorePath = logpath.Substring(0, index - 1);
                Directory.CreateDirectory(StorePath);
                string logfilename = logpath.Substring(index, logpath.Length - index);
                string logfile = logpath;
                if (File.Exists(logfile))
                {
                    FileInfo fileInfo = new FileInfo(logfile);
                    if (fileInfo.Length > (long)10485760)
                    {
                        string baseDirectory = StorePath;
                        DateTime now = DateTime.Now;
                        string str2 = string.Concat(baseDirectory, logfilename, now.ToString("yyMMddhhss"), ".log");
                        fileInfo.MoveTo(str2);
                        if (File.Exists(logfile))
                        {
                            File.Delete(logfile);
                        }
                        using (StreamWriter streamWriter = File.CreateText(logfile))
                        {
                            streamWriter.WriteLine("Hello,This is ErrorLog: " + logfilename);
                            streamWriter.WriteLine(DateTime.Now.ToString());
                            streamWriter.WriteLine();
                            streamWriter.Close();
                        }
                    }
                    StreamWriter streamWriter1 = File.AppendText(logfile);
                    streamWriter1.WriteLine(DateTime.Now.ToString());
                    streamWriter1.WriteLine(LogText);
                    streamWriter1.WriteLine();
                    streamWriter1.Close();
                }
                else
                {
                    using (StreamWriter streamWriter2 = File.CreateText(logfile))
                    {
                        streamWriter2.WriteLine("Hello,This is ErrorLog: " + logfilename);
                        streamWriter2.WriteLine(DateTime.Now.ToString());
                        streamWriter2.WriteLine(LogText);
                        streamWriter2.WriteLine();
                        streamWriter2.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                lock_.ExitWriteLock();
            }
        }

    }

    
}
