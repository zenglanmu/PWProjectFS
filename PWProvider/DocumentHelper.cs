using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DokanNet;
using PWProjectFS.PWApiWrapper;
using DocumentType= PWProjectFS.PWApiWrapper.dmscli.DocumentType;

namespace PWProjectFS.PWProvider
{
    public class PWDocument
    {
        public string id { get; set; }
        /* 文件的uuid，可以唯一确定 */
        public int documentId { get; set; }
        /*documentId和projectId也可确定文件 */
        public int projectId { get; set; }
        public string name { get; set; }
        public string filename { get; set; }
        public string description { get; set; }
        public string version { get; set; }
        public string mimetype { get; set; }
        public DateTime create_time { get; set; }
        public DateTime update_time { get; set; }
        public DateTime file_update_time { get; set; }
        public int created_by { get; set; }
        public int updated_by { get; set; }
        public long filesize { get; set; }
        public bool locked { get; set; }
        public bool is_final { get; set; } // 最终状态
        public bool locked_by_me { get; set; }
        public bool is_latest { get; set; }

        public DocumentType documentType { get; set; }

        public FileInformation toFileInformation()
        {
            var Attributes = FileAttributes.Archive;
            if ((this.locked && !this.locked_by_me) || this.is_final)
            {
                Attributes = Attributes | FileAttributes.ReadOnly;
            }
            var file = new FileInformation
            {
                Attributes = Attributes,
                CreationTime = this.create_time,
                LastAccessTime = this.update_time,
                LastWriteTime = this.file_update_time,
                Length = this.filesize,
                FileName = this.filename
            };
            return file;
        }
    }
    public class DocumentHelper
    {
        private object _lock = null;
        private PWResourceCache m_cache;
        private ProjectHelper m_projectHelper;

        public DocumentHelper(object _lock, PWResourceCache cache, ProjectHelper projectHelper)
        {
            this._lock = _lock;
            this.m_cache = cache;
            this.m_projectHelper = projectHelper;
        }

        /// <summary>
        /// 从AADMSBUFFER_DOCUMENT读取文档
        /// </summary>
        /// <param name="i">在buffer里的序号</param>
        /// <returns></returns>
        private PWDocument PopulateDocumentFromBuffer(int i)
        {
            int timezone = this.m_cache.GetTimeZoneMinutes();
            var doc = new PWDocument();
            doc.id = dmscli.aaApi_GetDocumentGuidProperty(dmscli.DocumentProperty.DocGuid, i).ToString();
            doc.documentId = dmscli.aaApi_GetDocumentNumericProperty(dmscli.DocumentProperty.ID, i);
            doc.projectId = dmscli.aaApi_GetDocumentNumericProperty(dmscli.DocumentProperty.ProjectID, i);
            doc.name = dmscli.aaApi_GetDocumentStringProperty(dmscli.DocumentProperty.Name, i);
            doc.filename = dmscli.aaApi_GetDocumentStringProperty(dmscli.DocumentProperty.FileName, i);
            doc.description = dmscli.aaApi_GetDocumentStringProperty(dmscli.DocumentProperty.Desc, i);
            doc.version = dmscli.aaApi_GetDocumentStringProperty(dmscli.DocumentProperty.Version, i);
            doc.mimetype = dmscli.aaApi_GetDocumentStringProperty(dmscli.DocumentProperty.MimeType, i);
            doc.create_time = Util.ToUtcTime(dmscli.aaApi_GetDocumentStringProperty(dmscli.DocumentProperty.CreateTime, i), timezone);
            doc.update_time = Util.ToUtcTime(dmscli.aaApi_GetDocumentStringProperty(dmscli.DocumentProperty.UpdateTime, i), timezone);
            doc.file_update_time = Util.ToUtcTime(dmscli.aaApi_GetDocumentStringProperty(dmscli.DocumentProperty.FileUpdateTime, i), timezone);
            doc.created_by = dmscli.aaApi_GetDocumentNumericProperty(dmscli.DocumentProperty.CreatorID, i);
            doc.updated_by = dmscli.aaApi_GetDocumentNumericProperty(dmscli.DocumentProperty.UpdaterID, i);
            doc.filesize = (long)dmscli.aaApi_GetDocumentUint64Property(dmscli.DocumentProperty.Size, i);
            doc.locked = dmscli.aaApi_GetDocumentStringProperty(dmscli.DocumentProperty.DMSStatus, i).IndexOf("O") != -1;
            doc.is_final = dmscli.aaApi_GetDocumentNumericProperty(dmscli.DocumentProperty.HasFinalStatus, i) != 0;
            doc.locked_by_me = dmscli.aaApi_GetDocumentNumericProperty(dmscli.DocumentProperty.IsOutToMe, i) != 0;
            doc.is_latest = dmscli.aaApi_GetDocumentNumericProperty(dmscli.DocumentProperty.OriginalNumber, i) == 0;
            doc.documentType = (DocumentType)dmscli.aaApi_GetDocumentNumericProperty(dmscli.DocumentProperty.ItemType, i);
            return doc;
        }

