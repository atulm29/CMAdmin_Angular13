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
using System.Text;
using System.Threading.Tasks;

namespace CMAdmin.API.Services
{
    public interface IFacilitiesService
    {
        FacilitiesResp GetFaclityList(string CollegeId = "", string UserType= "", int PageIndex = 0, int PageSize = 10);
    }
    public class FacilitiesService: IFacilitiesService
    {
        private readonly ILoggerManager _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGeneralRepository _generalRepository;
        private readonly ICollegeMasterRepository _collegeMasterRepository;
        private readonly IOrganizationRepository _organizationRepository;
        public FacilitiesService(ILoggerManager logger,
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


        public FacilitiesResp GetFaclityList(string CollegeId = "", string UserType = "", int PageIndex = 0, int PageSize = 10)
        {
            FacilitiesResp objFacilitiesResp = new FacilitiesResp();
            try
            {

                string AdminCollegeId = CollegeId; string GroupId = ""; 
                if (UserType == "G")
                {
                    GroupId = _collegeMasterRepository.GetGroupIdFromCollegeId(AdminCollegeId);
                }
                bool ShowIsDefault = false;

                DataTable odtCollege = _collegeMasterRepository.GetCollegeSubscriptionsList(CollegeId, GroupId, PageIndex, PageSize, ShowIsDefault: ShowIsDefault);
                if (odtCollege.Rows.Count <= 0)
                {
                    try
                    {
                        odtCollege = _collegeMasterRepository.GetCollegeSubscriptionsList(CollegeId, GroupId, PageIndex - 1, PageSize, ShowIsDefault: ShowIsDefault);
                        PageIndex = PageIndex - 1;
                    }
                    catch { }
                }
                if (odtCollege.Rows.Count > 0)
                {
                    objFacilitiesResp.TotalRecords = Convert.ToInt32(odtCollege.Rows[0]["TotalRowCount"]);
                    objFacilitiesResp.TableData = CreateGridView(odtCollege, "", PageIndex);

                }
                else
                {
                    objFacilitiesResp.TotalRecords = 0;
                    objFacilitiesResp.TableData = new TableData();
                }
            }
            catch(Exception ex)
            {
                _logger.LogException(ex);
            }
            return objFacilitiesResp;
        }

        private TableData CreateGridView(DataTable odtCollege, string type = "", int PageIndex = 0, int CollegeId = 0, string UserType ="")
        {
            TableData tableData = new TableData();
            tableData.TableHeader = new List<KeyValueData>();
            tableData.TableRowData = new List<KeyValueData>();
            try
            {
                DataTable odtCollegeListConfig = new DataTable();
                odtCollegeListConfig = _collegeMasterRepository.GetCollegeListConfigurationForCollege(Convert.ToString(CollegeId));

                string IsMagentoRequired = Convert.ToString(Config.IsMagentoRequired);
                string AdminCollegeId = Convert.ToString(CollegeId);
                if (odtCollegeListConfig.Rows.Count > 0)
                {
                    KeyValueData objKeyValueHeaderData = new KeyValueData();

                    objKeyValueHeaderData.Key = "SrNo";
                    objKeyValueHeaderData.Value = "Sr. No.";
                    tableData.TableHeader.Add(objKeyValueHeaderData);

                    foreach (DataRow dr in (odtCollegeListConfig.Rows))
                    {
                        objKeyValueHeaderData = new KeyValueData();
                        objKeyValueHeaderData.Key = dr["ConfigFieldName"].ToString();
                        objKeyValueHeaderData.Value = dr["ConfigAlias"].ToString();
                        tableData.TableHeader.Add(objKeyValueHeaderData);
                    }
                    if (string.IsNullOrEmpty(AdminCollegeId.Trim()) || AdminCollegeId == "0")
                    {
                        objKeyValueHeaderData = new KeyValueData();
                        objKeyValueHeaderData.Key = "Action(s)";
                        objKeyValueHeaderData.Value = "Action(s)";
                        tableData.TableHeader.Add(objKeyValueHeaderData);
                    }
                }
                bool exists = false;
                bool isGroup = false;
                if (UserType == "G" || string.IsNullOrEmpty(AdminCollegeId.Trim()) || AdminCollegeId == "0")
                {
                    isGroup = true;
                }
                foreach (DataRow dr in odtCollege.Rows)
                {
                    KeyValueData objKeyValueTableData = new KeyValueData();

                    objKeyValueTableData.Key = "SrNo";
                    objKeyValueTableData.Value = dr["Row"].ToString();
                    tableData.TableRowData.Add(objKeyValueTableData);

                    exists = odtCollegeListConfig.Select().ToList().Exists(row => row["ConfigFieldName"].ToString().ToUpper() == "ADDRESS");
                    if (exists)
                    {
                        objKeyValueTableData = new KeyValueData();
                        var result = tableData.TableHeader.Single(s => s.Key.ToUpper() == "ADDRESS");
                        objKeyValueTableData.Key = result.Key;
                        objKeyValueTableData.Value = dr["CollegeAddress"].ToString();
                        tableData.TableRowData.Add(objKeyValueTableData);
                    }
                    else
                        exists = false;

                    exists = odtCollegeListConfig.Select().ToList().Exists(row => row["ConfigFieldName"].ToString().ToUpper() == "CITY");
                    if (exists)
                    {
                        objKeyValueTableData = new KeyValueData();
                        var result = tableData.TableHeader.Single(s => s.Key.ToUpper() == "CITY");
                        objKeyValueTableData.Key = result.Key;
                        objKeyValueTableData.Value = dr["City"].ToString();
                        tableData.TableRowData.Add(objKeyValueTableData);
                    }
                    else
                        exists = false;

                    exists = odtCollegeListConfig.Select().ToList().Exists(row => row["ConfigFieldName"].ToString().ToUpper() == "COLLEGENAME");
                    if (exists)
                    {
                        if (!string.IsNullOrEmpty(AdminCollegeId.Trim()) && AdminCollegeId != "0" && !isGroup)
                        {
                            objKeyValueTableData = new KeyValueData();
                            var result = tableData.TableHeader.Single(s => s.Key.ToUpper() == "COLLEGENAME");
                            objKeyValueTableData.Key = result.Key;
                            objKeyValueTableData.Value = dr["CollegeName"].ToString();
                            tableData.TableRowData.Add(objKeyValueTableData);
                        }
                        else
                        {
                           
                            objKeyValueTableData = new KeyValueData();
                            var result = tableData.TableHeader.Single(s => s.Key.ToUpper() == "COLLEGENAME");
                            objKeyValueTableData.Key = result.Key;
                            objKeyValueTableData.Value = "<a onclick= 'GoToCollege(" + dr["CollegeId"].ToString() + ")' >" + dr["CollegeName"].ToString() + "</a>";
                            // sb.Append("<td><a onclick= 'GoToCollege(" + dr["CollegeId"].ToString() + ")' >" + dr["CollegeName"].ToString() + "</a></td>");                        
                            tableData.TableRowData.Add(objKeyValueTableData);
                        }
                    }
                    else
                        exists = false;

                    exists = odtCollegeListConfig.Select().ToList().Exists(row => row["ConfigFieldName"].ToString().ToUpper() == "BATCHES");
                    if (exists)
                    {
                        objKeyValueTableData = new KeyValueData();
                        var result = tableData.TableHeader.Single(s => s.Key.ToUpper() == "BATCHES");
                        objKeyValueTableData.Key = result.Key;
                        objKeyValueTableData.Value = dr["Batches"].ToString();
                        tableData.TableRowData.Add(objKeyValueTableData);
                    }
                    else
                        exists = false;

                    exists = odtCollegeListConfig.Select().ToList().Exists(row => row["ConfigFieldName"].ToString().ToUpper() == "INSTRUCTORS");
                    if (exists)
                    {
                        objKeyValueTableData = new KeyValueData();
                        var result = tableData.TableHeader.Single(s => s.Key.ToUpper() == "INSTRUCTORS");
                        objKeyValueTableData.Key = result.Key;
                        objKeyValueTableData.Value = dr["InstructorCount"].ToString();
                        tableData.TableRowData.Add(objKeyValueTableData);
                    }
                    else
                        exists = false;

                    exists = odtCollegeListConfig.Select().ToList().Exists(row => row["ConfigFieldName"].ToString().ToUpper() == "PAIDSTUDENTS");
                    if (exists)
                    {
                        objKeyValueTableData = new KeyValueData();
                        var result = tableData.TableHeader.Single(s => s.Key.ToUpper() == "PAIDSTUDENTS");
                        objKeyValueTableData.Key = result.Key;
                        objKeyValueTableData.Value = dr["PaidStudentCount"].ToString();
                        tableData.TableRowData.Add(objKeyValueTableData);
                    }
                    else
                        exists = false;

                    exists = odtCollegeListConfig.Select().ToList().Exists(row => row["ConfigFieldName"].ToString().ToUpper() == "TRIALSTUDENTS");
                    if (exists)
                    {
                        objKeyValueTableData = new KeyValueData();
                        var result = tableData.TableHeader.Single(s => s.Key.ToUpper() == "TRIALSTUDENTS");
                        objKeyValueTableData.Key = result.Key;
                        objKeyValueTableData.Value = dr["TrialStudentCount"].ToString();
                        tableData.TableRowData.Add(objKeyValueTableData);
                    }
                    else
                        exists = false;
                    if (string.IsNullOrEmpty(AdminCollegeId.Trim()) || AdminCollegeId == "0")
                    {
                        objKeyValueTableData = new KeyValueData();
                        objKeyValueTableData.Key = "Action(s)";

                        StringBuilder sb = new StringBuilder();

                        sb.Append(" <a class='instituteEdit'  href='InstituteRegistration.aspx?Id=" + dr["CollegeId"].ToString() + "' alt='Edit' title='Edit'>Edit</a>");
                        if (Convert.ToString(dr["IsDefault"]).Trim().ToUpper() == "N")
                        {
                            sb.Append(" <a class='instituteAssignCourses' style='display:none;' href='AdminCourseList.aspx?CollegeId=" + dr["CollegeId"].ToString() + "'  alt='Assign Courses' title='Assign Courses'>Assign Courses</a>");
                            sb.Append(" <a class='instituteAssignCourses' runat='server' onclick='showLicencePopup(" + dr["CollegeId"].ToString() + "," + dr["GroupId"].ToString() + ")' alt='License' title='License'>License</a>");
                            sb.Append(" <a class='instituteDelete'  onclick='DeleteSelectedCollege(" + dr["CollegeId"].ToString() + ")' alt='Delete' title='Delete'>Delete</a>");
                        }
                        objKeyValueTableData.Value = sb.ToString();
                        tableData.TableRowData.Add(objKeyValueTableData);
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogException(ex);
            }
            return tableData;
        }
    }
}
