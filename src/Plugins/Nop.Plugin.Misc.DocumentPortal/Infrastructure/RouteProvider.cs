using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Routing;
using Nop.Web.Infrastructure;

namespace Nop.Plugin.Misc.DocumentPortal.Infrastructure;

public class RouteProvider : BaseRouteProvider, IRouteProvider
{
    public int Priority => 0;

    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        endpointRouteBuilder.MapControllerRoute(
            name: DocumentPortalDefaults.Routes.UPLOAD,
            pattern: "tai-lieu/upload",
            defaults: new { controller = "Document", action = "Upload" });

        endpointRouteBuilder.MapControllerRoute(
            name: DocumentPortalDefaults.Routes.DELETE,
            pattern: "tai-lieu/xoa/{id:int}",
            defaults: new { controller = "Document", action = "Delete" });

        endpointRouteBuilder.MapControllerRoute(
            name: DocumentPortalDefaults.Routes.DOWNLOAD,
            pattern: "tai-lieu/tai/{id:int}",
            defaults: new { controller = "Document", action = "Download" });

        endpointRouteBuilder.MapControllerRoute(
            name: DocumentPortalDefaults.Routes.SEARCH,
            pattern: "tai-lieu/tim-kiem",
            defaults: new { controller = "Document", action = "List" });

        endpointRouteBuilder.MapControllerRoute(
            name: DocumentPortalDefaults.Routes.LIST,
            pattern: "tai-lieu",
            defaults: new { controller = "Document", action = "List" });

        endpointRouteBuilder.MapControllerRoute(
            name: DocumentPortalDefaults.Routes.DETAIL,
            pattern: "tai-lieu/{slug}",
            defaults: new { controller = "Document", action = "Detail" });

        // Admin routes
        endpointRouteBuilder.MapControllerRoute(
            name: "DocumentPortalAdmin.List",
            pattern: "Admin/DocumentPortalAdmin/List",
            defaults: new { controller = "DocumentPortalAdmin", action = "List", area = AreaNames.ADMIN });

        endpointRouteBuilder.MapControllerRoute(
            name: "DocumentPortalAdmin.Settings",
            pattern: "Admin/DocumentPortalAdmin/Settings",
            defaults: new { controller = "DocumentPortalAdmin", action = "Settings", area = AreaNames.ADMIN });

        // Vendor routes
        endpointRouteBuilder.MapControllerRoute(
            name: DocumentPortalDefaults.Routes.VENDOR_LIST,
            pattern: "Admin/DocumentPortalVendor/List",
            defaults: new { controller = "DocumentPortalVendor", action = "List", area = AreaNames.ADMIN });

        endpointRouteBuilder.MapControllerRoute(
            name: DocumentPortalDefaults.Routes.VENDOR_DELETE,
            pattern: "Admin/DocumentPortalVendor/Delete/{id:int}",
            defaults: new { controller = "DocumentPortalVendor", action = "Delete", area = AreaNames.ADMIN });
    }
}
