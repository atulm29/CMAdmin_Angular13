using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMAdmin.API.Models
{
    public class Organization
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int CollegeId { get; set; }
        public string ShowNewUserTab { get; set; }
        public string ShowAssessmentsTab { get; set; }
        public int MasterCourseId { get; set; }
        public string AssessmentTabName { get; set; }
        public string OrganizationType { get; set; }
        public int OrganizationId { get; set; }
    }

    public class CreateOrganization
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string OrganizationType { get; set; }
    }
}
