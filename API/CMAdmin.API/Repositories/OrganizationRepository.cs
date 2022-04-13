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
    public interface IOrganizationRepository
    {
        Task<List<Organization>> GetAll();
        Task<Organization> GetById(int id);
        Task<Organization> DeleteById(int id);
        int IsCollegeGroupUserAlreadyExists(string GroupName, string GroupId = "");
        int AddCollegeGroup(string GroupName, string CollegeId, string OrganizationType);
        DataTable GetGroupSubscriptionsList(string GroupId = "", int PageIndex = 0, int PageSize = 10);        
    }
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly IConfiguration _config;
        private readonly ILoggerManager _logger;
        public OrganizationRepository(IConfiguration config, ILoggerManager logger)
        {
            _config = config;
            _logger = logger;
        }
        public IDbConnection connection
        {
            get { return new SqlConnection(Config.MCQBConnectionString); }
        }
        public async Task<List<Organization>> GetAll()
        {
            try
            {
                using (IDbConnection con = connection)
                {
                    string Query = "SELECT * FROM MCQ_GroupMaster ORDER BY GroupName";
                    con.Open();
                    var result = await con.QueryAsync<Organization>(Query);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return null;
            }
        }
        public async Task<Organization> GetById(int id)
        {
            try
            {
                String logParams = "GroupId:" + id;
                _logger.LogInfo("[OrganizationRepository]|[GetById]|logParams: " + logParams);
                using (IDbConnection con = connection)
                {
                    string sQuery = "SELECT * FROM MCQ_GroupMaster ";
                    sQuery += " WHERE GroupId=" + id + " ORDER BY GroupName";
                    con.Open();
                    var result = await con.QueryAsync<Organization>(sQuery);
                    return result.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return null;
            }
        }
        public Task<Organization> DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public int IsCollegeGroupUserAlreadyExists(string GroupName, string GroupId = "")
        {
            int ExistingGroupId = 0;
            DBAccess oDBAccess = null;
            try
            {
                oDBAccess = new DBAccess();
                GroupName = GroupName.Replace("'", "''");
                String sqlStr = "SELECT CAST(GroupId AS INT) GroupId FROM MCQ_GroupMaster WHERE GroupName='" + GroupName + "' ";
                if (!string.IsNullOrEmpty(GroupId.Trim()))
                    sqlStr += " AND GroupId !=" + GroupId + " ";

                ExistingGroupId = oDBAccess.lfnExecuteScaler<int>(sqlStr);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                //LogWriter.WriteLog("IsCollegeGroupUserAlreadyExists(" + GroupName + ")->Error->" + ex.Message + Environment.NewLine + ex.StackTrace);
            }
            finally
            {
                if (oDBAccess != null && oDBAccess.isConnectionOpen()) oDBAccess.CloseDB();
            }
            return ExistingGroupId;
        }
        public int AddCollegeGroup(string GroupName, string CollegeId, string OrganizationType)
        {
            DBAccess oDBAccess = null;
            int GroupId = 0;
            try
            {
                oDBAccess = new DBAccess();

                StringBuilder lsSQL = new StringBuilder();

                //Add Professor
                lsSQL = new StringBuilder();

                lsSQL.Append("INSERT INTO MCQ_GroupMaster (GroupName,CollegeId,OrganizationType) VALUES('" + GroupName + "'," + CollegeId + ",'" + OrganizationType + "'); Select @@Identity; Select @@Identity;");

                object oGroupId = new object();
                oDBAccess.lfnUpdateData(lsSQL.ToString(), out oGroupId);

                GroupId = Convert.ToInt32(oGroupId.ToString());
                if (GroupId > 0)
                {
                    lsSQL = new StringBuilder();
                    lsSQL.Append(" UPDATE MCQ_CollegeMaster SET GroupId=" + GroupId + " WHERE CollegeId=" + CollegeId + "");
                    oDBAccess.lfnUpdateData(lsSQL.ToString());
                }
                oDBAccess.CommitTransaction();
            }
            catch (Exception ex)
            {
                //LogWriter.WriteLog("AddCollegeGroup()->Error->" + ex.Message + Environment.NewLine + ex.StackTrace);
                _logger.LogException(ex);
            }
            finally
            {
                if (oDBAccess != null && oDBAccess.isConnectionOpen()) oDBAccess.CloseDB();
            }
            return GroupId;
        }

        public DataTable GetGroupSubscriptionsList(string GroupId = "", int PageIndex = 0, int PageSize = 10)
        {
            DBAccess oDBAccess = null;
            DataTable oDataTable = new DataTable();
            try
            {
                oDBAccess = new DBAccess();
                StringBuilder sb = new StringBuilder();

                ArrayList oParameters = new ArrayList();
                if (!string.IsNullOrEmpty(GroupId) && GroupId != "0")
                {
                    oParameters.Add(new SqlParameter() { ParameterName = "@GroupId", SqlDbType = SqlDbType.Int, Value = GroupId });
                }

                oParameters.Add(new SqlParameter() { ParameterName = "@PageSize", SqlDbType = SqlDbType.Int, Value = PageSize });
                if (PageIndex > 0)
                    oParameters.Add(new SqlParameter() { ParameterName = "@PageIndex", SqlDbType = SqlDbType.Int, Value = PageIndex });
                else
                    oParameters.Add(new SqlParameter() { ParameterName = "@PageIndex", Value = DBNull.Value });

                sb.Append("SP_MCQ_GetGroupSubscriptionsList");
                oDataTable = oDBAccess.lfnGetDataTableProcedure(sb.ToString(), oParameters);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);               
            }
            finally
            {
                if (oDBAccess != null && oDBAccess.isConnectionOpen()) oDBAccess.CloseDB();
            }
            return oDataTable;
        }        

    }
}
