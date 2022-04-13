using CMAdmin.API.Helpers;
using CMAdmin.API.Models;
using CMAdmin.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;


namespace CMAdmin.API.Services
{
    public interface IOrganizationService
    {
        OrganizationResp GetOrganizationGroupList(string GroupId = "", int PageIndex = 0, int PageSize = 10);
    }
    public class OrganizationService : IOrganizationService
    {

        private readonly ILoggerManager _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGeneralRepository _generalRepository;
        private readonly ICollegeMasterRepository _collegeMasterRepository;
        private readonly IOrganizationRepository _organizationRepository;
        public OrganizationService(ILoggerManager logger,
                                    IHttpContextAccessor httpContextAccessor,
                                    IGeneralRepository generalRepository,
                                    ICollegeMasterRepository collegeMasterRepository,
                                    IOrganizationRepository organizationRepository)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _collegeMasterRepository = collegeMasterRepository;
            _generalRepository = generalRepository;
            _organizationRepository = organizationRepository;
        }

        public OrganizationResp GetOrganizationGroupList(string GroupId = "", int PageIndex = 0, int PageSize = 10)
        {
            try
            {
                OrganizationResp objOrganizationResp = new OrganizationResp();
                var objAdminUser = TokenHelper.GetTokenCliamData(_httpContextAccessor.HttpContext);
                if(PageIndex <= 0) PageIndex += 1;

                string AdminCollegeId = Convert.ToString(objAdminUser.CollegeId);
                bool isGroup = false;
                if (Convert.ToString(objAdminUser.UserType) == "G")
                {
                    isGroup = true;
                    GroupId = _collegeMasterRepository.GetGroupIdFromCollegeId(AdminCollegeId);
                }
                //if (!string.IsNullOrEmpty(AdminCollegeId.Trim()) && AdminCollegeId != "0" && !isGroup)
                //{
                //    oCollege.CollegeID = AdminCollegeId;
                //}
                //if (dvGroup.Visible && string.IsNullOrEmpty(AdminCollegeId.Trim()))
                //{
                //    if (drpGroup.SelectedIndex > 0)
                //        GroupId = drpGroup.SelectedValue.Trim();
                //}
                string CollegeID = objAdminUser.CollegeId.ToString();
                objOrganizationResp.SrNo = "Sr. No.";
                objOrganizationResp.GroupConfigLabel = _generalRepository.GetDefaultCollegeConfigAlias(CollegeID, "Group") + " Name";
                objOrganizationResp.InstituteLabelConfig = "#" + _generalRepository.GetDefaultCollegeConfigAlias(CollegeID, "College") + "(s)";
                objOrganizationResp.InstructorLabelConfig = "#" + _generalRepository.GetDefaultCollegeConfigAlias(CollegeID, "Instructor") + "(s)";
                objOrganizationResp.SubScribeStudentLabelConfig = "#Subscribed " + _generalRepository.GetDefaultCollegeConfigAlias(CollegeID, "Student");
                objOrganizationResp.TrialStudentLabelConfig = "#Trial " + _generalRepository.GetDefaultCollegeConfigAlias(CollegeID, "Student");

                DataTable odtCollege = _organizationRepository.GetGroupSubscriptionsList(GroupId, PageIndex, PageSize);
                objOrganizationResp.RowResults = new List<OrganizationRowName>();
                if (odtCollege.Rows.Count > 0)
                {
                    objOrganizationResp.TotalRecords = Convert.ToInt32(odtCollege.Rows[0]["TotalRowCount"]);
                    foreach (DataRow dr in odtCollege.Rows)
                    {
                        OrganizationRowName obj = new OrganizationRowName();
                        obj.RowNum = Convert.ToString(dr["Row"]);
                        obj.GroupName = Convert.ToString(dr["GroupName"]);
                        obj.TotalInstitute = Convert.ToString(dr["TotalInstitute"]);
                        obj.InstructorCount = Convert.ToString(dr["InstructorCount"]);
                        obj.PaidStudentCount = Convert.ToString(dr["PaidStudentCnt"]);
                        obj.TrialStudentCount = Convert.ToString(dr["TrialStudentCnt"]);
                        objOrganizationResp.RowResults.Add(obj);
                    }
                }
                else
                {
                    objOrganizationResp.TotalRecords = 0;
                }

                return objOrganizationResp;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return null;
            }
        }

    }
}
