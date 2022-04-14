using CMAdmin.API.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.Extensions.Logging;
using CMAdmin.API.Helpers;
using CMAdmin.API.Data;
using System.Collections;

namespace CMAdmin.API.Repositories
{
    public interface ICollegeMasterRepository
    {
        string GetGroupIdFromCollegeId(string CollegeId);
        DataTable GetCollegeSubscriptionsList(string CollegeId = "", string GroupId = "", int PageIndex = 0, int PageSize = 10, string FromDate = "", string ToDate = "", bool ShowIsDefault = false, string AdminUserType = "", string AdminUserId = "");
        DataTable GetCollegeListConfigurationForCollege(string CollegeId = "0");
        Task<List<InstituteRole>> GetDistinctCollegeListByAddressCity(string Address, string City, string GroupId = "", string CollegeId = "");
        DataTable GetAllColleges(string CollegeID, string GroupId = "");
    }
    public class CollegeMasterRepository : ICollegeMasterRepository
    {
        private readonly IConfiguration _config;
        private readonly ILoggerManager _logger;
        public CollegeMasterRepository(IConfiguration config, ILoggerManager logger)
        {
            _config = config;
            _logger = logger;
        }
        public IDbConnection connection
        {
            get { return new SqlConnection(Config.MCQBConnectionString); }
        }

        public string GetGroupIdFromCollegeId(string CollegeId)
        {
            try
            {
                String logParams = "CollegeId:" + CollegeId;
                _logger.LogInfo("[CollegeMasterRepository]|[GetGroupIdFromCollegeId]|logParams: " + logParams);
                using (IDbConnection con = connection)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(" SELECT ISNULL(CAST(GroupId AS VARCHAR(50)),'') GroupId FROM MCQ_CollegeMaster WHERE CollegeId=" + CollegeId + " ");

                    con.Open();
                    var result = con.QueryFirst<string>(sb.ToString());
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return null;
            }
        }

