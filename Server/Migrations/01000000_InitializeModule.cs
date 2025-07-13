using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using GIBS.Module.ContactMe.Migrations.EntityBuilders;
using GIBS.Module.ContactMe.Repository;

namespace GIBS.Module.ContactMe.Migrations
{
    [DbContext(typeof(ContactMeContext))]
    [Migration("GIBS.Module.ContactMe.01.00.00.00")]
    public class InitializeModule : MultiDatabaseMigration
    {
        public InitializeModule(IDatabase database) : base(database)
        {
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var entityBuilder = new ContactMeEntityBuilder(migrationBuilder, ActiveDatabase);
            entityBuilder.Create();
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            var entityBuilder = new ContactMeEntityBuilder(migrationBuilder, ActiveDatabase);
            entityBuilder.Drop();
        }
    }
}
