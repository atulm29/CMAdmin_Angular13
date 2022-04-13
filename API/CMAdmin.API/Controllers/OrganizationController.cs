using CMAdmin.API.Helpers;
using CMAdmin.API.Models;
using CMAdmin.API.Repositories;
using CMAdmin.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CMAdmin.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly ILoggerManager _logger;
        private readonly IOrganizationRepository _repository;
        private readonly IOrganizationService _organizationService;
        private readonly IGeneralRepository _generalRepository;
        public OrganizationController(ILoggerManager logger, 
                                     IOrganizationRepository repository, 
                                     IOrganizationService organizationService,
                                     IGeneralRepository generalRepository)
        {
            _logger = logger;            
            this._repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _organizationService = organizationService;
            _generalRepository = generalRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                _logger.LogDebug("[OrganizationController]|[Get]|Start => Get All records API.");
                var result = await _repository.GetAll();
                if(result != null)
                _logger.LogDebug("[OrganizationController]|[Get]|End => Get All records API count: " + result.Count);
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

        [HttpGet("GetFiltered")]
        public  IActionResult GetFiltered([FromQuery] PaginationFilter filter)
        {
            try
            {
                _logger.LogDebug("[OrganizationController]|[GetFiltered]|Start => Get records by filter API.");
                var result = _organizationService.GetOrganizationGroupList(filter.SearchTitle, filter.PageNumber, filter.PageSize);
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
                    _logger.LogDebug("[OrganizationController]|[GetFiltered]|End => Get records by filter API count: " + result.RowResults.Count);                   

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

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                _logger.LogDebug("[OrganizationController]|[Get]|Start => GetById => input id: " + id);
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

    
        [HttpPost("createorganization")]
        public IActionResult CreateOrganization([FromBody] CreateOrganization objModel)
        {
            try
            {
                _logger.LogDebug("[OrganizationController]|[CreateOrganization]|Start => CreateOrganization");
                 string InstituteId = "0", GroupName = "", OrganizationType = string.Empty, groupConfigLabel = string.Empty, instituteLabelConfig = string.Empty;
                groupConfigLabel = _generalRepository.GetDefaultCollegeConfigAlias("0", "Group");
                instituteLabelConfig = _generalRepository.GetDefaultCollegeConfigAlias("0", "College");
                if (!string.IsNullOrEmpty(objModel.GroupName.Trim()))
                    GroupName = objModel.GroupName.Trim();

                if (!string.IsNullOrEmpty(objModel.OrganizationType))
                {
                    OrganizationType = "Indesign";
                }
                int ExtGroupId = 0;

                if (!string.IsNullOrEmpty(GroupName))
                {
                    ExtGroupId = _repository.IsCollegeGroupUserAlreadyExists(GroupName);
                    if (ExtGroupId > 0)
                    {
                        Error error = new Error();
                        error.ErrorCode = "GroupExist001";
                        error.Message = groupConfigLabel + " already exist."; 
                        return Ok(new ApiFailResponse(Return.StatusCodes.Fail, Return.Messages.Success, error));                       
                    }
                    objModel.GroupId = _repository.AddCollegeGroup(GroupName, InstituteId, OrganizationType);
                    string message = groupConfigLabel + " created successfully. For copying courses to it you need to first create master " + instituteLabelConfig + " for this " + groupConfigLabel + ".";
                  
                    return Ok(new ApiResponse(Return.StatusCodes.Success, message, objModel));
                }
                else
                {
                    Error error = new Error();
                    error.ErrorCode = "GroupExist002";
                    error.Message = "Enter " + groupConfigLabel + " name";
                    return NotFound(new ApiResponse(Return.StatusCodes.Fail, Return.Messages.NotFound, error)); 
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
        
        [HttpPut("{id}")]
        public IActionResult UpdateOrganization(int id, [FromBody] Organization objModel)
        {
            return null;
        }
       
        [HttpDelete("{id}")]
        public IActionResult DeleteOrganization(int id)
        {
            return null;
        }
    }
}