        private static Guid SelectDocument(string documentId)
        {
            Guid guid = Util.ParseStringToGuid(documentId);
            switch (dmscli.aaApi_GUIDSelectDocument(ref guid))
            {
                case 0:
                    throw new PWException($"{documentId}对应文件不存在");
                case -1:
                    throw PWException.GetPWLastException();
            }
            return guid;
        }

        /// <summary>
        /// 读取一篇document
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        private PWDocument _Read(string documentId)
        {
            var documentGuid = SelectDocument(documentId);           
            var doc = this.PopulateDocumentFromBuffer(0);
            return doc;
        }

        public PWDocument Read(string documentId)
        {
            // use lock to ensure thread safe calling pw apis
            lock (this._lock)
            {
                var doc = this._Read(documentId);
                var cache_key = $"ReadDocumentsByParentProjectId:{doc.projectId}";
                Func<List<PWDocument>> get_value_func = () =>
                {
                    return this._ReadByParent(doc.projectId);

                };
                var docs = this.m_cache.TryGet(cache_key, get_value_func);
                var exist = docs.Where(x => x.id == doc.id).FirstOrDefault();
                if (exist != null)
                {
                    exist = doc; // update exist
                }
                else
                {
                    docs.Add(doc);
                }
                return doc;
            }
        }

        private List<PWDocument> _ReadByParent(int parentProjectId)
        {
            var docs = new List<PWDocument>();
            if (parentProjectId == 0)
            {
                // 顶层目录是不能放文件的
                return docs;
            }
            var docnums = dmscli.aaApi_SelectDocumentsByProjectId(parentProjectId);
            if (docnums == 0)
            {
                return new List<PWDocument>(); // Failed to select properties of all documents. There are no documents in specified project.
            }
            else if (docnums == -1)
            {
                throw PWException.GetPWLastException();
            }
            else
            {
                //pass
            }
            
            for(int i = 0; i < docnums; i++)
            {
                var doc = this.PopulateDocumentFromBuffer(i);
                docs.Add(doc);
            }            
            return docs;
        }

        public List<PWDocument> ReadByParent(int parentProjectId)
        {
            // use lock to ensure thread safe calling pw apis
            lock (this._lock)
            {
                var cache_key = $"ReadDocumentsByParentProjectId:{parentProjectId}";
                Func<List<PWDocument>> get_value_func = () =>
                {
                    return this._ReadByParent(parentProjectId);

                };
                return this.m_cache.TryGet(cache_key, get_value_func);
            }
        }

        private void _Delete(string documentId)
        {
            var documentGuid = Guid.Parse(documentId);
            if (!dmscli.aaApi_GUIDDeleteDocument(dmscli.DocumentDeleteMasks.None, ref documentGuid))
            {
                throw PWException.GetPWLastException();
            }
        }


        /// <summary>
        /// 解除文件占用，free之前会先检入文档
        /// </summary>
        /// <param name="documentId"></param>
        private void _Free(string documentId)
        {
            var documentGuid = Guid.Parse(documentId);
            // 是否已被检出，已检出的话先保存
            bool outToMe = false;
            var doc = this.Read(documentId);
            if (!dmscli.aaApi_IsDocumentCheckedOutToMe(doc.projectId, doc.documentId, out outToMe))
            {
                throw PWException.GetPWLastException();
            }
            if (outToMe)
            {
                if (!dmscli.aaApi_GUIDRefreshDocumentServerCopy(ref documentGuid))
                {
                    throw PWException.GetPWLastException();
                }
                if (!dmscli.aaApi_GUIDFreeDocument(ref documentGuid, this.m_cache.GetCurrentUserId()))
                {
                    throw PWException.GetPWLastException();
                }
            }
            // 本地副本
            if (!dmscli.aaApi_GUIDPurgeDocumentCopy(ref documentGuid, this.m_cache.GetCurrentUserId()))
            {
                throw PWException.GetPWLastException();
            }


        }

