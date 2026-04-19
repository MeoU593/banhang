using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.DocumentPortal.Factories;
using Nop.Plugin.Misc.DocumentPortal.Services;

namespace Nop.Plugin.Misc.DocumentPortal.Infrastructure;

public class NopStartup : INopStartup
{
    public int Order => 3000;

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IDocumentPortalService, DocumentPortalService>();
        services.AddScoped<IDocumentPortalModelFactory, DocumentPortalModelFactory>();
    }

    public void Configure(IApplicationBuilder application)
    {
    }
}
