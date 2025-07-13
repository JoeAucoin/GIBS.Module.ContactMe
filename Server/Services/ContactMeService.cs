using GIBS.Module.ContactMe.Repository;
using Microsoft.AspNetCore.Http;
using Oqtane.Controllers;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Security;
using Oqtane.Services;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GIBS.Module.ContactMe.Services
{
    public class ServerContactMeService : IContactMeService
    {
        private readonly IContactMeRepository _ContactMeRepository;
        private readonly IUserPermissions _userPermissions;
        private readonly ILogManager _logger;
        private readonly IHttpContextAccessor _accessor;
        private readonly Alias _alias;
        private readonly INotificationRepository _notifications;
        //  private readonly ISettingRepository _settingRepository;
        //  private readonly IModuleRepository _moduleRepository;
        //  private readonly ISiteRepository _siteRepository;
        private readonly IUserRepository _userRepository;
       // private readonly Oqtane.Services.INotificationService _notifications;
       // private object _notificationService;

        public ServerContactMeService(IContactMeRepository ContactMeRepository, INotificationRepository notifications, IUserPermissions userPermissions, ITenantManager tenantManager, ILogManager logger, IHttpContextAccessor accessor, ISettingRepository settingRepository, IModuleRepository moduleRepository, ISiteRepository siteRepository, IUserRepository userRepository)
        {
            _ContactMeRepository = ContactMeRepository;
            _userPermissions = userPermissions;
            _logger = logger;
            _accessor = accessor;
            _alias = tenantManager.GetAlias();
            _notifications = notifications;
            //_settingRepository = settingRepository;
            //_moduleRepository = moduleRepository;
            //_siteRepository = siteRepository;
            _userRepository = userRepository;
        }

        public Task<List<User>> GetUsersAsync()
        {
            // use the server-side IUserRepository to get the users
            return Task.FromResult(_userRepository.GetUsers().ToList());
        }

        public Task<List<Models.ContactMe>> GetContactMesAsync(int ModuleId)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, ModuleId, PermissionNames.View))
            {
                return Task.FromResult(_ContactMeRepository.GetContactMes(ModuleId).ToList());
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized ContactMe Get Attempt {ModuleId}", ModuleId);
                return null;
            }
        }

        public Task<Models.ContactMe> GetContactMeAsync(int ContactMeId, int ModuleId)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, ModuleId, PermissionNames.View))
            {
                return Task.FromResult(_ContactMeRepository.GetContactMe(ContactMeId));
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized ContactMe Get Attempt {ContactMeId} {ModuleId}", ContactMeId, ModuleId);
                return null;
            }
        }


        public async Task<Models.ContactMe> AddContactMeAsync(Models.ContactMe ContactMe)
        {
            // check honeypot field
            if (!string.IsNullOrEmpty(ContactMe.Fax))
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Bot submission detected based on honeypot field.");
                return await Task.FromResult<Models.ContactMe>(null); // Ensure async behavior
            }

            var site = _alias.SiteId; // _siteRepository.GetSite(_alias.SiteId);
           
            var body = new StringBuilder();
            body.AppendLine($"<p>You have received a new contact form submission:</p>");
            body.AppendLine($"<p><b>Name:</b> {ContactMe.Name}</p>");
            body.AppendLine($"<p><b>Company:</b> {ContactMe.Company}</p>");
            body.AppendLine($"<p><b>Address:</b> {ContactMe.Address}</p>");
            body.AppendLine($"<p><b>Phone:</b> {ContactMe.Phone}</p>");
            body.AppendLine($"<p><b>Email:</b> {ContactMe.Email}</p>");
            body.AppendLine($"<p><b>Website:</b> {ContactMe.Website}</p>");
            body.AppendLine($"<p><b>Interest:</b> {ContactMe.Interest}</p>");
            body.AppendLine($"<p><b>Questions/Comments:</b></p>");
            body.AppendLine($"<p>{ContactMe.QuestionComments}</p>");
            body.AppendLine($"<hr />");
            body.AppendLine($"<p><b>Submitted On:</b> {ContactMe.CreatedOn}</p>");
            body.AppendLine($"<p><b>IP Address:</b> https://ip-address-lookup-v4.com/ip/{ContactMe.IP_Address}</p>");

            var sendon = DateTime.UtcNow;
            var sendtoname = ContactMe.SendToName ?? "Contact Form Submission";
            var sendtoemail = ContactMe.SendToEmail ?? "";

            // sanitize the input to prevent XSS attacks
            ContactMe.Name = WebUtility.HtmlEncode(ContactMe.Name);
            ContactMe.Company = WebUtility.HtmlEncode(ContactMe.Company);
            ContactMe.Address = WebUtility.HtmlEncode(ContactMe.Address);
            ContactMe.Phone = WebUtility.HtmlEncode(ContactMe.Phone);
            ContactMe.Email = WebUtility.HtmlEncode(ContactMe.Email);
            ContactMe.Website = WebUtility.HtmlEncode(ContactMe.Website);
            ContactMe.QuestionComments = WebUtility.HtmlEncode(ContactMe.QuestionComments);

            ContactMe = _ContactMeRepository.AddContactMe(ContactMe);
            _logger.Log(LogLevel.Information, this, LogFunction.Create, "ContactMe Added {ContactMe}", ContactMe);

            var recordID = ContactMe.ContactMeId; // Assuming ContactMe.ContactMeId is set after adding
            var subject = $"Contact Form Submission - " + recordID.ToString();

            var notification = new Notification(site, sendtoname.ToString(), sendtoemail.ToString(), subject, body.ToString(), sendon);
            _notifications.AddNotification(notification);
            _logger.Log(LogLevel.Information, this, LogFunction.Create, "Notification Added", notification);

            //return ContactMe;
            return await Task.FromResult(ContactMe);
        }

        public Task<Models.ContactMe> UpdateContactMeAsync(Models.ContactMe ContactMe)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, ContactMe.ModuleId, PermissionNames.Edit))
            {
                // sanitize the input to prevent XSS attacks
                ContactMe.Name = WebUtility.HtmlEncode(ContactMe.Name);
                ContactMe.Company = WebUtility.HtmlEncode(ContactMe.Company);
                ContactMe.Address = WebUtility.HtmlEncode(ContactMe.Address);
                ContactMe.Phone = WebUtility.HtmlEncode(ContactMe.Phone);
                ContactMe.Email = WebUtility.HtmlEncode(ContactMe.Email);
                ContactMe.Website = WebUtility.HtmlEncode(ContactMe.Website);
                ContactMe.QuestionComments = WebUtility.HtmlEncode(ContactMe.QuestionComments);

                ContactMe = _ContactMeRepository.UpdateContactMe(ContactMe);
                _logger.Log(LogLevel.Information, this, LogFunction.Update, "ContactMe Updated {ContactMe}", ContactMe);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized ContactMe Update Attempt {ContactMe}", ContactMe);
                ContactMe = null;
            }
            return Task.FromResult(ContactMe);
        }

        public Task DeleteContactMeAsync(int ContactMeId, int ModuleId)
        {
            if (_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, ModuleId, PermissionNames.Edit))
            {
                _ContactMeRepository.DeleteContactMe(ContactMeId);
                _logger.Log(LogLevel.Information, this, LogFunction.Delete, "ContactMe Deleted {ContactMeId}", ContactMeId);
            }
            else
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized ContactMe Delete Attempt {ContactMeId} {ModuleId}", ContactMeId, ModuleId);
            }
            return Task.CompletedTask;
        }

       
    }
}