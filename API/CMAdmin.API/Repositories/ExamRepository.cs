using CMAdmin.API.Data;
using CMAdmin.API.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CMAdmin.API.Repositories
{
    public interface IExamRepository
    {
        DataTable GetExamByCollegeId(int CollegeId, int ProfessorId, bool IsFlipickInstitute, int IsStudent = 0, bool ApplyOrderBy = false, string AssessmentCourseId = "");
    }
    public class ExamRepository : IExamRepository
    {
        private readonly IConfiguration _config;
        private readonly ILoggerManager _logger;
        public ExamRepository(IConfiguration config, ILoggerManager logger)
        {
            _config = config;
            _logger = logger;
        }

        public DataTable GetExamByCollegeId(int CollegeId, int ProfessorId, bool IsFlipickInstitute, int IsStudent = 0, bool ApplyOrderBy = false, string AssessmentCourseId = "")
        {
            DBAccess oDBAccess = null;
            DataTable oDataTable = new DataTable();
            try
            {
                String logParams = "CollegeId: " + CollegeId + "|ProfessorId: " + ProfessorId + "|IsFlipickInstitute:" + IsFlipickInstitute
                                 + "|IsStudent: " + IsStudent + "|ApplyOrderBy:" + ApplyOrderBy + "|AssessmentCourseId:" + AssessmentCourseId;
                _logger.LogInfo("[ExamRepository]|[GetExamByCollegeId]|logParams: " + logParams);

                oDBAccess = new DBAccess();
                string SelectBranches = " SELECT DISTINCT ExamId FROM MCQ_PFBranch WHERE CollegeId=" + CollegeId + " AND ProfessorId=" + ProfessorId + " ";
                DataTable odtProfBatches = oDBAccess.lfnGetDataTable(SelectBranches);
                string ProfBranches = "";
                if (IsStudent == 0)
                {
                    if (odtProfBatches.Rows.Count > 0)
                    {
                        foreach (DataRow dr in odtProfBatches.Rows)
                        {
                            ProfBranches += dr["ExamId"].ToString() + ",";
                        }
                    }
                    else
                    {
                        if (!IsFlipickInstitute)
                            return odtProfBatches;
                    }
                }
                if (ProfBranches.EndsWith(","))
                    ProfBranches = ProfBranches.Substring(0, ProfBranches.Length - 1);
                if (ProfBranches.StartsWith(","))
                    ProfBranches = ProfBranches.Substring(1, ProfBranches.Length - 1);

                string Selectstr = " SELECT *, "
                            + " CASE WHEN CourseCount = 1 THEN(SELECT TOP(1) CourseId FROM MCQ_CollegeCourse WHERE CollegeId = T.CollegeId AND ExamId = T.ExamId) "
                            + " ELSE 0 END CourseId "
                            + " FROM( "


                           + " SELECT DISTINCT MCQ_CollegeMaster.CollegeId, MCQ_CollegeMaster.CollegeName, MCQ_ExamMaster.ExamId, MCQ_ExamMaster.ExamName"
                           + " ,MCQ_UniversityMaster.UniversityId, MCQ_UniversityMaster.UniversityName, MCQ_CourseLevelMaster.LevelId, MCQ_CourseLevelMaster.LevelName"
                           + " ,MCQ_DisciplineMaster.DisciplineId,MCQ_DisciplineMaster.DisciplineName "
                           + " ,ViewCourselevel,ViewUniversity,ViewDiscipline,ViewExam "
                           + " ,(SELECT Count(DISTINCT CourseId) FROM MCQ_CollegeCourse WHERE CollegeId=MCQ_CollegeMaster.CollegeId AND ExamId=MCQ_ExamMaster.ExamId) CourseCount "
                           + " FROM MCQ_ExamMaster "
                           + " INNER JOIN MCQ_DisciplineMaster ON MCQ_ExamMaster.DisciplineId = MCQ_DisciplineMaster.DisciplineId"
                           + " INNER JOIN MCQ_UniversityMaster ON MCQ_DisciplineMaster.UniversityId = MCQ_UniversityMaster.UniversityId"
                           + " INNER JOIN MCQ_CourseLevelMaster ON MCQ_UniversityMaster.LevelId = MCQ_CourseLevelMaster.LevelId"
                           + " INNER JOIN MCQ_CollegeLevel ON MCQ_CollegeLevel.LevelId = MCQ_CourseLevelMaster.LevelId"
                           + " 	AND MCQ_CollegeLevel.UniversityId = MCQ_UniversityMaster.UniversityId"
                           + " INNER JOIN MCQ_CollegeMaster ON MCQ_CollegeMaster.CollegeId = MCQ_CollegeLevel.CollegeId"
                           + " INNER JOIN MCQ_CollegeDiscipline ON MCQ_CollegeDiscipline.CollegeId = MCQ_CollegeMaster.CollegeId"
                           + " 	AND MCQ_DisciplineMaster.DisciplineId = MCQ_CollegeDiscipline.DisciplineId"
                           + " INNER JOIN MCQ_CollegeExam ON MCQ_CollegeMaster.CollegeId = MCQ_CollegeExam.CollegeId"
                           + " 	AND MCQ_DisciplineMaster.DisciplineId = MCQ_CollegeExam.DisciplineId"
                           + " 	AND MCQ_ExamMaster.ExamId = MCQ_CollegeExam.ExamId";
                if (IsStudent == 1)
                {
                    Selectstr += " INNER JOIN MCQ_CollegeCourse ON MCQ_CollegeMaster.CollegeId = MCQ_CollegeCourse .CollegeId AND MCQ_ExamMaster.ExamId = MCQ_CollegeCourse .ExamId ";
                    Selectstr += " INNER JOIN MCQ_BranchMaster ON MCQ_CollegeCourse .BranchId = MCQ_BranchMaster .BranchId ";
                    Selectstr += " INNER JOIN MCQ_BatchMaster ON MCQ_BatchMaster.CourseId = MCQ_CollegeCourse.CourseId AND MCQ_BranchMaster.BranchId=MCQ_BatchMaster.BranchId ";
                    Selectstr += " INNER JOIN MCQ_StudentBatch ON MCQ_BatchMaster.BatchId = MCQ_StudentBatch.BatchId ";

                }
                Selectstr += " INNER JOIN MCQ_CollegeLevelMaster ON MCQ_CollegeMaster.CollegeId = MCQ_CollegeLevelMaster.CollegeId ";
                Selectstr += " INNER JOIN MCQ_CollegeLevelDetails ON MCQ_CollegeLevelMaster.CollegeLevelId = MCQ_CollegeLevelDetails.CollegeLevelId ";
                Selectstr += " WHERE MCQ_CollegeMaster.CollegeId =" + CollegeId;
                if (IsStudent == 1)
                {
                    Selectstr += " AND MCQ_StudentBatch.StudentId=" + ProfessorId + " ";
                }

                if (ProfBranches != "0" && ProfBranches.Length > 0)
                {
                    Selectstr += " AND MCQ_ExamMaster.ExamId IN (" + ProfBranches + ")";
                }
                Selectstr += " ) AS T ";

                if (!string.IsNullOrEmpty(Convert.ToString(Config.SortTreeViewExamsBy)))
                    Selectstr += " ORDER By " + Config.SortTreeViewExamsBy.Trim();

                oDataTable = oDBAccess.lfnGetDataTable(Selectstr);
                if (!string.IsNullOrEmpty(AssessmentCourseId))
                {
                    DataRow[] drr = oDataTable.Select("CourseId=" + AssessmentCourseId + " AND CourseCount=1 ");
                    for (int i = 0; i < drr.Length; i++)
                        drr[i].Delete();
                    oDataTable.AcceptChanges();
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
            return oDataTable;
        }

    }
}
