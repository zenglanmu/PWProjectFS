﻿using System;
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
        public bool locked_by_me { get; set; }
        public bool is_latest { get; set; }

        public DocumentType documentType { get; set; }

        public FileInformation toFileInformation()
        {
            var file = new FileInformation
            {
                Attributes = FileAttributes.Archive,
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
                return this._Read(documentId);
            }
        }

        private List<PWDocument> _ReadByParent(int parentProjectId)
        {
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

            var docs = new List<PWDocument>();
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
                return this._ReadByParent(parentProjectId);
            }
        }

        private void _Delete(string documentId)
        {
            var documentGuid = SelectDocument(documentId);
            if (!dmscli.aaApi_GUIDDeleteDocument(dmscli.DocumentDeleteMasks.None, ref documentGuid))
            {
                throw PWException.GetPWLastException();
            }
        }

        public void Delete(string documentId)
        {
            // use lock to ensure thread safe calling pw apis
            lock (this._lock)
            {
                var doc = this.Read(documentId);
                this._Delete(documentId);
                var cache_key = $"GetDocumentByNameAndProjectId:{doc.projectId}-{doc.filename}";
                this.m_cache.Delete(cache_key);
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

        public PWDocument Touch(string docFullPath)
        {
            lock (this._lock)
            {
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
                        return this._Create(projectId, filelocalpath);
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
            // use lock to ensure thread safe calling pw apis
            lock (this._lock)
            {
                var cache_key = $"GetDocumentByNameAndProjectId:{projectId}-{filename}";
                Func<PWDocument> get_value_func = () =>
                {
                    return this._GetDocumentByNameAndProjectId(filename, projectId);
                };
                return this.m_cache.TryGet(cache_key, get_value_func);
            }
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
            string oldFilename = oldpath.Substring(oldpath.LastIndexOf("\\") + 1);
            var oldProjectid = this.m_projectHelper.GetProjectIdByNamePath(oldpath);

            string newProjectPath = newpath.Substring(0, newpath.LastIndexOf("\\"));
            string newFilename = newpath.Substring(newpath.LastIndexOf("\\") + 1);
            var newProjectid = this.m_projectHelper.GetProjectIdByNamePath(newProjectPath);
            if (newProjectid == -1)
            {
                // 新目录不存在
                throw new UnauthorizedAccessException("target directory not exists");
            }
            var pw_doc = this.GetDocumentByNamePath(oldpath);
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
                var cache_key = $"GetDocumentByNameAndProjectId:{oldProjectid}-{oldFilename}";
                this.m_cache.Delete(cache_key);
                // 以及清空在目的地的缓存，防止前面判断是否存在时获取过
                cache_key = $"GetDocumentByNameAndProjectId:{newProjectid}-{newFilename}";
                this.m_cache.Delete(cache_key);
            }
        }
    }
}
