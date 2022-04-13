using CMAdmin.API.Helpers;
using CMAdmin.API.Models;
using CMAdmin.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
		private readonly IAdminUserMasterRepository _repository;
		public UsersController(ILoggerManager logger, IAdminUserMasterRepository repository)
		{
			this._logger = logger;
			this._repository = repository ?? throw new ArgumentNullException(nameof(repository));
		}

        [HttpGet]
        public async Task<IActionResult> Get()
        {

            try
            {
                _logger.LogDebug("[AdminUserMasterController]|[Get]|Start => Get All records API.");
                var result = await _repository.GetAll();
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
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                _logger.LogDebug("[AdminUserMasterController]|[GetById]|Start => GetById => input id: " + id);
                var result = await _repository.GetById(id);
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
                var result = await _repository.GetCollegeInstructors(CollegeId, "L,V,A", AdminUserID);
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
    }
}
