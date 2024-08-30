using System;

namespace PWProjectFS.PWApiWrapper
{
    public class PWException : Exception
    {
        public PWException(string message) : base(message) { }
        public PWException(int errorId, string errorDetail) :
            this(string.Format("pw错误信息:{0},错误码:{1}", errorDetail, errorId))
        {
            this.PWErrorDetail = errorDetail;
            this.PWErrorId = errorId;

        }

        public string PWErrorDetail { get; private set; }
        public int PWErrorId { get; private set; }

        public override string Message
        {
            get
            {
                return base.Message;
            }
        }

        public static PWException GetPWLastException()
        {
            int errorId = dmsgen.aaApi_GetLastErrorId();
            string error = dmsgen.aaApi_GetLastErrorDetail();
            return new PWException(errorId, error);
        }
    }
}
