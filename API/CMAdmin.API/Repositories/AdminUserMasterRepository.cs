using CMAdmin.API.Helpers;
using CMAdmin.API.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
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
    }
    public class AdminUserMasterRepository : IAdminUserMasterRepository
    {
        private readonly IConfiguration _config;
        private readonly ILoggerManager _logger;
        public AdminUserMasterRepository(ILoggerManager logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
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
    }
}
