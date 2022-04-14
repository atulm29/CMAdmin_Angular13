using CMAdmin.API.Data;
using CMAdmin.API.Helpers;
using CMAdmin.API.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CMAdmin.API.Repositories
{
    public interface IAdminUserMasterRepository
    {
        Task<List<AdminUserMaster>> GetAll();
        Task<AdminUserMaster> GetById(int id);
        AdminUserMaster IsValidUserCredentials(string username, string password);
        Task<List<Users>> GetCollegeInstructors(string CollegeId, string UserType = "", string AdminUserID = "");
        int IsAdminUserAlreadyExists(string LoginName, string UserId = "");
        string AddInstructor(string FirstName, string LastName, string Loginname, string Password, string CollegeId, string UserType, string RoleId, string IsStudentLogin = "N", string TrainerType = "", string InstructorExpirationDays = "", string AllowStudentRegistration = "1", string DeptSectionId = "");
        string UpdateInstructor(string FirstName, string LastName, string Loginname, string Password, string CollegeId, string UserType, string ExistingUserId, string TrainerType = "", string ExpDate = "", bool UpdateStudents = false, string AllowStudentRegistration = "1", string DeptSectionId = "", string RoleId = "", string IsStudentLogin = "N");
        Users GetProfessorDetailsByProfessorId(int ProfessorId);
    }
    public class AdminUserMasterRepository : IAdminUserMasterRepository
    {
        private readonly IConfiguration _config;
        private readonly ILoggerManager _logger;
        private readonly ICollegeMasterRepository _collegeMasterRepository;
        public AdminUserMasterRepository(ILoggerManager logger, IConfiguration config, ICollegeMasterRepository collegeMasterRepository)
        {
            _logger = logger;
            _config = config;
            _collegeMasterRepository = collegeMasterRepository;
        }
        public IDbConnection connection
        {
            get { return new SqlConnection(Config.MCQBConnectionString); }
        }
        public AdminUserMaster IsValidUserCredentials(string username, string password)
        {
            try
            {
                using (IDbConnection con = connection)
                {
                    string Query = "SP_MCQ_InstructorLogin";
                    con.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@LoginName", username);
                    param.Add("@Password", password);
                    var result = con.QueryFirst<AdminUserMaster>(Query, param, commandType: CommandType.StoredProcedure);
                    // return null if user not found
                    if (result == null) return null;
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return null;
            }
        }
        public async Task<List<AdminUserMaster>> GetAll()
        {
            try
            {
                using (IDbConnection con = connection)
                {
                    string Query = "SELECT * FROM MCQ_AdminUserMaster";
                    con.Open();
                    var result = await con.QueryAsync<AdminUserMaster>(Query);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return null;
            }
        }
        public async Task<AdminUserMaster> GetById(int id)
        {
            try
            {
                String logParams = "AdminUserId:" + id;
                _logger.LogInfo("[AdminUserMasterRepository]|[GetById]|logParams: " + logParams);
                using (IDbConnection con = connection)
                {
                    string sQuery = "SELECT * FROM MCQ_AdminUserMaster ";
                    sQuery += " WHERE AdminUserId=" + id;
                    con.Open();
                    var result = await con.QueryAsync<AdminUserMaster>(sQuery);
                    return result.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return null;
            }
        }
        public async Task<List<Users>> GetCollegeInstructors(string CollegeId, string UserType = "", string AdminUserID = "")
        {
            try
            {
                using (IDbConnection con = connection)
                {
                    string Query = "SELECT *   ";
                    Query += " ,(SELECT STUFF((SELECT distinct ',' + CAST(DepartmentName AS VARCHAR(50)) FROM MCQ_Departmentmaster WHERE MCQ_DepartmentMaster.DepartmentId IN(SELECT DepartmentId FROM MCQ_InstructorDeptSection WHERE InstructorId = MCQ_AdminUserMaster.AdminUserId) FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(1000)') ,1,1,'') ) DepartmentName ";
                    Query += " ,(SELECT STUFF((SELECT distinct ',' + CAST(DeptSectionName AS VARCHAR(50)) FROM MCQ_DeptSectionMaster WHERE MCQ_DeptSectionMaster.DeptSectionId  IN(SELECT DeptSectionId FROM MCQ_InstructorDeptSection WHERE InstructorId = MCQ_AdminUserMaster.AdminUserId) FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(1000)') ,1,1,'') ) DeptSectionName ";
                    Query += " ,ISNULL((SELECT TOP(1) RoleName FROM MCQ_RoleMaster WHERE ISNULL(RoleId,0)=MCQ_AdminUserMaster.RoleId),'') RoleName ";
                    Query += " From  MCQ_AdminUserMaster WHERE ColleGeId=" + CollegeId + " order by MCQ_AdminUserMaster.AdminUserId desc ";

                    con.Open();
                    var result = await con.QueryAsync<Users>(Query);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return null;
            }
        }
        public int IsAdminUserAlreadyExists(string LoginName, string UserId = "")
        {
            int AdminUserId = 0;
            DBAccess oDBAccess = null;
            try
            {
                oDBAccess = new DBAccess();
                DataTable oDataTable = new DataTable();
                String sqlStr = "SELECT * FROM MCQ_AdminUserMaster WHERE LoginName='" + LoginName + "' ";
                if (!string.IsNullOrEmpty(UserId))
                {
                    sqlStr += " And  AdminUserId !=" + UserId;
                }


                oDataTable = oDBAccess.lfnGetDataTable(sqlStr);
                if (oDataTable.Rows.Count == 0)
                    AdminUserId = 0;
                else
                    AdminUserId = Convert.ToInt16(oDataTable.Rows[0]["AdminUserId"]);

            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                //LogWriter.WriteLog("IsAdminUserAlreadyExists(" + LoginName + ")->Error->" + ex.Message + Environment.NewLine + ex.StackTrace);
            }
            finally
            {
                if (oDBAccess != null && oDBAccess.isConnectionOpen()) oDBAccess.CloseDB();
            }
            return AdminUserId;
        }

        public string AddInstructor(string FirstName, string LastName, string Loginname, string Password, string CollegeId, string UserType, string RoleId, string IsStudentLogin = "N", string TrainerType = "", string InstructorExpirationDays = "", string AllowStudentRegistration = "1", string DeptSectionId = "")
        {
            DBAccess oDBAccess = null;
            string UserId = "";
            try
            {
                oDBAccess = new DBAccess();
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(" Insert into MCQ_AdminUserMaster ( ");
                sb.AppendLine(" FirstName,LastName,LoginName,Password,UserType,CollegeID,IsStudentLogin,TrainerType,CreatedDate,AllowStudentRegistration ");
                if (!string.IsNullOrEmpty(InstructorExpirationDays) && InstructorExpirationDays != "0")
                    sb.AppendLine(" ,ExpirationDate ");
                sb.AppendLine(" ,RoleId ");
                sb.AppendLine(" ) ");
                sb.AppendLine(" VALUES(@FirstName,@LastName,@Loginname,@Password,@UserType,@CollegeId,@IsStudentLogin,@TrainerType,GETDATE(),@AllowStudentRegistration ");
                if (!string.IsNullOrEmpty(InstructorExpirationDays) && InstructorExpirationDays != "0")
                    sb.AppendLine(" ,DATEADD(dd,CAST(@InstructorExpirationDays AS INT),GETDATE()) ");
                sb.AppendLine(" ,@RoleId");
                sb.AppendLine(" ) ");
                sb.AppendLine(" SELECT cast(@@Identity as VARCHAR(50)); ");

                ArrayList oParameters = new ArrayList();
                oParameters.Add(new SqlParameter() { ParameterName = "@FirstName", Value = FirstName });
                oParameters.Add(new SqlParameter() { ParameterName = "@LastName", Value = LastName });
                oParameters.Add(new SqlParameter() { ParameterName = "@Loginname", Value = Loginname });
                oParameters.Add(new SqlParameter() { ParameterName = "@Password", Value = Password });
                oParameters.Add(new SqlParameter() { ParameterName = "@UserType", Value = UserType });
                oParameters.Add(new SqlParameter() { ParameterName = "@CollegeId", Value = CollegeId });
                oParameters.Add(new SqlParameter() { ParameterName = "@IsStudentLogin", Value = IsStudentLogin });
                oParameters.Add(new SqlParameter() { ParameterName = "@TrainerType", Value = TrainerType });
                if (!string.IsNullOrEmpty(InstructorExpirationDays) && InstructorExpirationDays != "0")
                    oParameters.Add(new SqlParameter() { ParameterName = "@InstructorExpirationDays", Value = InstructorExpirationDays });
                oParameters.Add(new SqlParameter() { ParameterName = "@AllowStudentRegistration", Value = AllowStudentRegistration });
                oParameters.Add(new SqlParameter() { ParameterName = "@RoleId", Value = RoleId });
                //UserId = oDBAccess.lfnExecuteScaler<string>(InsertSQL);
                UserId = oDBAccess.lfnExecuteScaler<string>(sb.ToString(), oParameters);
                if (!string.IsNullOrEmpty(UserId) && !string.IsNullOrEmpty(DeptSectionId))
                {
                    oParameters = new ArrayList();
                    oParameters.Add(new SqlParameter() { ParameterName = "@InstructorId", Value = UserId });
                    oParameters.Add(new SqlParameter() { ParameterName = "@DeptSectionId", Value = DeptSectionId });

                    string InsDeptSection = "SP_MCQ_AddInstructorDeptSection";
                    oDBAccess.lfnGetDataTableProcedure(InsDeptSection, oParameters);
                }
                oDBAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
               // LogWriter.WriteLog("AddProfessorWithCollegeAndBranch()->Error->" + ex.Message + Environment.NewLine + ex.StackTrace);
            }
            finally
            {
                if (oDBAccess != null && oDBAccess.isConnectionOpen()) oDBAccess.CloseDB();
            }
            return UserId;
        }

        public string UpdateInstructor(string FirstName, string LastName, string Loginname, string Password, string CollegeId, string UserType, string ExistingUserId, string TrainerType = "", string ExpDate = "", bool UpdateStudents = false, string AllowStudentRegistration = "1", string DeptSectionId = "", string RoleId = "", string IsStudentLogin = "N")
        {
            DBAccess oDBAccess = null;
            string UserId = "";
            try
            {
                oDBAccess = new DBAccess();

                string UpdateStr = "UPDATE MCQ_AdminUserMaster SET "
                                + " FirstName='" + FirstName + "' "
                                + " ,Lastname='" + LastName + "' "
                                + " ,LoginName='" + Loginname + "' "
                                + " ,Password='" + Password + "'"
                                //+ " ,UserType='" + UserType + "' "
                                + " ,AllowStudentRegistration=" + AllowStudentRegistration + " "
                                + " ,TrainerType='" + TrainerType + "' "
                                + " ,RoleId='" + RoleId + "' ";

                if (UserType.ToUpper().Trim() == "V" && !string.IsNullOrEmpty(ExpDate))
                {
                    UpdateStr += " ,ExpirationDate= CONVERT(DATETIME, '" + ExpDate + "', 103) ";
                }
                if (IsStudentLogin == "Y")
                    UpdateStr += " ,IsStudentLogin='" + IsStudentLogin + "' ";

                UpdateStr += " WHERE AdminUserId=" + ExistingUserId + "; "
                              + " Select @@ROWCOUNT;";

                UserId = Convert.ToString(oDBAccess.lfnExecuteScaler<int>(UpdateStr));
                if (UpdateStudents)
                {
                    string UpdateStudentsStr = " UPDATE MCQ_UserMaster set ExpirationDate=CONVERT(DATETIME, '" + ExpDate + "', 103) WHERE CreatedBy=" + ExistingUserId + " ";
                    oDBAccess.lfnUpdateData(UpdateStudentsStr);
                }

                if (!string.IsNullOrEmpty(UserId) && !string.IsNullOrEmpty(DeptSectionId))
                {
                    ArrayList oParameters = new ArrayList();
                    oParameters.Add(new SqlParameter() { ParameterName = "@InstructorId", Value = ExistingUserId });
                    oParameters.Add(new SqlParameter() { ParameterName = "@DeptSectionId", Value = DeptSectionId });
                    string InsDeptSection = "SP_MCQ_AddInstructorDeptSection";
                    oDBAccess.lfnGetDataTableProcedure(InsDeptSection, oParameters);
                }

                oDBAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                // LogWriter.WriteLog("AddProfessorWithCollegeAndBranch()->Error->" + ex.Message + Environment.NewLine + ex.StackTrace);
            }
            finally
            {
                if (oDBAccess != null && oDBAccess.isConnectionOpen()) oDBAccess.CloseDB();
            }
            return UserId;
        }

        public Users GetProfessorDetailsByProfessorId(int ProfessorId)
        {
            Users objUser = new Users();
            DBAccess oDBAccess = null;
            DataTable odt = new DataTable();
            try
            {
                oDBAccess = new DBAccess();
                string Selectstr = "SELECT *,CONVERT(VARCHAR(12),ExpirationDate,103 ) ExpDate from MCQ_AdminUserMaster where AdminUserId =" + ProfessorId + " ";
                odt = oDBAccess.lfnGetDataTable(Selectstr);
                if (odt.Rows.Count > 0)
                {
                    objUser.LoginName = odt.Rows[0]["LoginName"].ToString();
                    objUser.Password = odt.Rows[0]["Password"].ToString();
                    objUser.FirstName = odt.Rows[0]["FirstName"].ToString();
                    objUser.LastName = odt.Rows[0]["LastName"].ToString();
                    objUser.CollegeId = odt.Rows[0]["CollegeId"].ToString();
                    objUser.UserType = odt.Rows[0]["UserType"].ToString();
                    objUser.RoleId = odt.Rows[0]["RoleId"].ToString();
                    objUser.AdminUserId = Convert.ToInt32(odt.Rows[0]["AdminUserId"].ToString());

                    DataTable odtCollege = _collegeMasterRepository.GetAllColleges(objUser.CollegeId, string.Empty);
                    if (odtCollege.Rows.Count > 0)
                    {
                        objUser.GroupId = odtCollege.Rows[0]["GroupId"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                //LogWriter.WriteLog("GetRegisteredProffessorForCollege()->Error->" + ex.Message + Environment.NewLine + ex.StackTrace);
            }
            finally
            {
                if (oDBAccess != null && oDBAccess.isConnectionOpen()) oDBAccess.CloseDB();
            }
            return objUser;
        }

    }
}
