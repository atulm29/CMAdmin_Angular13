using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMAdmin.API.Models
{
    public class AdminUserMaster
    {
        public int AdminUserId { get; set; }
        public string LoginName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CollegeId { get; set; }
        public string UserType { get; set; }
        public string RoleId { get; set; }
    }

    public class Users
    {
        public int AdminUserId { get; set; }
        public string LoginName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CollegeId { get; set; }
        public string UserType { get; set; }
        public string RoleId { get; set; }
        public string DepartmentName { get; set; }
        public string DeptSectionName { get; set; }
        public string RoleName { get; set; }
    }
}
