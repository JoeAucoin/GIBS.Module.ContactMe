using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Oqtane.Shared;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using GIBS.Module.ContactMe.Services;
using Oqtane.Controllers;
using System.Net;
using System.Threading.Tasks;

namespace GIBS.Module.ContactMe.Controllers
{
    [Route(ControllerRoutes.ApiRoute)]
    public class ContactMeController : ModuleControllerBase
    {
        private readonly IContactMeService _ContactMeService;

        public ContactMeController(IContactMeService ContactMeService, ILogManager logger, IHttpContextAccessor accessor) : base(logger, accessor)
        {
            _ContactMeService = ContactMeService;
        }

        // GET: api/<controller>?moduleid=x
        [HttpGet]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<IEnumerable<Models.ContactMe>> Get(string moduleid)
        {
            int ModuleId;
            if (int.TryParse(moduleid, out ModuleId) && IsAuthorizedEntityId(EntityNames.Module, ModuleId))
            {
                return await _ContactMeService.GetContactMesAsync(ModuleId);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized ContactMe Get Attempt {ModuleId}", moduleid);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return null;
            }
        }

        // GET api/<controller>/5
        [HttpGet("{id}/{moduleid}")]
        [Authorize(Policy = PolicyNames.ViewModule)]
        public async Task<Models.ContactMe> Get(int id, int moduleid)
        {
            Models.ContactMe ContactMe = await _ContactMeService.GetContactMeAsync(id, moduleid);
            if (ContactMe != null && IsAuthorizedEntityId(EntityNames.Module, ContactMe.ModuleId))
            {
                return ContactMe;
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized ContactMe Get Attempt {ContactMeId} {ModuleId}", id, moduleid);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return null;
            }
        }

        // POST api/<controller>
        [HttpPost]
        [AllowAnonymous]
        public async Task<Models.ContactMe> Post([FromBody] Models.ContactMe ContactMe)
        {
            if (ModelState.IsValid)
            {
                ContactMe = await _ContactMeService.AddContactMeAsync(ContactMe);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized ContactMe Post Attempt {ContactMe}", ContactMe);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                ContactMe = null;
            }
            return ContactMe;
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task<Models.ContactMe> Put(int id, [FromBody] Models.ContactMe ContactMe)
        {
            if (ModelState.IsValid && ContactMe.ContactMeId == id && IsAuthorizedEntityId(EntityNames.Module, ContactMe.ModuleId))
            {
                ContactMe = await _ContactMeService.UpdateContactMeAsync(ContactMe);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized ContactMe Put Attempt {ContactMe}", ContactMe);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                ContactMe = null;
            }
            return ContactMe;
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}/{moduleid}")]
        [Authorize(Policy = PolicyNames.EditModule)]
        public async Task Delete(int id, int moduleid)
        {
            Models.ContactMe ContactMe = await _ContactMeService.GetContactMeAsync(id, moduleid);
            if (ContactMe != null && IsAuthorizedEntityId(EntityNames.Module, ContactMe.ModuleId))
            {
                await _ContactMeService.DeleteContactMeAsync(id, ContactMe.ModuleId);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized ContactMe Delete Attempt {ContactMeId} {ModuleId}", id, moduleid);
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }
        }
    }
}