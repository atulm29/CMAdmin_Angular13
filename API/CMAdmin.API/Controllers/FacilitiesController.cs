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

namespace CMAdmin.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FacilitiesController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IFacilitiesService _facilitiesService;
        private readonly ICollegeMasterRepository _collegeMasterRepository;
        public FacilitiesController(ILoggerManager logger, IFacilitiesService facilitiesService, ICollegeMasterRepository collegeMasterRepository)
        {
            this._logger = logger;
            this._facilitiesService = facilitiesService ?? throw new ArgumentNullException(nameof(facilitiesService));
            this._collegeMasterRepository = collegeMasterRepository;
        }

        [HttpGet("GetFiltered")]
        public IActionResult GetFiltered([FromQuery] PaginationFilter filter)
        {
            try
            {
                var objAdminUser = TokenHelper.GetTokenCliamData(HttpContext);
                _logger.LogDebug("[FacilitiesController]|[GetFiltered]|Start => Get records by filter API.");
                var result = _facilitiesService.GetFaclityList(objAdminUser.CollegeId.ToString(), objAdminUser.UserType, filter.PageNumber, filter.PageSize);
                Pagination objPagination = new Pagination();
                objPagination.TotalItems = result.TotalRecords;
                if (result.TotalRecords > 0)
                {
                    objPagination.TotalRecordsText = result.TotalRecords.ToString() + " " + (result.TotalRecords > 1 ? "Records" : "Record") + " Found";
                }
                else
                {
                    objPagination.TotalRecordsText = "0 Record Found";
                }
                objPagination.PageNumber = filter.PageNumber;
                objPagination.PageSize = filter.PageSize;
                objPagination.PageList = objPagination.GetPageListItem(objPagination.TotalItems, objPagination.PageSize);
                if (result != null)
                {
                    _logger.LogDebug("[FacilitiesController]|[GetFiltered]|End => Get records by filter API count: " + result.TotalRecords);

                }
                return Ok(new ApiResponse(Return.StatusCodes.Success, Return.Messages.Success, result, objPagination));
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                Error error = Error.GetError(ex);
                return StatusCode((int)HttpStatusCode.InternalServerError,
                    new ApiException(Return.StatusCodes.Exception, Return.Messages.Exception, error));
            }
        }


        [HttpGet("GetRoleInstitute")]
        public async Task<IActionResult> GetRoleInstitute(int CollegeId =0, int GroupId =0)
        {
            try
            {
                _logger.LogDebug("[FacilitiesController]|[GetRoleInstitute]|Start => GetRoleInstitute => input CollegeId: " + CollegeId.ToString() + "| GroupId: " + GroupId.ToString());
                var result = await _collegeMasterRepository.GetDistinctCollegeListByAddressCity("","", GroupId.ToString(), CollegeId.ToString());
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
