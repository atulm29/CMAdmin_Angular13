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
using CMAdmin.API.Data;
using System.Collections;
using CMAdmin.API.Helpers;

namespace CMAdmin.API.Repositories
{
    public interface IAnalyticsReportsRepository
    {
        DataTable GetAdminDashboard(string CollegeId = "", string UserType = "", string InstructorId = "", string Name = "", string RoleId = "");
        DataTable GetDashboardCountTilesData(string CollegeId = "", string InstructorId = "");
    }
    public class AnalyticsReportsRepository : IAnalyticsReportsRepository
    {
        private readonly IConfiguration _config;
        private readonly ILoggerManager _logger;
        public AnalyticsReportsRepository(IConfiguration config, ILoggerManager logger)
        {
            _config = config;
            _logger = logger;
        }
        public IDbConnection connection
        {
            get { return new SqlConnection(Config.MCQBConnectionString); }
        }
        public DataTable GetAdminDashboard(string CollegeId = "", string UserType = "", string InstructorId = "", string Name = "", string RoleId = "")
        {
            DBAccess oDBAccess = null;
            DataTable oDataTable = new DataTable();
            try
            {
                String logParams = "CollegeId: " + CollegeId + "|UserType: " + UserType + "|InstructorId:" + InstructorId
                    + "|Name: " + Name + "|RoleId:" + RoleId;
                _logger.LogInfo("[AnalyticsReportsRepository]|[GetAdminDashboard]|logParams: " + logParams);

                oDBAccess = new DBAccess();

                ArrayList oParameters = new ArrayList();
                if (!string.IsNullOrEmpty(CollegeId))
                    oParameters.Add(new SqlParameter() { ParameterName = "@CollegeId", Value = CollegeId });
                if (!string.IsNullOrEmpty(UserType))
                    oParameters.Add(new SqlParameter() { ParameterName = "@UserType", Value = UserType });
                if (!string.IsNullOrEmpty(InstructorId))
                    oParameters.Add(new SqlParameter() { ParameterName = "@InstructorId", Value = InstructorId });
                if (!string.IsNullOrEmpty(Name))
                    oParameters.Add(new SqlParameter() { ParameterName = "@Name", Value = Name });
                if (!string.IsNullOrEmpty(RoleId))
                    oParameters.Add(new SqlParameter() { ParameterName = "@RoleId", Value = RoleId });
                string query = "SP_MCQ_GetAdminDashboardByRole";

                oDataTable = oDBAccess.lfnGetDataTableProcedure(query, oParameters);
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

        public DataTable GetDashboardCountTilesData(string CollegeId = "", string InstructorId = "")
        {
            DBAccess oDBAccess = null;
            DataTable oDataTable = new DataTable();
            try
            {
                String logParams = "CollegeId: " + CollegeId + "|InstructorId:" + InstructorId;
                _logger.LogInfo("[AnalyticsReportsRepository]|[GetDashboardCountTilesData]|logParams: " + logParams);

                oDBAccess = new DBAccess();

                ArrayList oParameters = new ArrayList();
                if (!string.IsNullOrEmpty(CollegeId))
                    oParameters.Add(new SqlParameter() { ParameterName = "@CollegeId", Value = CollegeId });
                if (!string.IsNullOrEmpty(InstructorId))
                    oParameters.Add(new SqlParameter() { ParameterName = "@CreatedBy", Value = InstructorId });

                string query = "SP_MCQ_GetDashboardCountTilesData";

                oDataTable = oDBAccess.lfnGetDataTableProcedure(query, oParameters);
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
