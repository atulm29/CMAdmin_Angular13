using CMAdmin.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CMAdmin.API.Helpers
{
    public class TokenHelper
    {
        public static AdminUserMaster GetTokenCliamData(HttpContext httpContext)
        {
            AdminUserMaster _objAdminUserMaster = new AdminUserMaster();
             var useFullName = httpContext.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            _objAdminUserMaster.AdminUserId = string.IsNullOrEmpty(httpContext.User?.Claims?.FirstOrDefault(c => c.Type == "AdminUserId")?.Value)? 0 : Convert.ToInt32(httpContext.User?.Claims?.FirstOrDefault(c => c.Type == "AdminUserId")?.Value);
            _objAdminUserMaster.UserType = string.IsNullOrEmpty(httpContext.User?.Claims?.FirstOrDefault(c => c.Type == "UserType")?.Value) ? string.Empty : Convert.ToString(httpContext.User?.Claims?.FirstOrDefault(c => c.Type == "UserType")?.Value);
            _objAdminUserMaster.CollegeId = string.IsNullOrEmpty(httpContext.User?.Claims?.FirstOrDefault(c => c.Type == "CollegeId")?.Value) ? string.Empty : Convert.ToString(httpContext.User?.Claims?.FirstOrDefault(c => c.Type == "CollegeId")?.Value);
            _objAdminUserMaster.RoleId = string.IsNullOrEmpty(httpContext.User?.Claims?.FirstOrDefault(c => c.Type == "RoleId")?.Value) ? string.Empty : Convert.ToString(httpContext.User?.Claims?.FirstOrDefault(c => c.Type == "RoleId")?.Value);
            return _objAdminUserMaster;
        }
    }
}