        public void Free(string documentId)
        {
            lock (this._lock)
            {
                this._Free(documentId);
            }
        }

        public void Delete(string documentId)
        {
            // use lock to ensure thread safe calling pw apis
            lock (this._lock)
            {
                var doc = this.Read(documentId);
                // 检查是否检出了，如果本机检出了就删除，否则抛IOException
                if (doc.is_final)
                {
                    throw new IOException("PW文件处于最终状态");
                }
                if(doc.locked)
                {
                    if (doc.locked_by_me)
                    {
                        this._Free(documentId);
                    }
                    else
                    {
                        throw new IOException("PW文件锁定");
                    }
                    
                }
                this._Delete(documentId);
                // update cache
                var docs = this.ReadByParent(doc.projectId);
                var exist = docs.Where(x => x.id == doc.id).FirstOrDefault();
                if (exist != null)
                {
                    docs.Remove(exist);
                }
                else
                {
                    // nothing
                }
            }
        }


        /// <summary>
        ///本地文件上传， 创建文件
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private PWDocument _Create(int projectId, string filelocalpath)
        {
            string filename = filelocalpath.Substring(filelocalpath.LastIndexOf("\\") + 1);
            string lpctstrName = filename;
            string lpctstrDesc = filename;
            int lApplicationId = dmscli.aaApi_GetFExtensionApplication(filename.Substring(filename.LastIndexOf(".") + 1));

            int plDocumentId = 0;
            var workingFileBufferSize = 4096;
            var lptstrWorkingFile = new StringBuilder(workingFileBufferSize);
            var plAttributeId = 0;
            var ret = dmscli.aaApi_CreateDocument(
                ref plDocumentId,  // plDocumentId
                projectId,// lProjectId 
                0, // lStorageId, pass 0 to use the project's storage .
                0, // lFileType 
                dmscli.DocumentType.Normal, // lItemType 
                lApplicationId, // lApplicationId 
                0, // lDepartmentId 
                0, //lWorkspaceProfileId 
                filelocalpath, // 本地文件路径
                filename, // lpctstrFileName 
                lpctstrName, // lpctstrName 
                lpctstrDesc, // lpctstrDesc
                "", //lpctstrVersion 
                false, // bLeaveOut。  If this parameter is FALSE then the document will be checked in. If this parameter is TRUE then the document will be copied to the working directory and marked as checked out by the current user
                dmscli.DocumentCreationFlag.Default, // ulFlags， 正常创建情况
                lptstrWorkingFile,// copy out后的在工作目录的路径
                workingFileBufferSize, 
                ref plAttributeId
             );
            if (!ret || plDocumentId<=0)
            {
                throw PWException.GetPWLastException();
            }
            var selRet = dmscli.aaApi_SelectDocument(projectId, plDocumentId);
            if (selRet==-1)
            {
                throw PWException.GetPWLastException();
            }else if (selRet == 0)
            {
                throw new PWException($"文件未找到,projectId:{projectId},plDocumentId:{plDocumentId}");
            }
            var pw_doc = this.PopulateDocumentFromBuffer(0);
            return pw_doc;
        }        


        /// <summary>
        /// 假如路径不存在，创建新文件，否则返回路径里的文件
        /// </summary>
        /// <param name="docFullPath"></param>
        /// <returns></returns>
        public PWDocument OpenOrCreate(string docFullPath)
        {
            if (docFullPath.EndsWith("\\"))
            {
                // 明显传的是目录的情况
                return null;
            }
            if (!docFullPath.Contains("\\"))
            {
                // 没有子路径的情况，显然不是文件
                return null;
            }
            lock (this._lock)
            {
                var pw_doc = this.GetDocumentByNamePath(docFullPath);
                if (pw_doc != null)
                {
                    // if already exist
                    return pw_doc;
                }
                string lpctstrPath = docFullPath.Substring(0, docFullPath.LastIndexOf("\\"));
                string filename = docFullPath.Substring(docFullPath.LastIndexOf("\\") + 1);
                var projectId = this.m_projectHelper.GetProjectIdByNamePath(lpctstrPath);
                if (projectId == -1)
                {
                    return null;
                }
                else
                {
                    using(var tempdir=new PWTempDir())
                    {
                        // create empty file then upload to pw
                        var filelocalpath = Path.Combine(tempdir.GetTempDir(), filename);
                        File.Create(filelocalpath).Dispose();
                        var doc = this._Create(projectId, filelocalpath);
                        // update cache
                        var docs = this.ReadByParent(doc.projectId);
                        var exist = docs.Where(x => x.id == doc.id).FirstOrDefault();
                        if (exist != null)
                        {
                            exist=doc; // 不应该走到这
                        }
                        else
                        {
                            docs.Add(doc);
                        }
                        return doc;
                    }
                    
                }
                
            }
        }

