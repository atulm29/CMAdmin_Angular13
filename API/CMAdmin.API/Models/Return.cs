using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMAdmin.API.Models
{
    public class Return
    {
        public class StatusCodes
        {
            public static int Success = 0;
            public static int Fail = -1;
            public static int Exception = -2;            
            public static int SPException = -3; 
            public static int SPFail = -4;
        }

        public class Messages
        {
            public const String Success = "Success";
            public const String Fail = "Fail";
            public const String Exception = "Exception";
            public const String NotFound = "Resource not found.";
            public const String FailLogin = "Please enter valid credentials.";
            public const String FailForgotPassword = "Please enter valid email.";           
        }
    }
}
