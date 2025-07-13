using Microsoft.AspNetCore.Builder; 
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Services;
using GIBS.Module.ContactMe.Repository;
using GIBS.Module.ContactMe.Services;
using Oqtane.Repository;

namespace GIBS.Module.ContactMe.Startup
{
    public class ServerStartup : IServerStartup
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // not implemented
        }

        public void ConfigureMvc(IMvcBuilder mvcBuilder)
        {
            // not implemented
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IContactMeService, ServerContactMeService>();
            services.AddTransient<IContactMeRepository, ContactMeRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddDbContextFactory<ContactMeContext>(opt => { }, ServiceLifetime.Transient);

        }
    }
}
