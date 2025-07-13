using System.Collections.Generic;
using System.Threading.Tasks;

namespace GIBS.Module.ContactMe.Repository
{
    public interface IContactMeRepository
    {
        IEnumerable<Models.ContactMe> GetContactMes(int ModuleId);
        Models.ContactMe GetContactMe(int ContactMeId);
        Models.ContactMe GetContactMe(int ContactMeId, bool tracking);
        Models.ContactMe AddContactMe(Models.ContactMe ContactMe);
        Models.ContactMe UpdateContactMe(Models.ContactMe ContactMe);
        void DeleteContactMe(int ContactMeId);
    }
}
