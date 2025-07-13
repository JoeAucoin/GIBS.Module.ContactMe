using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Oqtane.Modules;
using Oqtane.Models;
using Oqtane.Infrastructure;
using Oqtane.Interfaces;
using Oqtane.Enums;
using Oqtane.Repository;
using GIBS.Module.ContactMe.Repository;
using System.Threading.Tasks;

namespace GIBS.Module.ContactMe.Manager
{
    public class ContactMeManager : MigratableModuleBase, IInstallable, IPortable, ISearchable
    {
        private readonly IContactMeRepository _ContactMeRepository;
        private readonly IDBContextDependencies _DBContextDependencies;

        public ContactMeManager(IContactMeRepository ContactMeRepository, IDBContextDependencies DBContextDependencies)
        {
            _ContactMeRepository = ContactMeRepository;
            _DBContextDependencies = DBContextDependencies;
        }

        public bool Install(Tenant tenant, string version)
        {
            return Migrate(new ContactMeContext(_DBContextDependencies), tenant, MigrationType.Up);
        }

        public bool Uninstall(Tenant tenant)
        {
            return Migrate(new ContactMeContext(_DBContextDependencies), tenant, MigrationType.Down);
        }

        public string ExportModule(Oqtane.Models.Module module)
        {
            string content = "";
            List<Models.ContactMe> ContactMes = _ContactMeRepository.GetContactMes(module.ModuleId).ToList();
            if (ContactMes != null)
            {
                content = JsonSerializer.Serialize(ContactMes);
            }
            return content;
        }

        public void ImportModule(Oqtane.Models.Module module, string content, string version)
        {
            List<Models.ContactMe> ContactMes = null;
            if (!string.IsNullOrEmpty(content))
            {
                ContactMes = JsonSerializer.Deserialize<List<Models.ContactMe>>(content);
            }
            if (ContactMes != null)
            {
                foreach(var ContactMe in ContactMes)
                {
                    _ContactMeRepository.AddContactMe(new Models.ContactMe { ModuleId = module.ModuleId, Name = ContactMe.Name });
                }
            }
        }

        public Task<List<SearchContent>> GetSearchContentsAsync(PageModule pageModule, DateTime lastIndexedOn)
        {
           var searchContentList = new List<SearchContent>();

           foreach (var ContactMe in _ContactMeRepository.GetContactMes(pageModule.ModuleId))
           {
               if (ContactMe.ModifiedOn >= lastIndexedOn)
               {
                   searchContentList.Add(new SearchContent
                   {
                       EntityName = "GIBSContactMe",
                       EntityId = ContactMe.ContactMeId.ToString(),
                       Title = ContactMe.Name,
                       Body = ContactMe.Name,
                       ContentModifiedBy = ContactMe.ModifiedBy,
                       ContentModifiedOn = ContactMe.ModifiedOn
                   });
               }
           }

           return Task.FromResult(searchContentList);
        }
    }
}
