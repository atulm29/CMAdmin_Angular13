using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMAdmin.API.Models
{
    public class RoleMaster
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public int UserCount { get; set; }
    }

    public class Permissions
    {
        public int AdminDashBoardId { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public bool IsSelected { get; set; }
    }

    public class CreateRoles
    {
        public string CollegeId { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<Permissions> Permissions { get; set; }
    }

    public class EditRoles
    {
        public string GroupId { get; set; }
        public string CollegeId { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<Permissions> Permissions { get; set; }
    }
}
