using CMAdmin.API.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using CMAdmin.API.Helpers;
using CMAdmin.API.Data;
using System.Text;
using System.Collections;

namespace CMAdmin.API.Repositories
{
    public interface IRoleMasterRepository
    {
        Task<List<RoleMaster>> GetAllRole(int CollegeId);
        Task<List<Permissions>> GetRoleMaster(string AdminCollegeId = "0", string IsSuperAdmin = "N");
        DataTable GetUniqueRoleName(int RoleId, string RoleName, int CollegeId);
        int SaveRole(int RoleId, string roleName, int CollegeId = 0);
        bool DeleteRolePermission(int RoleId);
        int SaveRolePermissions(int RoleId, string AdminDashboardName, string PermissionName, int CollegeId);
        public EditRoles GetRoleDataWithPermission(int RoleId);
    }
    public class RoleMasterRepository: IRoleMasterRepository
    {
        private readonly IConfiguration _config;
        private readonly ILoggerManager _logger;
        public RoleMasterRepository(IConfiguration config, ILoggerManager logger)
        {
            _config = config;
            _logger = logger;
        }
        public IDbConnection connection
        {
            get { return new SqlConnection(Config.MCQBConnectionString); }
        }

        public async Task<List<RoleMaster>> GetAllRole(int CollegeId)
        {
            try
            {
                using (IDbConnection con = connection)
                {
                    string Query = "select count(au.AdminUserId) as UserCount,rm.rolename,rm.roleid from MCQ_RoleMaster rm left outer join MCQ_AdminUserMaster au on rm.RoleId = au.RoleId where rm.CollegeId=" + CollegeId + " group by rm.RoleName,rm.roleid";
                    con.Open();
                    var result = await con.QueryAsync<RoleMaster>(Query);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return null;
            }
        }
        public async Task<List<Permissions>> GetRoleMaster(string AdminCollegeId = "0", string IsSuperAdmin = "N")
        {
            try
            {
                using (IDbConnection con = connection)
                {
                  
                    string Query = "";
                    if ((AdminCollegeId == "0" || AdminCollegeId == "") && IsSuperAdmin == "Y")
                    {
                        Query = "SELECT AdminDashboardId,Name,Alias FROM MCQ_AdminDashboard WHERE CollegeId = 0 AND Active='Y'  ";                        
                    }
                    else
                    {
                        Query = "SELECT AdminDashboardId,Name,Alias FROM MCQ_AdminDashboard WHERE CollegeId = 0 AND Active='Y' AND Name NOT IN ('GroupManagement','InstituteManagement','Notifications','HRMS','ManageLicenses','ManageEcommerce','CustomStudentManagement')";                    
                    }
                    con.Open();
                    var result = await con.QueryAsync<Permissions>(Query);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return null;
            }
        }
        public DataTable GetUniqueRoleName(int RoleId, string RoleName, int CollegeId)
        {
            DBAccess oDBAccess = null;
            DataTable dt = new DataTable();

            string str = "";
            try
            {
                oDBAccess = new DBAccess();
                str = "select RoleName from  MCQ_RoleMaster where RoleName='" + RoleName + "'";
                str += " and CollegeId=" + CollegeId;
                str += " and RoleId !=" + RoleId;
                dt = oDBAccess.lfnGetDataTable(str);
            }

            catch (Exception ex)
            {
                _logger.LogException(ex);
                //LogWriter.WriteLog("GetUniqueRoleName()->Error->" + ex.Message + Environment.NewLine + ex.StackTrace);
            }
            finally
            {
                if (oDBAccess != null && oDBAccess.isConnectionOpen()) oDBAccess.CloseDB();
            }
            return dt;
        }
        public int SaveRole(int RoleId, string roleName, int CollegeId = 0)
        {
            string query = string.Empty;
            DBAccess oDBAccess = null;
            try
            {
                oDBAccess = new DBAccess();            
                if (RoleId == 0)
                {
                    query = "INSERT INTO MCQ_RoleMaster(RoleName,CollegeId) values('" + roleName + "','" + CollegeId + "') ; select cast(@@IDENTITY as int)";
                    RoleId = oDBAccess.lfnExecuteScaler<int>(query);
                }
                else
                {
                    query = "UPDATE MCQ_RoleMaster SET RoleName='" + roleName + "' WHERE RoleId=" + RoleId;
                    oDBAccess.lfnUpdateData(query);
                }               
            }
            catch (Exception ex)
            {
                RoleId = 0;
                _logger.LogException(ex);               
            }
            finally
            {
                if (oDBAccess != null && oDBAccess.isConnectionOpen()) oDBAccess.CloseDB();
            }
            return RoleId;
        }
        public bool DeleteRolePermission(int RoleId)
        {
            string query = string.Empty;
            bool result = false;
            DBAccess oDBAccess = null;
            try
            {
                oDBAccess = new DBAccess();
                query = "Delete from MCQ_RolePermission where RoleId=" + RoleId;
                oDBAccess.lfnUpdateData(query);
                result = true;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                // LogWriter.WriteLog("SaveStudent()->Error->" + ex.Message + Environment.NewLine + ex.StackTrace);
                result = false;
            }
            finally
            {
                if (oDBAccess != null && oDBAccess.isConnectionOpen()) oDBAccess.CloseDB();
            }
            return result;
        }

        public int SaveRolePermissions(int RoleId, string AdminDashboardName, string PermissionName, int CollegeId)
        {
            string query = string.Empty;
            int RolePermissionId = 0;
            DBAccess oDBAccess = null;
            try
            {
                oDBAccess = new DBAccess();
                oDBAccess.BeginTransaction();

                StringBuilder lssql = new StringBuilder();
                ArrayList oParameters;
                //lssql.Append("INSERT INTO MCQ_RolePermission(RoleId,AdminDashBoardId,PermissionId)");
                //lssql.Append("values(@RoleId,@AdminDashboardId,@PermissionId);");
                lssql.Append("INSERT INTO MCQ_RolePermission(RoleId,AdminDashBoardName,PermissionName,CollegeId)");
                lssql.Append("values(@RoleId,@AdminDashboardName,@PermissionName,@CollegeId);");
                lssql.Append("select cast(@@IDENTITY as int)");

                oParameters = new ArrayList();
                oParameters.Add(new SqlParameter() { ParameterName = "@RoleId", Value = RoleId });
                oParameters.Add(new SqlParameter() { ParameterName = "@AdminDashboardName", Value = AdminDashboardName });
                oParameters.Add(new SqlParameter() { ParameterName = "@PermissionName", Value = PermissionName });
                oParameters.Add(new SqlParameter() { ParameterName = "@CollegeId", Value = CollegeId });
                RolePermissionId = oDBAccess.lfnExecuteScaler<int>(lssql.ToString(), oParameters);
                oDBAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                RolePermissionId = 0;
                _logger.LogException(ex);
                //  LogWriter.WriteLog("SaveRolePermissions()->Error->" + ex.Message + Environment.NewLine + ex.StackTrace);
                //throw ex;
            }
            finally
            {
                if (oDBAccess != null && oDBAccess.isConnectionOpen()) oDBAccess.CloseDB();
            }
            return RolePermissionId;

        }

        public EditRoles GetRoleDataWithPermission(int RoleId)
        {
            DataTable oDataTable = new DataTable();
            DBAccess oDBAccess = null;
            EditRoles objEditRoles = new EditRoles();
            try
            {
                oDBAccess = new DBAccess();
                string Selectstr = "SELECT MCQ_Rolemaster.RoleName,MCQ_Rolemaster.CollegeId,MCQ_GroupMaster.GroupId FROM MCQ_Rolemaster "
                          + " INNER JOIN MCQ_CollegeMaster ON MCQ_RoleMaster.CollegeId = MCQ_CollegeMaster.CollegeId"
                          + " LEFT JOIN MCQ_Groupmaster ON MCQ_CollegeMaster.GroupId = MCQ_GroupMaster.GroupId"
                          + " WHERE MCQ_RoleMaster.RoleId=" + RoleId;

                oDataTable = oDBAccess.lfnGetDataTable(Selectstr);
                if (oDataTable.Rows.Count > 0)
                {
                    DataRow odr;
                    odr = oDataTable.Rows[0];
                    objEditRoles.GroupId = Convert.ToString(odr["GroupId"]);
                    objEditRoles.CollegeId = Convert.ToString(odr["CollegeId"]);
                    objEditRoles.RoleName = Convert.ToString(odr["RoleName"]);
                    var dtAllPerm = this.GetRoleMaster(objEditRoles.CollegeId, "Y");
                    var _query = "Select CollegeId,RoleId,AdminDashboardName,PermissionName from MCQ_RolePermission where RoleId=" + RoleId;
                    DataTable dtRolePerm = oDBAccess.lfnGetDataTable(_query);
                    for (int i = 0; i < dtRolePerm.Rows.Count; i++)
                    {
                        for (int j = 0; j < dtAllPerm.Result.Count(); j++)
                        {
                            if (dtAllPerm.Result[j].Name == dtRolePerm.Rows[i]["AdminDashboardName"].ToString())
                            {
                                dtAllPerm.Result[j].IsSelected = true;
                            }
                        }
                    }
                    objEditRoles.Permissions = dtAllPerm.Result;
                }
                else { return null; }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                //LogWriter.WriteLog("GetAllRole()->Error->" + ex.Message + Environment.NewLine + ex.StackTrace);
            }

            finally
            {
                if (oDBAccess != null && oDBAccess.isConnectionOpen()) oDBAccess.CloseDB();
            }
            return objEditRoles;
        }
    }
}
