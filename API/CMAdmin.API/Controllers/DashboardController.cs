using CMAdmin.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using CMAdmin.API.Repositories;
using CMAdmin.API.Services;
using System.Linq;
using System.Security.Claims;
using CMAdmin.API.Helpers;


namespace CMAdmin.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IDashboardService _dashboardService;
        public DashboardController(ILoggerManager logger, IDashboardService dashboardService)
        {
            this._logger = logger;
            this._dashboardService = dashboardService ?? throw new ArgumentNullException(nameof(dashboardService));
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var objAdminUser = TokenHelper.GetTokenCliamData(HttpContext);
                _logger.LogDebug("[DashboardController]|[Get]|Start => Get All records API.");
                var result =  _dashboardService.GetAdminDashboard(objAdminUser.AdminUserId, objAdminUser.CollegeId.ToString(), objAdminUser.RoleId.ToString(), objAdminUser.UserType);
                if (result != null)
                    _logger.LogDebug("[DashboardController]|[Get]|End => Get All records API : " + result.Count);
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
