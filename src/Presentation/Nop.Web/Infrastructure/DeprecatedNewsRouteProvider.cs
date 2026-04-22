using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Web.Infrastructure;

/// <summary>
/// Blocks deprecated public news URLs after merging into blog.
/// </summary>
public class DeprecatedNewsRouteProvider : BaseRouteProvider, IRouteProvider
{
    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        var lang = GetLanguageRoutePattern();

        endpointRouteBuilder.MapControllerRoute(name: "DeprecatedNewsArchive",
            pattern: $"{lang}/news",
            defaults: new { controller = "Common", action = "PageNotFound" });

        endpointRouteBuilder.MapControllerRoute(name: "DeprecatedNewsArchiveCatchAll",
            pattern: $"{lang}/news/{{*path}}",
            defaults: new { controller = "Common", action = "PageNotFound" });
    }

    public int Priority => 10000;
}
