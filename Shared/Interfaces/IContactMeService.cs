using Oqtane.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GIBS.Module.ContactMe.Services
{
    public interface IContactMeService 
    {
        Task<List<Models.ContactMe>> GetContactMesAsync(int ModuleId);

        Task<Models.ContactMe> GetContactMeAsync(int ContactMeId, int ModuleId);

        Task<Models.ContactMe> AddContactMeAsync(Models.ContactMe ContactMe);

        Task<Models.ContactMe> UpdateContactMeAsync(Models.ContactMe ContactMe);

        Task DeleteContactMeAsync(int ContactMeId, int ModuleId);

        Task<List<User>> GetUsersAsync();
    }
}
