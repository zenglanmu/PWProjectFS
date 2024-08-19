using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PWProjectFS.PWApiWrapper;

namespace PWProjectFS.PWProvider
{
    public class PWDocument
    {
        public string id { get; set; }
        public string projectId { get; set; }
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
    }
    public class DocumentHelper
    {
        private object _lock = null;
        private PWResourceCache m_cache;
        public DocumentHelper(object _lock, PWResourceCache cache)
        {
            this._lock = _lock;
            this.m_cache = cache;
        }

        /// <summary>
        /// 从AADMSBUFFER_DOCUMENT读取文档
        /// </summary>
        /// <param name="i">在buffer里的序号</param>
        /// <returns></returns>
        private PWDocument PopulateDocumentFromBuffer(int i, int timezone)
        {
            var doc = new PWDocument();
            doc.id = dmscli.aaApi_GetDocumentGuidProperty(dmscli.DocumentProperty.DocGuid, i).ToString();
            doc.projectId = dmscli.aaApi_GetDocumentGuidProperty(dmscli.DocumentProperty.ProjGuid, i).ToString();
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
            int timeZoneMinutes = Util.GetTimeZoneMinutes();
            var doc = this.PopulateDocumentFromBuffer(0, timeZoneMinutes);
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
            int timeZoneMinutes = Util.GetTimeZoneMinutes();
            var docs = new List<PWDocument>();
            for(int i = 0; i < docnums; i++)
            {
                var doc = this.PopulateDocumentFromBuffer(i, timeZoneMinutes);
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
                this._Delete(documentId);
            }
        }
    }
}
