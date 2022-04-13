using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMAdmin.API.Models
{
    public class OrganizationResp
    {
        public string SrNo { get; set; }
        public string GroupConfigLabel { get; set; }
        public string InstituteLabelConfig { get; set; }
        public string InstructorLabelConfig { get; set; }
        public string SubScribeStudentLabelConfig { get; set; }
        public string TrialStudentLabelConfig { get; set; }
        public List<OrganizationRowName> RowResults { get; set; }
        public int TotalRecords { get; set; }
    }

    public class OrganizationRowName
    {
        public string RowNum { get; set; }
        public string GroupName { get; set; }
        public string TotalInstitute { get; set; }
        public string InstructorCount { get; set; }
        public string PaidStudentCount { get; set; }
        public string TrialStudentCount { get; set; }
    }
}