        public DataTable GetCollegeSubscriptionsList(string CollegeId = "", string GroupId = "", int PageIndex = 0, int PageSize = 10, string FromDate = "", string ToDate = "", bool ShowIsDefault = false, string AdminUserType = "", string AdminUserId = "")
        {
            DBAccess oDBAccess = null;
            DataTable oDataTable = new DataTable();
            try
            {
                bool isGroup = false;
                oDBAccess = new DBAccess();
                StringBuilder sb = new StringBuilder();

                ArrayList oParameters = new ArrayList();
                if (!string.IsNullOrEmpty(GroupId) && GroupId != "0")
                {
                    isGroup = true;
                    oParameters.Add(new SqlParameter() { ParameterName = "@GroupId", SqlDbType = SqlDbType.Int, Value = GroupId });
                }
                if (!string.IsNullOrEmpty(CollegeId) && CollegeId != "0" && !isGroup)
                    oParameters.Add(new SqlParameter() { ParameterName = "@CollegeId", SqlDbType = SqlDbType.Int, Value = CollegeId });

                oParameters.Add(new SqlParameter() { ParameterName = "@PageSize", SqlDbType = SqlDbType.Int, Value = PageSize });


                if (!string.IsNullOrEmpty(FromDate)) oParameters.Add(new SqlParameter() { ParameterName = "@StartDate", Value = FromDate });
                if (!string.IsNullOrEmpty(ToDate)) oParameters.Add(new SqlParameter() { ParameterName = "@EndDate", Value = ToDate });
                if (PageIndex > 0) oParameters.Add(new SqlParameter() { ParameterName = "@PageIndex", SqlDbType = SqlDbType.Int, Value = PageIndex });
                oParameters.Add(new SqlParameter() { ParameterName = "@ShowIsDefault", Value = ShowIsDefault });
                if (!string.IsNullOrEmpty(AdminUserType))
                    oParameters.Add(new SqlParameter() { ParameterName = "@AdminUserType", Value = AdminUserType });
                if (!string.IsNullOrEmpty(AdminUserId))
                    oParameters.Add(new SqlParameter() { ParameterName = "@AdminUserId", Value = AdminUserId });
                sb.Append("SP_MCQ_GetCollegeSubscriptionsList");
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

        public DataTable GetCollegeListConfigurationForCollege(string CollegeId = "0")
        {
            DBAccess oDBAccess = null;
            DataTable oDataTable = new DataTable();
            try
            {
                oDBAccess = new DBAccess();
                if (string.IsNullOrEmpty(CollegeId))
                    CollegeId = "0";
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(" IF EXISTS(SELECT TOP(1) * FROM MCQ_CollegeListConfiguration WHERE CollegeId=" + CollegeId + ")");
                sb.AppendLine(" BEGIN ");
                sb.AppendLine(" SELECT * FROM MCQ_CollegeListConfiguration WHERE Active='Y' AND CollegeId = " + CollegeId + " ORDER BY SequenceNo ");
                sb.AppendLine(" END ");
                sb.AppendLine(" ELSE ");
                sb.AppendLine(" BEGIN ");
                sb.AppendLine(" SELECT * FROM MCQ_CollegeListConfiguration WHERE Active='Y' AND CollegeId = 0 ORDER BY SequenceNo ");
                sb.AppendLine(" END ");

                oDataTable = oDBAccess.lfnGetDataTable(sb.ToString());
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

        public async Task<List<InstituteRole>> GetDistinctCollegeListByAddressCity(string Address, string City, string GroupId = "", string CollegeId = "")
        {
            try
            {
                using (IDbConnection con = connection)
                {
                    StringBuilder sb = new StringBuilder();
                    bool isGroup = false;
                    sb.AppendLine(" SELECT CollegeId,CollegeName FROM MCQ_CollegeMaster WHERE  1=1  ");
                    if (!string.IsNullOrEmpty(Address))
                        sb.AppendLine(" AND CollegeAddress='" + Address + "' ");
                    if (!string.IsNullOrEmpty(City))
                        sb.AppendLine(" AND City='" + City + "' ");
                    if (!string.IsNullOrEmpty(GroupId) && GroupId != "0")
                    {
                        isGroup = true;
                        sb.AppendLine(" AND GroupId=" + GroupId + " ");
                    }
                    if (!string.IsNullOrEmpty(CollegeId) && CollegeId != "0" && !isGroup)
                        sb.AppendLine(" AND CollegeId=" + CollegeId + " ");
                    sb.AppendLine(" ORDER BY CollegeName");
                    con.Open();
                    var result = await con.QueryAsync<InstituteRole>(sb.ToString());
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                return null;
            }
        }

        public DataTable GetAllColleges(string CollegeID, string GroupId = "")
        {
            DBAccess oDBAccess = null;
            DataTable oDataTable = new DataTable();
            try
            {
                oDBAccess = new DBAccess();
                bool isGroup = false;
                string Selectstr = "SELECT *,CONVERT(VARCHAR(12),StartDate,103) Start ,CONVERT(VARCHAR(12),ExpiryDate,103) Expiry "
                                    + " ,CONVERT(VARCHAR(12),CreatedDate,103) CreatedDateShow "
                                    + " ,ISNULL((SELECT GroupName FROM MCQ_GroupMaster WHERE GroupId=MCQ_CollegeMaster.GroupId),'') GroupName "
                                    + " From  MCQ_CollegeMaster WHERE 1=1 ";
                if (!string.IsNullOrEmpty(GroupId) && GroupId != "0")
                {
                    isGroup = true;
                    Selectstr += " AND GroupId=" + GroupId;
                }
                if (Convert.ToUInt32(CollegeID) > 0 && !isGroup)
                {
                    Selectstr += " AND CollegeId=" + CollegeID;
                }
                oDataTable = oDBAccess.lfnGetDataTable(Selectstr);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                // LogWriter.WriteLog("GetAllColleges()->Error->" + ex.Message + Environment.NewLine + ex.StackTrace);
            }
            finally
            {
                if (oDBAccess != null && oDBAccess.isConnectionOpen()) oDBAccess.CloseDB();
            }
            return oDataTable;
        }

    }
}
