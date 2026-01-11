using GIBS.Module.ContactMe.Models;
using GIBS.Module.ContactMe.Repository;
using GIBS.Module.ContactMe.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using MimeKit;
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
using System.Reflection;
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
        private readonly IUserRepository _userRepository;
        private readonly ISettingRepository _settings;


        public ServerContactMeService(IContactMeRepository ContactMeRepository, INotificationRepository notifications, IUserPermissions userPermissions, ITenantManager tenantManager, ILogManager logger, IHttpContextAccessor accessor, ISettingRepository settingRepository, IModuleRepository moduleRepository, ISiteRepository siteRepository, IUserRepository userRepository, ISettingRepository settings)
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
            _settings = settings;

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

            var site = _alias.SiteId; //
            var request = _accessor.HttpContext.Request;
            var mySite = $"{request.Scheme}://{request.Host}{request.PathBase}";

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
            body.AppendLine($"<p><b>Site:</b> " + mySite.ToString() + "</p>");

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

           

          //  _logger.Log(LogLevel.Information, this, LogFunction.Create, "ContactMe Added {ContactMe}", ContactMe);
           
            var recordID = ContactMe.ContactMeId; // Assuming ContactMe.ContactMeId is set after adding
            var subject = $"Contact Form Submission - " + recordID.ToString();

            // Get Module Settings
            var modulesettings = _settings.GetSettings("Module", ContactMe.ModuleId).ToDictionary(s => s.SettingName, s => s.SettingValue);
            var bccName = modulesettings.GetValueOrDefault("BccName", "");
            var bccEmail = modulesettings.GetValueOrDefault("BccEmail", "");

            await SendHtmlEmailAsync(sendtoname, sendtoemail, bccName, bccEmail, ContactMe.Name, ContactMe.Email, subject, body.ToString());

            var notification = new Notification(site, sendtoname.ToString(), sendtoemail.ToString(), subject, body.ToString(), sendon);
            _notifications.AddNotification(notification);
           // _logger.Log(LogLevel.Information, this, LogFunction.Create, "Notification Added", notification);

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

        public async Task SendHtmlEmailAsync(string recipientName, string recipientEmail, string bccName, string bccEmail, string replyToName, string replyToEmail, string subject, string htmlMessage)
        {
           

            // Retrieve Site Settings

            // var settings = _settings.GetSettings(EntityNames.Site, _alias.SiteId).ToList();
            var settings = _settings.GetSettings(EntityNames.Site, _alias.SiteId, EntityNames.Host, -1).ToList();


            string GetSetting(string key, string defaultValue) =>
                settings.FirstOrDefault(s => s.SettingName == key)?.SettingValue ?? defaultValue;

            string smtpHost = GetSetting("SMTPHost", "");
            int smtpPort = int.Parse(GetSetting("SMTPPort", "587"));
            string smtpUserName = GetSetting("SMTPUsername", "");
            string smtpPassword = GetSetting("SMTPPassword", "");
            string smtpSender = GetSetting("SMTPSender", smtpUserName);
            string smtpSSL = GetSetting("SMTPSSL", "false"); // Oqtane often has this setting

            _logger.Log(LogLevel.Information, this, LogFunction.Create, "SMTP Settings: Host={Host}, Port={Port}, User={User}, Password={Password}, Sender={Sender}, SSL={SSL}", smtpHost, smtpPort, smtpUserName, smtpPassword, smtpSender, smtpSSL);


            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Webmaster", smtpSender));
            message.To.Add(new MailboxAddress(recipientName, recipientEmail));
            message.ReplyTo.Add(new MailboxAddress(replyToName, replyToEmail));

            if (!string.IsNullOrEmpty(bccEmail))
            {
                message.Bcc.Add(new MailboxAddress(bccName, bccEmail));
            }

            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlMessage,
                TextBody = "Please view this email in a client that supports HTML."
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            client.CheckCertificateRevocation = false;

            // Connect
            await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.Auto);

            // Authenticate
            if (!string.IsNullOrEmpty(smtpUserName) && !string.IsNullOrEmpty(smtpPassword))
            {
                await client.AuthenticateAsync(smtpUserName, smtpPassword);
            }

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }


    }
}