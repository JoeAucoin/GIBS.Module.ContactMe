using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Oqtane.Modules;
using Oqtane.Repository;
using Oqtane.Infrastructure;
using Oqtane.Repository.Databases.Interfaces;

namespace GIBS.Module.ContactMe.Repository
{
    public class ContactMeContext : DBContextBase, ITransientService, IMultiDatabase
    {
        public virtual DbSet<Models.ContactMe> ContactMe { get; set; }

        public ContactMeContext(IDBContextDependencies DBContextDependencies) : base(DBContextDependencies)
        {
            // ContextBase handles multi-tenant database connections
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Models.ContactMe>().ToTable(ActiveDatabase.RewriteName("GIBSContactMe"));
        }
    }
}
