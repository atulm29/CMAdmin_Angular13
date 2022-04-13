using CMAdmin.API.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using CMAdmin.API.Data;
using System.Text;
using CMAdmin.API.Helpers;
using System.Collections;

namespace CMAdmin.API.Repositories
{
    public interface IGeneralRepository
    {
        string GetIsScormCloudCollege(string CollegeId);
        string GetStudentUserIdByEmail(string Email);
        string GetStudentPortalURLByCollegeId(string CollegeId);
        String GetDefaultCollegeConfigAlias(string CollegeId = "0", string ConfigFieldName = "");
    }
    public class GeneralRepository : IGeneralRepository
    {
        private readonly IConfiguration _config;
        private readonly ILoggerManager _logger;
        public GeneralRepository(IConfiguration config, ILoggerManager logger)
        {
            _config = config;
            _logger = logger;
        }
        public IDbConnection connection
        {
            get { return new SqlConnection(Config.MCQBConnectionString); }
        }

        public string GetIsScormCloudCollege(string CollegeId)
        {
            string UserId = "0";
            DBAccess oDBAccess = new DBAccess();
            StringBuilder sql = new StringBuilder();
            try
            {
                String logParams = "CollegeId:" + CollegeId;
                _logger.LogInfo("[GeneralRepository]|[GetIsScormCloudCollege]|logParams: " + logParams);

                if (!string.IsNullOrEmpty(CollegeId))
                {
                    sql.Append(" SELECT CAST(ISNULL(ScomeCloud,0) AS VARCHAR(20)) IsScormCloud FROM MCQ_CollegeMaster WHERE CollegeId=" + CollegeId + "  ");
                    UserId = oDBAccess.lfnExecuteScaler<string>(sql.ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
            finally
            {
                if (oDBAccess != null && oDBAccess.isConnectionOpen()) oDBAccess.CloseDB();
            }
            return UserId;
        }
        public string GetStudentUserIdByEmail(string Email)
        {
            DBAccess oDBAccess = null;
            string ErrorMsg = "";
            try
            {
                String logParams = "Email:" + Email;
                _logger.LogInfo("[GeneralRepository]|[GetStudentUserIdByEmail]|logParams: " + logParams);
                oDBAccess = new DBAccess();

                StringBuilder lssql = new StringBuilder();

                lssql.AppendLine(" SELECT CAST(UserId AS VARCHAR(200)) UserId FROM MCQ_Usermaster WHERE dbo.decryptstring('RSD@Flipick2016',Email) = '" + Email + "' ");

                ErrorMsg = oDBAccess.lfnExecuteScaler<string>(lssql.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
            finally
            {
                if (oDBAccess != null && oDBAccess.isConnectionOpen()) oDBAccess.CloseDB();
            }

            return ErrorMsg;
        }
        public string GetStudentPortalURLByCollegeId(string CollegeId)
        {
            DBAccess oDBAccess = null;
            string ErrorMsg = "";
            try
            {
                String logParams = "CollegeId:" + CollegeId;
                _logger.LogInfo("[GeneralRepository]|[GetStudentPortalURLByCollegeId]|logParams: " + logParams);
                oDBAccess = new DBAccess();

                StringBuilder lssql = new StringBuilder();

                lssql.AppendLine(" SELECT StudentPortalURL FROM MCQ_CollegeMaster WHERE CollegeId = " + CollegeId + " ");

                ErrorMsg = oDBAccess.lfnExecuteScaler<string>(lssql.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
            finally
            {
                if (oDBAccess != null && oDBAccess.isConnectionOpen()) oDBAccess.CloseDB();
            }

            return ErrorMsg;
        }
        public string GetDefaultCollegeConfigAlias(string CollegeId = "0", string ConfigFieldName = "")
        {
            string FieldAlias = "";
            DBAccess oDBAccess = null;
            DataTable oDataTable = new DataTable();
            try
            {
                oDBAccess = new DBAccess();
                if (string.IsNullOrEmpty(CollegeId))
                    CollegeId = "0";
                StringBuilder sb = new StringBuilder();
                sb.Append("SP_MCQ_GetDefaultCollegeConfigAlias");
                ArrayList oParameters = new ArrayList();
                oParameters.Add(new SqlParameter() { ParameterName = "@CollegeId", Value = CollegeId });
                oParameters.Add(new SqlParameter() { ParameterName = "@ConfigFieldName", Value = ConfigFieldName });
                DataTable odt = new DataTable();
                FieldAlias = ConfigFieldName;
                odt = oDBAccess.lfnGetDataTableProcedure(sb.ToString(), oParameters);
                if (odt.Rows.Count > 0)
                    FieldAlias = Convert.ToString(odt.Rows[0]["Alias"]);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
            finally
            {
                if (oDBAccess != null && oDBAccess.isConnectionOpen()) oDBAccess.CloseDB();
            }
            return FieldAlias;
        }
    }
}
