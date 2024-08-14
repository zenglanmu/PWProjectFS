using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PWProjectFS.PWApiWrapper
{
    public class PWException : Exception
    {
        public PWException(string message) : base(message) { }

        public override string Message
        {
            get
            {
                return base.Message;
            }
        }

        public static PWException GetPWLastException()
        {
            var errorId = dmsgen.aaApi_GetLastErrorId();
            var error = dmsgen.aaApi_GetLastErrorDetail();
            return new PWException(string.Format("pw错误信息:{0},错误码:{1}", error, errorId));
        }
    }
}
