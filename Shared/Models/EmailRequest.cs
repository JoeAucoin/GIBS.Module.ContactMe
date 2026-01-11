using Oqtane.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIBS.Module.ContactMe.Models
{
    public class EmailRequest
    {
        public string RecipientName { get; set; }
        public string RecipientEmail { get; set; }
        public string BccName { get; set; }
        public string BccEmail { get; set; }
        public string ReplyToName { get; set; }
        public string ReplyToEmail { get; set; }
        public string Subject { get; set; }
        public string HtmlMessage { get; set; }
    }
}
