using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Oqtane.Models;

namespace GIBS.Module.ContactMe.Models
{
    [Table("GIBSContactMe")]
    public class ContactMe : IAuditable
    {
        [Key]
        public int ContactMeId { get; set; }
        public int ModuleId { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string QuestionComments { get; set; }
        public string Interest { get; set; }
        public string IP_Address { get; set; }

        [NotMapped]
        public string Fax { get; set; } // Honeypot field

        [NotMapped]
        public string SendToEmail { get; set; } // Honeypot field
        [NotMapped]
        public string SendToName { get; set; } // Honeypot field

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}
