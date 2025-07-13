using Microsoft.Extensions.DependencyInjection;
using Oqtane.Services;
using GIBS.Module.ContactMe.Services;

namespace GIBS.Module.ContactMe.Startup
{
    public class ClientStartup : IClientStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IContactMeService, ContactMeService>();
        }
    }
}
