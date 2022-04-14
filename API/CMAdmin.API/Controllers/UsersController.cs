using CMAdmin.API.Helpers;
using CMAdmin.API.Models;
using CMAdmin.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Threading.Tasks;

namespace CMAdmin.API.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
		private readonly ILoggerManager _logger;
		private readonly IAdminUserMasterRepository _adminRepository;
        private readonly IStudentMasterRepository _sudentMasterRepository;
        public UsersController(ILoggerManager logger, IAdminUserMasterRepository repository, IStudentMasterRepository sudentMasterRepository)
		{
			this._logger = logger;
			this._adminRepository = repository ?? throw new ArgumentNullException(nameof(repository));
            this._sudentMasterRepository = sudentMasterRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {

            try
            {
                _logger.LogDebug("[AdminUserMasterController]|[Get]|Start => Get All records API.");
                var result = await _adminRepository.GetAll();
                if (result != null)
                {
                    _logger.LogDebug("[AdminUserMasterController]|[Get]|End => Get All records API count: " + result.Count);
                }
                return Ok(new ApiResponse(Return.StatusCodes.Success, Return.Messages.Success, result));
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                Error error = Error.GetError(ex);
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new ApiException(Return.StatusCodes.Exception, Return.Messages.Exception, error));
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                _logger.LogDebug("[AdminUserMasterController]|[GetById]|Start => GetById => input id: " + id);
                var result = _adminRepository.GetProfessorDetailsByProfessorId(id);
                if (result == null)
                {
                    return NotFound(new ApiResponse(Return.StatusCodes.Fail, Return.Messages.NotFound, result));
                }
                return Ok(new ApiResponse(Return.StatusCodes.Success, Return.Messages.Success, result));
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                Error error = Error.GetError(ex);
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new ApiException(Return.StatusCodes.Exception, Return.Messages.Exception, error));
            }
        }


        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers(string CollegeId="0")
        {
            try
            {
                var objAdminUser = TokenHelper.GetTokenCliamData(HttpContext);
                string AdminUserID = "";
                if (objAdminUser.UserType == "V" || objAdminUser.UserType == "L")
                {
                    AdminUserID = objAdminUser.AdminUserId.ToString();
                }
                _logger.LogDebug("[AdminUserMasterController]|[GetUsers]|Start => GetUsers => CollegeId: " + CollegeId);
                var result = await _adminRepository.GetCollegeInstructors(CollegeId, "L,V,A", AdminUserID);
                if (result == null)
                {
                    return NotFound(new ApiResponse(Return.StatusCodes.Fail, Return.Messages.NotFound, result));
                }
                return Ok(new ApiResponse(Return.StatusCodes.Success, Return.Messages.Success, result));
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                Error error = Error.GetError(ex);
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new ApiException(Return.StatusCodes.Exception, Return.Messages.Exception, error));
            }
        }


        [HttpPost("SaveUser")]
        public IActionResult SaveUser([FromBody] Users objModel)
        {
            try
            {
                string AdminUserType = "L"; string IsStudentLogin = "N"; string TrainerType = ""; string InstructorExpirationDays = "";
                string AllowStudentRegistration = "1"; string DeptSectionId = "";
                _logger.LogDebug("[UsersController]|[SaveUsers]|Start => SaveUsers");
                Error error = new Error();
                int ExistingStudentId = _sudentMasterRepository.IsStudentAlreadyExists(objModel.LoginName);
                if (ExistingStudentId > 0)
                {
                    error.ErrorCode = "SaveUserError_001";
                    error.Message = "LoginName already exists for Student :" + objModel.LoginName;
                    return Ok(new ApiFailResponse(Return.StatusCodes.Fail, Return.Messages.Success, error));
                }
                int ExistingUserId = _adminRepository.IsAdminUserAlreadyExists(objModel.LoginName, string.Empty);
                if (ExistingUserId > 0)
                {
                    error.ErrorCode = "SaveUserError_002";
                    error.Message = "LoginName already exists for Instructor :" + objModel.LoginName;
                    return Ok(new ApiFailResponse(Return.StatusCodes.Fail, Return.Messages.Success, error));              
                }
                string UserID = _adminRepository.AddInstructor(objModel.FirstName, objModel.LastName, objModel.LoginName, objModel.Password, objModel.CollegeId, AdminUserType, objModel.RoleId, IsStudentLogin
                                                               , TrainerType, InstructorExpirationDays, AllowStudentRegistration, DeptSectionId);
                if (UserID != "")
                {
                    return Ok(new ApiResponse(Return.StatusCodes.Success, Return.Messages.Success, objModel));
                }
                else
                {
                    error.ErrorCode = "SaveUserError_003";
                    error.Message = "Error occure while user saving.";
                    return Ok(new ApiFailResponse(Return.StatusCodes.Fail, Return.Messages.Success, error));
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                Error error = Error.GetError(ex);
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new ApiException(Return.StatusCodes.Exception, Return.Messages.Exception, error));
            }
        }


        [HttpPost("UpdateUser")]
        public IActionResult UpdateUser([FromBody] Users objModel)
        {
            try
            {
                string AdminUserType = "L"; string IsStudentLogin = "N"; string TrainerType = ""; 
                string AllowStudentRegistration = "1"; string DeptSectionId = "";
                _logger.LogDebug("[UsersController]|[UpdateUser]|Start => UpdateUser");
                Error error = new Error();
                int ExistingStudentId = _sudentMasterRepository.IsStudentExists(objModel.LoginName, objModel.AdminUserId);
                if (ExistingStudentId > 0)
                {
                    error.ErrorCode = "UpdateserError_001";
                    error.Message = "LoginName already exists for Student :" + objModel.LoginName;
                    return Ok(new ApiFailResponse(Return.StatusCodes.Fail, Return.Messages.Success, error));
                }
                bool UpdateStudents = false;
                DateTime Expirationdate = new DateTime();
                string UserID = _adminRepository.UpdateInstructor(objModel.FirstName, objModel.LastName, objModel.LoginName, objModel.Password, objModel.CollegeId, AdminUserType, objModel.AdminUserId.ToString(), TrainerType, Expirationdate.ToString("dd/MM/yyyy")
                    , UpdateStudents, AllowStudentRegistration, DeptSectionId, objModel.RoleId, IsStudentLogin);

                if (UserID != "")
                {
                    return Ok(new ApiResponse(Return.StatusCodes.Success, Return.Messages.Success, objModel));
                }
                else
                {
                    error.ErrorCode = "UpdateserError_002";
                    error.Message = "Error occure while user saving.";
                    return Ok(new ApiFailResponse(Return.StatusCodes.Fail, Return.Messages.Success, error));
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                Error error = Error.GetError(ex);
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new ApiException(Return.StatusCodes.Exception, Return.Messages.Exception, error));
            }
        }
    }
}
