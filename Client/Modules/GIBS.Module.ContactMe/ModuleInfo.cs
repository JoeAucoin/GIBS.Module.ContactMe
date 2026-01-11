using Oqtane.Models;
using Oqtane.Modules;

namespace GIBS.Module.ContactMe
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "ContactMe",
            Description = "Simple Contact Form",
            Version = "1.0.1",
            ServerManagerType = "GIBS.Module.ContactMe.Manager.ContactMeManager, GIBS.Module.ContactMe.Server.Oqtane",
            ReleaseVersions = "1.0.0,1.0.1",
            Dependencies = "GIBS.Module.ContactMe.Shared.Oqtane",
            PackageName = "GIBS.Module.ContactMe" 
        };
    }
}
