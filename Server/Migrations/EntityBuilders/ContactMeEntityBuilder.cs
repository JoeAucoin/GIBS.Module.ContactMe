using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Oqtane.Databases.Interfaces;
using Oqtane.Migrations;
using Oqtane.Migrations.EntityBuilders;
using System.Net;
using System.Numerics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace GIBS.Module.ContactMe.Migrations.EntityBuilders
{
    public class ContactMeEntityBuilder : AuditableBaseEntityBuilder<ContactMeEntityBuilder>
    {
        private const string _entityTableName = "GIBSContactMe";
        private readonly PrimaryKey<ContactMeEntityBuilder> _primaryKey = new("PK_GIBSContactMe", x => x.ContactMeId);
        private readonly ForeignKey<ContactMeEntityBuilder> _moduleForeignKey = new("FK_GIBSContactMe_Module", x => x.ModuleId, "Module", "ModuleId", ReferentialAction.Cascade);

        public ContactMeEntityBuilder(MigrationBuilder migrationBuilder, IDatabase database) : base(migrationBuilder, database)
        {
            EntityTableName = _entityTableName;
            PrimaryKey = _primaryKey;
            ForeignKeys.Add(_moduleForeignKey);
        }

        protected override ContactMeEntityBuilder BuildTable(ColumnsBuilder table)
        {
            ContactMeId = AddAutoIncrementColumn(table,"ContactMeId");
            ModuleId = AddIntegerColumn(table,"ModuleId");
            Name = AddMaxStringColumn(table,"Name");
            Company = AddStringColumn(table, "Company", 100, true, true);
            Address = AddMaxStringColumn(table, "Address", true, true);
            Phone = AddStringColumn(table, "Phone", 50, true, true);
            Email = AddStringColumn(table, "Email", 50, true, true);
            Website = AddStringColumn(table, "Website", 100, true, true);
            QuestionComments = AddMaxStringColumn(table, "QuestionComments", true, true);
            Interest = AddMaxStringColumn(table, "Interest", true, true);
            IP_Address = AddStringColumn(table, "IP_Address", 45, true, true);
            AddAuditableColumns(table);
            return this;
        }

        public OperationBuilder<AddColumnOperation> ContactMeId { get; set; }
        public OperationBuilder<AddColumnOperation> ModuleId { get; set; }
        public OperationBuilder<AddColumnOperation> Name { get; set; }
        public OperationBuilder<AddColumnOperation> Company { get; set; }
        public OperationBuilder<AddColumnOperation> Address { get; set; }
        public OperationBuilder<AddColumnOperation> Phone { get; set; }
        public OperationBuilder<AddColumnOperation> Email { get; set; }
        public OperationBuilder<AddColumnOperation> Website { get; set; }
        public OperationBuilder<AddColumnOperation> QuestionComments { get; set; }
        public OperationBuilder<AddColumnOperation> Interest { get; set; }
        public OperationBuilder<AddColumnOperation> IP_Address { get; set; }
    }
}
