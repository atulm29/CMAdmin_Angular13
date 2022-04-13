using CMAdmin.API.Helpers;
using CMAdmin.API.Models;
using CMAdmin.API.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CMAdmin.API.Services
{
    public interface IDashboardService
    {
        List<AdminDashboard> GetAdminDashboard(int UserId, string CollegeId, string RoleId, string UserType = "");
    }
    public class DashboardService : IDashboardService
    {
        private readonly ILoggerManager _logger;
        private readonly IGeneralRepository _generalRepository;
        private readonly ICollegeMasterRepository _collegeMasterRepository;
        private readonly IAnalyticsReportsRepository _analyticsReportsRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IExamRepository _examRepository;
        private readonly ISemesterRepository _semesterRepository;
        public DashboardService(ILoggerManager logger, 
                                ICollegeMasterRepository collegeMasterRepository, 
                                IAnalyticsReportsRepository analyticsReportsRepository,
                                IGeneralRepository generalRepository,
                                ISubjectRepository subjectRepository,
                                IExamRepository examRepository,
                                ISemesterRepository semesterRepository)
        {
            _logger = logger;
            _collegeMasterRepository = collegeMasterRepository;
            _analyticsReportsRepository = analyticsReportsRepository;
            _generalRepository = generalRepository;
            _subjectRepository = subjectRepository;
            _examRepository = examRepository;
            _semesterRepository = semesterRepository;
        }

        public List<AdminDashboard> GetAdminDashboard(int UserId, string CollegeId, string RoleId, string UserType = "")
        {
           try
           {
                List<AdminDashboard> AdminDashboardList = new List<AdminDashboard>();

                string GroupId = "";

                if (Convert.ToString(UserType) == "G")
                    GroupId = _collegeMasterRepository.GetGroupIdFromCollegeId(CollegeId);

                DataTable odt = new DataTable();

            
                string IsScormCloud = _generalRepository.GetIsScormCloudCollege(CollegeId);
                string IsStudentLogin = "N";

                odt = _analyticsReportsRepository.GetAdminDashboard(CollegeId, UserType, UserId.ToString(), "", RoleId);

                DataView dvodtAll = odt.DefaultView;
                dvodtAll.RowFilter = "Name LIKE'%_count'";

                DataTable odtCounts = new DataTable();

                if (dvodtAll.ToTable().Rows.Count > 0)
                {
                    odtCounts = _analyticsReportsRepository.GetDashboardCountTilesData(CollegeId, UserId.ToString());
                }
                AdminDashboard oAdminDashboard = null;
                if (UserType.ToUpper() == "U" || UserType.ToUpper() == "M")
                {
                    DataView dvodtOther = odt.DefaultView;
                    dvodtOther.RowFilter = "Name IN ('Library')";
                    odt = dvodtOther.ToTable();
                }
                if (IsScormCloud == "1" || IsScormCloud == "2")
                {
                    DataView dvodtOther = odt.DefaultView;
                    dvodtOther.RowFilter = "Name IN ('CourseManagement','Reports','LTIServer','StudentsRegistered_Count','ActiveStudents_Count','Services')";
                    odt = dvodtOther.ToTable();
                }
                else
                {
                    DataView dvodtOther = odt.DefaultView;
                    dvodtOther.RowFilter = "Name NOT IN ('LTIServer')";
                    odt = dvodtOther.ToTable();
                }
                foreach (DataRow dr in odt.Rows)
                {
                    if (string.IsNullOrEmpty(CollegeId) && (Convert.ToString(dr["Name"]).ToLower() == "settings" || Convert.ToString(dr["Name"]).ToLower() == "collegebatches" || Convert.ToString(dr["Name"]).ToLower() == "onlinelecture" || Convert.ToString(dr["Name"]).ToLower() == "ltiserver"))
                        continue;

                    if (Convert.ToString(dr["Name"]).ToLower() == "switchtostudent" && IsStudentLogin != "Y")
                        continue;
                    //Show Clients OR Facilities OR Institutes tile only for default college admin user(13Jan2022)
                    if (!string.IsNullOrEmpty(CollegeId))
                    {
                        string tileName = Convert.ToString(dr["Name"]);

                        if (tileName.ToLower() == "institutemanagement")
                        {
                            string IsDefault = _subjectRepository.GetMagentoFlag(CollegeId);

                            //If IsDefault!=y then continue
                            if (IsDefault != null && IsDefault.ToUpper() != "Y")
                            {
                                continue;
                            }
                        }
                    }


                    oAdminDashboard = new AdminDashboard();
                    oAdminDashboard.Name = Convert.ToString(dr["Name"]);
                    oAdminDashboard.Id = Convert.ToString(dr["AdmindashboardId"]);
                    oAdminDashboard.Alias = Convert.ToString(dr["Alias"]);
                    oAdminDashboard.SequenceNo = Convert.ToString(dr["SequenceNo"]);

                    if (oAdminDashboard.Name.ToLower() == "coursemanagement")
                    {
                        if (Convert.ToString(UserType).ToUpper().Trim() == "V")
                        {
                            bool IsFlipickInstitute = false;
                            if (Config.IsFlipickInstitute == Convert.ToString(CollegeId))
                                IsFlipickInstitute = true;
                           
                            DataTable odtAll = _examRepository.GetExamByCollegeId(Convert.ToInt32(CollegeId), Convert.ToInt32(UserId), IsFlipickInstitute);
                            int ExamId = 0;
                            string SemId = "0";
                            if (odtAll.Rows.Count > 0)
                            {
                                ExamId = Convert.ToInt32(odtAll.Rows[0]["ExamId"]);

                                DataTable odtSems = new DataTable();                            
                                odtSems = _semesterRepository.GetSemesterListForInstructor(Convert.ToString(UserId), Convert.ToString(CollegeId), ExamID: ExamId.ToString());
                                if (odtSems.Rows.Count > 0)
                                {
                                    SemId = Convert.ToString(odtSems.Rows[0]["SemesterId"]);
                                }
                            }
                            oAdminDashboard.URL += "ResearchReportDashboard.aspx?SemId=" + SemId + "&id=" + ExamId.ToString();
                        }
                        else if (!string.IsNullOrEmpty(CollegeId) && CollegeId != "0" && string.IsNullOrEmpty(GroupId))
                            oAdminDashboard.URL = "ResearchReportDashboard.aspx";
                        else
                            oAdminDashboard.URL = Convert.ToString(dr["URL"]);
                    }
                    else if (oAdminDashboard.Name.ToLower() == "groupmanagement")
                    {
                        if (!string.IsNullOrEmpty(CollegeId) && CollegeId != "0" && string.IsNullOrEmpty(GroupId))
                            continue;
                        else
                            oAdminDashboard.URL = Convert.ToString(dr["URL"]);
                    }
                    else if (Convert.ToString(dr["Name"]).ToLower() == "switchtostudent")
                    {
                        string LoginName = "superadmin@flipick.com"; //Hard Code
                        string StudentId = _generalRepository.GetStudentUserIdByEmail(LoginName);
                        if (StudentId != "" && StudentId != "0")
                        {
                            string StudentPortalURL = "";
                            if (!string.IsNullOrEmpty(CollegeId))
                                StudentPortalURL = _generalRepository.GetStudentPortalURLByCollegeId(CollegeId);
                            if (!string.IsNullOrEmpty(StudentPortalURL))
                            {
                                oAdminDashboard.URL = StudentPortalURL + "bypass?RedirectionType=ADMIN&Userid=" + StudentId + "";
                            }
                            else
                            {
                                oAdminDashboard.URL = Config.StudentPortalURL + "&UserId=" + StudentId + "";
                            }
                        }
                    }
                    else if (Convert.ToString(dr["Name"]).ToLower() == "switchtoscorecard")
                    {
                        oAdminDashboard.URL = "Scorecard.aspx";
                    }
                    else
                        oAdminDashboard.URL = Convert.ToString(dr["URL"]);

                    oAdminDashboard.CssClass = Convert.ToString(dr["CssClass"]);
                    oAdminDashboard.IconClass = Convert.ToString(dr["IconClass"]);
                    oAdminDashboard.Type = "Tile";
                    if (oAdminDashboard.Name.ToLower().Contains("_graph"))
                        oAdminDashboard.Type = "Graph";
                    else if (oAdminDashboard.Name.ToLower().Contains("_count"))
                    {
                        oAdminDashboard.Type = "Count";
                        if (odtCounts.Rows.Count > 0)
                            oAdminDashboard.DashboardCount = Convert.ToString(odtCounts.Rows[0]["" + oAdminDashboard.Name + ""]);
                    }
                    else if (oAdminDashboard.Name.ToLower().Contains("_list"))
                        oAdminDashboard.Type = "List";

                    switch (oAdminDashboard.Alias.ToLower())
                    {
                        case "organizations":
                            oAdminDashboard.URL = "organizations";
                            break;
                        case "facilities":
                            oAdminDashboard.URL = "facilities";
                            break;
                        case "admin users":
                            oAdminDashboard.URL = "adminusers";
                            break;
                        case "members":
                            oAdminDashboard.URL = "members";
                            break;
                        case "lti server":
                            oAdminDashboard.URL = "ltiServer";
                            break;
                        case "scorecard":
                            oAdminDashboard.URL = "scorecard";
                            break;
                        case "license & course meta master":
                            oAdminDashboard.URL = "license & course meta master";
                            break;
                        case "events":
                            oAdminDashboard.URL = "events";
                            break;
                        case "community":
                            oAdminDashboard.URL = "community";
                            break;
                        default:
                            // code block
                            break;
                    }

                    AdminDashboardList.Add(oAdminDashboard);
                }

                    return AdminDashboardList;
            }
           catch(Exception ex)
           {
                _logger.LogException(ex);
                return null;
            }
        }
    }
}
