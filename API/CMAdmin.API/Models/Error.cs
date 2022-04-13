using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMAdmin.API.Models
{
    public class Error
    {
        public Status Status { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }

        public Error() { }

        public Error(Status status, string message, string ErrorCode)
        {
            this.Status = status;
            this.Message = message;
            this.ErrorCode = ErrorCode;
        }
        public static Error GetError(Exception ex)
        {
            //LogWriter.WriteLog("Error->" + ex.Message + Environment.NewLine + ex.StackTrace);
            Error oError = new Error();
            oError.Message = ex.Message;
            oError.Status = Status.Error;
            return oError;
        }
        public static Error GetError(string MethodName, Exception ex)
        {
           // LogWriter.WriteLog(MethodName + "()->Error->" + ex.Message + Environment.NewLine + ex.StackTrace);
            Error oError = new Error();
            oError.Message = ex.Message;
            oError.Status = Status.Error;
            return oError;
        }
        public static Error GetSuccess()
        {
            //LogWriter.WriteLog("Sucess");
            Error oError = new Error();
            oError.Status = Status.Success;
            return oError;
        }

        public static Error SuccessWarning(string ErrorMessage)
        {
            //LogWriter.WriteLog("SucessWarning");
            Error oError = new Error();
            oError.Status = Status.SuccessWarning;
            oError.Message = ErrorMessage;
            return oError;
        }
    }
    public enum Status
    {
        Success,
        Error,
        Warning,
        SuccessWarning
    }
}
