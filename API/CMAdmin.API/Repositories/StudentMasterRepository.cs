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
    public interface IStudentMasterRepository
    {
        int IsStudentAlreadyExists(string EmailId, int UserId = 0, int CollegeId = 0);
        int IsStudentExists(string loginName, int AdminId);
    }
    public class StudentMasterRepository : IStudentMasterRepository
    {
        private readonly IConfiguration _config;
        private readonly ILoggerManager _logger;
        public StudentMasterRepository(IConfiguration config, ILoggerManager logger)
        {
            _config = config;
            _logger = logger;
        }
        public IDbConnection connection
        {
            get { return new SqlConnection(Config.MCQBConnectionString); }
        }
        public int IsStudentAlreadyExists(string EmailId, int UserId = 0, int CollegeId = 0)
        {
            DBAccess oDBAccess = null;
            int ExtuserId = 0;
            try
            {
                oDBAccess = new DBAccess();
                EmailId = EmailId.Replace("'", "''");
                String sqlStr = "IF (EXISTS (SELECT UserId FROM MCQ_UserMaster WHERE dbo.decryptstring('RSD@Flipick2016',Email)=@Email AND (@UserId IS NULL OR UserId !=@UserId))) "
                                + " BEGIN "
                                + " SELECT CAST(UserId AS INT) FROM MCQ_UserMaster WHERE dbo.decryptstring('RSD@Flipick2016',Email)=@Email AND (@UserId IS NULL OR UserId !=@UserId) "
                                + " END "
                                + " ELSE "
                                + " BEGIN "
                                + " SELECT 0 "
                                + " END ";

                ArrayList oParameters = new ArrayList();
                oParameters.Add(new SqlParameter() { ParameterName = "@Email", Value = EmailId });
                oParameters.Add(new SqlParameter() { ParameterName = "@UserId", Value = UserId });

                ExtuserId = oDBAccess.lfnExecuteScaler<int>(sqlStr, oParameters);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                // LogWriter.WriteLog("IsStudentAlreadyExists(" + EmailId + "," + UserId + ")->Error->" + ex.Message + Environment.NewLine + ex.StackTrace);
            }
            finally
            {
                if (oDBAccess != null && oDBAccess.isConnectionOpen()) oDBAccess.CloseDB();
            }
            return ExtuserId;
        }

        public int IsStudentExists(string loginName, int AdminId)
        {
            DBAccess oDBAccess = null;
            int ExtuserId = 0;
            try
            {

                oDBAccess = new DBAccess();
                loginName = loginName.Replace("'", "''");

                String sqlStr = " IF (EXISTS (SELECT UserId FROM MCQ_UserMaster WHERE dbo.decryptstring('RSD@Flipick2016',Email)=(Select LoginName From MCQ_AdminUserMaster Where AdminUserId=" + AdminId + " and LoginName = '" + loginName + "')))"
                        + "  BEGIN "
                        + "     SELECT 0 "
                        + "  END "
                        + "  ELSE "
                        + "  BEGIN "
                        + "     IF(EXISTS(SELECT UserId FROM MCQ_UserMaster WHERE dbo.decryptstring('RSD@Flipick2016',Email) = '" + loginName + "'))"
                        + "     BEGIN "
                        + "       SELECT cast(UserId as int) FROM MCQ_UserMaster WHERE dbo.decryptstring('RSD@Flipick2016',Email) = '" + loginName + "'"
                        + "     END "
                        + "  ELSE "
                        + "  BEGIN "
                        + "      SELECT 0 "
                        + "  END "
                        + "  END ";

                ExtuserId = oDBAccess.lfnExecuteScaler<int>(sqlStr);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                //LogWriter.WriteLog("IsStudentExists(" + loginName + "," + AdminId + ")->Error->" + ex.Message + Environment.NewLine + ex.StackTrace);
            }
            finally
            {
                if (oDBAccess != null && oDBAccess.isConnectionOpen()) oDBAccess.CloseDB();
            }
            return ExtuserId;
        }
    }
}
