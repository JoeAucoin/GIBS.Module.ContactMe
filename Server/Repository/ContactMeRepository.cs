using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Oqtane.Modules;

namespace GIBS.Module.ContactMe.Repository
{
    public class ContactMeRepository : IContactMeRepository, ITransientService
    {
        private readonly IDbContextFactory<ContactMeContext> _factory;

        public ContactMeRepository(IDbContextFactory<ContactMeContext> factory)
        {
            _factory = factory;
        }

        public IEnumerable<Models.ContactMe> GetContactMes(int ModuleId)
        {
            using var db = _factory.CreateDbContext();
            return db.ContactMe.Where(item => item.ModuleId == ModuleId).AsNoTracking().OrderByDescending(item => item.CreatedOn).ToList();
        }

        public Models.ContactMe GetContactMe(int ContactMeId)
        {
            using var db = _factory.CreateDbContext();
            return db.ContactMe.Find(ContactMeId);
        }

        public Models.ContactMe GetContactMe(int ContactMeId, bool tracking)
        {
            using var db = _factory.CreateDbContext();
            if (tracking)
            {
                return db.ContactMe.FirstOrDefault(item => item.ContactMeId == ContactMeId);
            }
            else
            {
                return db.ContactMe.AsNoTracking().FirstOrDefault(item => item.ContactMeId == ContactMeId);
            }
        }

        public Models.ContactMe AddContactMe(Models.ContactMe ContactMe)
        {
            using var db = _factory.CreateDbContext();
            db.ContactMe.Add(ContactMe);
            db.SaveChanges();
            return ContactMe;
        }

        public Models.ContactMe UpdateContactMe(Models.ContactMe ContactMe)
        {
            using var db = _factory.CreateDbContext();
            db.Entry(ContactMe).State = EntityState.Modified;
            db.SaveChanges();
            return ContactMe;
        }

        public void DeleteContactMe(int ContactMeId)
        {
            using var db = _factory.CreateDbContext();
            Models.ContactMe ContactMe = db.ContactMe.Find(ContactMeId);
            db.ContactMe.Remove(ContactMe);
            db.SaveChanges();
        }
    }
}