        private PWDocument _GetDocumentByNameAndProjectId(string filename, int projectId)
        {
            var searchItem = new dmscli.AADOCSELECT_ITEM();
            searchItem.ulFlags = 0;
            searchItem.lProjectId = projectId;
            searchItem.lpctstrFileName = filename;
            var lpSelectInfo = Util.StructureToPtr(searchItem);
            var docCounts = dmscli.aaApi_SelectDocuments2(lpSelectInfo, IntPtr.Zero, 0);
            Util.FreeHGlobal(ref lpSelectInfo);
            if (docCounts == 0)
            {
                return null;
            }

            var doc = this.PopulateDocumentFromBuffer(0);
            return doc;
        }

        public PWDocument GetDocumentByNamePath(string docFullPath)
        {
            if (docFullPath.EndsWith("\\"))
            {
                // 明显传的是目录的情况
                return null;
            }
            if (!docFullPath.Contains("\\"))
            {
                // 没有子路径的情况，显然不是文件
                return null;
            }
            string lpctstrPath = docFullPath.Substring(0, docFullPath.LastIndexOf("\\"));
            string filename = docFullPath.Substring(docFullPath.LastIndexOf("\\") + 1);
            var projectId = this.m_projectHelper.GetProjectIdByNamePath(lpctstrPath);
            if (projectId > -1)
            {
                // pass
            }
            else
            {
                return null;
            }
            var docs = this.ReadByParent(projectId);
            var exist = docs.Where(x => x.filename == filename).FirstOrDefault();
            return exist;
        }


        /// <summary>
        /// 将文件检出到本地，返回本地的路径
        /// </summary>
        /// <param name="PWDocument"></param>
        /// <returns></returns>
        private string _OpenDocument(PWDocument doc, bool checkout)
        {
            // 是否已被检出
            bool outToMe = false;
            var pDocGuid = Guid.Parse(doc.id);
            StringBuilder localPath = new StringBuilder(4096);
            if (!dmscli.aaApi_IsDocumentCheckedOutToMe(doc.projectId, doc.documentId, out outToMe))
            {
                throw PWException.GetPWLastException();
            }
            if (outToMe)
            {                         
                if (!dmscli.aaApi_GUIDGetDocumentCheckedOutFileName(ref pDocGuid, localPath, localPath.Capacity))
                {
                    throw new PWException("未在工作目录找到该文档");
                }
                else
                {
                    return localPath.ToString();
                }
            }
            else
            {
                var ret = false;
                if (checkout)
                {
                    // 检出处理
                    ret = dmscli.aaApi_GUIDCheckOutDocument(
                        ref pDocGuid,
                        null,//对应docWorkdir，如果传null，表示使用现有工作目录
                        localPath, // 检出的本地路径
                        localPath.Capacity);
                }
                else
                {
                    // copyout只读打开处理
                    // 检出处理
                    ret = dmscli.aaApi_GUIDCopyOutDocument(
                        ref pDocGuid,
                        null,//对应docWorkdir，如果传null，表示使用现有工作目录
                        localPath, // 检出的本地路径
                        localPath.Capacity);                   
                }
                if (!ret)
                {
                    throw PWException.GetPWLastException();
                }
                if (checkout)
                {
                    doc.locked = true;
                    doc.locked_by_me = true;
                }
                return localPath.ToString();

            }
        }

        public string OpenDocument(PWDocument doc, bool checkout = true)
        {
            lock (this._lock)
            {
                return this._OpenDocument(doc, checkout);
            }
        }

