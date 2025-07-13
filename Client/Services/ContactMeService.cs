using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Oqtane.Models;
using Oqtane.Services;
using Oqtane.Shared;

namespace GIBS.Module.ContactMe.Services
{
    public class ContactMeService : ServiceBase, IContactMeService
    {
        public ContactMeService(HttpClient http, SiteState siteState) : base(http, siteState) { }

        private string Apiurl => CreateApiUrl("ContactMe");

        public async Task<List<Models.ContactMe>> GetContactMesAsync(int ModuleId)
        {
            List<Models.ContactMe> ContactMes = await GetJsonAsync<List<Models.ContactMe>>(CreateAuthorizationPolicyUrl($"{Apiurl}?moduleid={ModuleId}", EntityNames.Module, ModuleId), null);
            return ContactMes;
        }

        public async Task<Models.ContactMe> GetContactMeAsync(int ContactMeId, int ModuleId)
        {
            return await GetJsonAsync<Models.ContactMe>(CreateAuthorizationPolicyUrl($"{Apiurl}/{ContactMeId}", EntityNames.Module, ModuleId));
        }

        public async Task<Models.ContactMe> AddContactMeAsync(Models.ContactMe ContactMe)
        {
            return await PostJsonAsync<Models.ContactMe>(CreateAuthorizationPolicyUrl($"{Apiurl}", EntityNames.Module, ContactMe.ModuleId), ContactMe);
        }

        public async Task<Models.ContactMe> UpdateContactMeAsync(Models.ContactMe ContactMe)
        {
            return await PutJsonAsync<Models.ContactMe>(CreateAuthorizationPolicyUrl($"{Apiurl}/{ContactMe.ContactMeId}", EntityNames.Module, ContactMe.ModuleId), ContactMe);
        }

        public async Task DeleteContactMeAsync(int ContactMeId, int ModuleId)
        {
            await DeleteAsync(CreateAuthorizationPolicyUrl($"{Apiurl}/{ContactMeId}", EntityNames.Module, ModuleId));
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await GetJsonAsync<List<User>>($"{Apiurl}/users");
        }
    }
}