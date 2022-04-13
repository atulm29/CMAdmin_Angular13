using CMAdmin.API.Data;
using CMAdmin.API.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CMAdmin.API.Repositories
{
    public interface ISemesterRepository
    {
        DataTable GetSemesterListForInstructor(string ProfessorId, string CollegeId, string ExamID = "0", string LevelId = "", string UniversityId = "", string DisciplineId = "");
    }
    public class SemesterRepository : ISemesterRepository
    {
        private readonly IConfiguration _config;
        private readonly ILoggerManager _logger;
        public SemesterRepository(IConfiguration config, ILoggerManager logger)
        {
            _config = config;
            _logger = logger;
        }

        public DataTable GetSemesterListForInstructor(string ProfessorId, string CollegeId, string ExamID = "0", string LevelId = "", string UniversityId = "", string DisciplineId = "")
        {
            DBAccess oDBAccess = null;
            DataTable oDataTable = new DataTable();
            try
            {
                String logParams = "CollegeId: " + CollegeId + "|ProfessorId: " + ProfessorId + "|ExamID:" + ExamID + "|UniversityId:" + UniversityId
                                    + "|DisciplineId: " + DisciplineId;
                _logger.LogInfo("[SemesterRepository]|[GetSemesterListForInstructor]|logParams: " + logParams);

                oDBAccess = new DBAccess();

                ArrayList oParameters = new ArrayList();

                if (!string.IsNullOrEmpty(ProfessorId))
                    oParameters.Add(new SqlParameter() { ParameterName = "@ProfessorId", Value = ProfessorId });
                if (!string.IsNullOrEmpty(CollegeId))
                    oParameters.Add(new SqlParameter() { ParameterName = "@CollegeId", Value = CollegeId });
                if (!string.IsNullOrEmpty(ExamID))
                    oParameters.Add(new SqlParameter() { ParameterName = "@ExamID", Value = ExamID });
                if (!string.IsNullOrEmpty(LevelId))
                    oParameters.Add(new SqlParameter() { ParameterName = "@LevelId", Value = LevelId });
                if (!string.IsNullOrEmpty(UniversityId))
                    oParameters.Add(new SqlParameter() { ParameterName = "@UniversityId", Value = UniversityId });
                if (!string.IsNullOrEmpty(DisciplineId))
                    oParameters.Add(new SqlParameter() { ParameterName = "@DisciplineId", Value = DisciplineId });
                if (!string.IsNullOrEmpty(Convert.ToString(Config.SortTreeViewSemesterBy)))
                    oParameters.Add(new SqlParameter() { ParameterName = "@OrderBy", Value = Config.SortTreeViewSemesterBy.Trim() });

                string query = "SP_MCQ_GetSemesterListForInstructor";
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
