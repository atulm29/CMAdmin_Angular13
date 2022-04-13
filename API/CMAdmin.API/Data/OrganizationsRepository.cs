using CMAdmin.API.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace CMAdmin.API.Data
{
    public class OrganizationsRepository
    {
        private readonly string _connectionString;

        public OrganizationsRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("defaultConnection");
        }

        public async Task<List<Organization>> GetAll()
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                string strSelectQuery = "SELECT * FROM MCQ_GroupMaster ORDER BY GroupName";
                using (SqlCommand cmd = new SqlCommand(strSelectQuery, sql))
                {
                    var response = new List<Organization>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(MapToValue(reader));
                        }
                    }
                    return response;
                }
            }
        }
        public async Task<Organization> GetById(int Id)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                string strSelectQuery = "SELECT * FROM MCQ_GroupMaster ";
                strSelectQuery += " WHERE GroupId=" + Id + " ORDER BY GroupName";
                using (SqlCommand cmd = new SqlCommand(strSelectQuery, sql))
                {
                    Organization response = null;
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response = MapToValue(reader);
                        }
                    }

                    return response;
                }
            }
        }
        public async Task Insert(string GroupName, string CollegeId, string OrganizationType)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                string strInsertQuery = $"INSERT INTO MCQ_GroupMaster(GroupName, CollegeId, OrganizationType) VALUES('" + GroupName + "', " + CollegeId + ", '" + OrganizationType + "');";
                using (SqlCommand cmd = new SqlCommand(strInsertQuery, sql))
                {
                    cmd.CommandType = CommandType.Text;
                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    return;
                }
            }
        }
        public async Task DeleteById(int Id)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DeleteValue", sql))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Id", Id));
                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    return;
                }
            }
        }
        private Organization MapToValue(SqlDataReader reader)
        {
            return new Organization()
            {
                GroupId = Convert.IsDBNull(reader["GroupId"]) ? 0 : (int)reader["GroupId"],
                GroupName = Convert.IsDBNull(reader["GroupName"]) ? string.Empty : reader["GroupName"].ToString(),
                CollegeId = Convert.IsDBNull(reader["CollegeId"]) ? 0 : (int)reader["CollegeId"],
                ShowNewUserTab = Convert.IsDBNull(reader["ShowNewUserTab"]) ? string.Empty : reader["ShowNewUserTab"].ToString(),
                ShowAssessmentsTab = Convert.IsDBNull(reader["ShowAssessmentsTab"]) ? string.Empty : reader["ShowAssessmentsTab"].ToString(),
                MasterCourseId = Convert.IsDBNull(reader["MasterCourseId"]) ? 0 : (int)reader["MasterCourseId"],
                AssessmentTabName = Convert.IsDBNull(reader["AssessmentTabName"]) ? string.Empty : reader["AssessmentTabName"].ToString(),
                OrganizationType = Convert.IsDBNull(reader["OrganizationType"]) ? string.Empty : reader["OrganizationType"].ToString(),
                OrganizationId = Convert.IsDBNull(reader["OrganizationId"]) ? 0 : (int)reader["OrganizationId"]
            };
        }
    }
}
