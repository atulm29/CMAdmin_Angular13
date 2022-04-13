using CMAdmin.API.Data;
using CMAdmin.API.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMAdmin.API.Repositories
{
    public interface ISubjectRepository
    {
        string GetMagentoFlag(string CollegeId);
    }
    public class SubjectRepository : ISubjectRepository
    {
        private readonly IConfiguration _config;
        private readonly ILoggerManager _logger;
        public SubjectRepository(IConfiguration config, ILoggerManager logger)
        {
            _config = config;
            _logger = logger;
        }

        public string GetMagentoFlag(string CollegeId)
        {
            string IsDefault = string.Empty;
            DBAccess oDBAccess = null;
            try
            {
                String logParams = "CollegeId:" + CollegeId;
                _logger.LogInfo("[SubjectRepository]|[GetMagentoFlag]|logParams: " + logParams);
                oDBAccess = new DBAccess();
                string query = "select IsDefault from MCQ_CollegeMaster WHERE CollegeId=" + CollegeId;
                IsDefault = oDBAccess.lfnExecuteScaler<string>(query);
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
            finally
            {
                if (oDBAccess != null && oDBAccess.isConnectionOpen()) oDBAccess.CloseDB();
            }
            return IsDefault;
        }
    }
}
