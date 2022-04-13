using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using CMAdmin.API.Helpers;
using CMAdmin.API.Services;
using CMAdmin.API.Models;
using System.Net;
using CMAdmin.API.Repositories;
using System.Data;

namespace CMAdmin.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersRolesController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IRoleMasterRepository _roleMasterRepository;
        public UsersRolesController(ILoggerManager logger, IRoleMasterRepository roleMasterRepository)
        {
            this._logger = logger;
            this._roleMasterRepository = roleMasterRepository;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                _logger.LogDebug("[UsersRolesController]|[Get]|Start => GetById => input id: " + id);
                var result = _roleMasterRepository.GetRoleDataWithPermission(id);
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

        [HttpGet("GetAllRole")]
        public async Task<IActionResult> GetAllRole(int CollegeId = 0)
        {
            try
            {
                if(CollegeId == 0) { CollegeId = -1; }
                _logger.LogDebug("[UsersRolesController]|[GetAllRole]|Start => GetAllRole => input CollegeId: " + CollegeId.ToString());
                var result = await _roleMasterRepository.GetAllRole(CollegeId);
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

        [HttpGet("GetPermission")]
        public async Task<IActionResult> GetPermission(int CollegeId = 0)
        {
            try
            {
                string AdminCollegeId = string.Empty;
                if (CollegeId == 0) 
                {
                    var objAdminUser = TokenHelper.GetTokenCliamData(HttpContext);
                    AdminCollegeId = objAdminUser.CollegeId; 
                }
                else { AdminCollegeId = CollegeId.ToString(); }
               
                _logger.LogDebug("[UsersRolesController]|[GetPermission]|Start => GetPermission => input CollegeId: " + CollegeId.ToString());
                var result = await _roleMasterRepository.GetRoleMaster(AdminCollegeId, "Y");
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

        [HttpPost("SaveRoles")]
        public IActionResult SaveRoles([FromBody] CreateRoles objModel)
        {
            try
            {
                int RoleID = 0;
                int Result = 0;
                _logger.LogDebug("[UsersRolesController]|[SaveRoles]|Start => SaveRoles");
                DataTable dt = new DataTable();
                dt = _roleMasterRepository.GetUniqueRoleName(Convert.ToInt32(objModel.RoleId), objModel.RoleName, Convert.ToInt32(objModel.CollegeId));
                if (dt.Rows.Count > 0)
                {
                    Error error = new Error();
                    error.ErrorCode = "RoleNameExist001";
                    error.Message = "Role name should be unique.";
                    return Ok(new ApiFailResponse(Return.StatusCodes.Fail, Return.Messages.Success, error));                   
                }
                else
                {
                    RoleID = _roleMasterRepository.SaveRole(Convert.ToInt32(objModel.RoleId), objModel.RoleName, Convert.ToInt32(objModel.CollegeId));
                    if (_roleMasterRepository.DeleteRolePermission(RoleID))
                    {
                        foreach (Permissions node in objModel.Permissions)
                        {
                            Result = _roleMasterRepository.SaveRolePermissions(RoleID, node.Name, "", Convert.ToInt32(objModel.CollegeId));
                        }
                    }

                    return Ok(new ApiResponse(Return.StatusCodes.Success, Return.Messages.Success, objModel));
                }
            }
            catch(Exception ex)
            {
                _logger.LogException(ex);
                Error error = Error.GetError(ex);
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new ApiException(Return.StatusCodes.Exception, Return.Messages.Exception, error));
            }
        }
    }
}