        public string OpenDocumentByPath(string docFullPath, bool checkout = true)
        {
            var doc = this.GetDocumentByNamePath(docFullPath);
            if (doc == null)
            {
                return null;
            }
            lock (this._lock)
            {
                return this._OpenDocument(doc, checkout);
            }
        }


        /// <summary>
        /// 检出的文档，本地有修改的话，更新服务器上保存的
        /// </summary>
        /// <param name="doc"></param>
        private void _UpdateCheckOutDocument(PWDocument doc)
        {
            var pDocGuid = Guid.Parse(doc.id);
            if (!dmscli.aaApi_GUIDRefreshDocumentServerCopy(ref pDocGuid))
            {
                throw PWException.GetPWLastException();
            }
        }

        public void UpdateCheckOutDocument(PWDocument doc)
        {
            lock (this._lock)
            {
                this._UpdateCheckOutDocument(doc);
            }
        }

        /// <summary>
        /// 更新文件名
        /// </summary>
        private void _Update(string documentId, string new_name)
        {
            var guid = Guid.Parse(documentId);
            var ret = dmscli.aaApi_GUIDModifyDocument(ref guid, 0, 0, 0, 0, 0, new_name, new_name, new_name);

            if (!ret)
            {
                throw PWException.GetPWLastException();
            }
            return;
        }

        /// <summary>
        /// 移动文件，同时支持更新文件名操作
        /// new_name为null则是只移动
        /// </summary>
        private void _MoveDocument(PWDocument doc, int targetProjectId, string new_name=null)
        { 
            var pTargetDocumentId = 0; //不覆盖原来的文件，因为如果有冲突的话，上层调用方应该先删除
            var ret = dmscli.aaApi_MoveDocument(
                doc.projectId,
                doc.documentId,
                targetProjectId,
                ref pTargetDocumentId,
                null,
                new_name,
                new_name,
                new_name,
                dmscli.DocumentCopyFlags.NoHooks
             );
            if (!ret)
            {
                throw PWException.GetPWLastException();
            }
            return;
        }


        /// <summary>
        /// 移动文件操作
        /// </summary>
        /// <param name="oldpath"></param>
        /// <param name="newpath"></param>
        public void MoveFile(string oldpath, string newpath)
        {
            string oldProjectPath = newpath.Substring(0, oldpath.LastIndexOf("\\"));
            string oldFilename = oldpath.Substring(oldpath.LastIndexOf("\\") + 1);
            var oldProjectid = this.m_projectHelper.GetProjectIdByNamePath(oldProjectPath);

            string newProjectPath = newpath.Substring(0, newpath.LastIndexOf("\\"));
            string newFilename = newpath.Substring(newpath.LastIndexOf("\\") + 1);
            var newProjectid = this.m_projectHelper.GetProjectIdByNamePath(newProjectPath);
            if (newProjectid == -1)
            {
                // 新目录不存在
                throw new UnauthorizedAccessException("target directory not exists");
            }
            var pw_doc = this.GetDocumentByNamePath(oldpath);
            if (pw_doc == null)
            {
                // 源文件不存在
                // TODO,要不要特殊处理
                return;
            }

            if(pw_doc.locked && pw_doc.locked_by_me)
            {
                // 占用的文件无法移动
                this.Free(pw_doc.id);
            }
            lock (this._lock)
            {             
                if(oldProjectid == newProjectid){
                    if (newFilename != oldFilename)
                    {
                        // 还是在源目录，只是重命名的情况
                        this._Update(pw_doc.id, newFilename);
                    }
                }
                
                if (oldProjectid != newProjectid)
                {
                    // 更改了上级目录
                    string new_name = null;
                    if (newFilename != oldFilename)
                    {
                        // 移动文件夹同时也改名了
                        new_name = newFilename;
                    }
                    this._MoveDocument(pw_doc,newProjectid, new_name);
                }

                // 清空缓存，防止移动后老的还在
                var cache_key = $"ReadDocumentsByParentProjectId:{oldProjectid}";
                // TODO,确认移动后原始路径的缓存清空
                this.m_cache.Delete(cache_key);
                // 以及清空在目的地的缓存，防止前面判断是否存在时获取过
                cache_key = $"ReadDocumentsByParentProjectId:{newProjectid}";
                this.m_cache.Delete(cache_key);
            }
        }
    }
}
