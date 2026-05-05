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
            name: DocumentPortalDefaults.Routes.EDIT_OWN,
            pattern: "tai-lieu/cua-toi/sua/{id:int}",
            defaults: new { controller = "Document", action = "EditOwn" });

        endpointRouteBuilder.MapControllerRoute(
            name: DocumentPortalDefaults.Routes.DELETE_OWN,
            pattern: "tai-lieu/cua-toi/xoa/{id:int}",
            defaults: new { controller = "Document", action = "DeleteOwn" });

        endpointRouteBuilder.MapControllerRoute(
            name: DocumentPortalDefaults.Routes.DOWNLOAD,
            pattern: "tai-lieu/tai/{id:int}",
            defaults: new { controller = "Document", action = "Download" });

        endpointRouteBuilder.MapControllerRoute(
            name: DocumentPortalDefaults.Routes.PREVIEW,
            pattern: "tai-lieu/xem/{id:int}",
            defaults: new { controller = "Document", action = "Preview" });

        endpointRouteBuilder.MapControllerRoute(
            name: "DocumentPortal.CommentAdd",
            pattern: "tai-lieu/binh-luan/{id:int}",
            defaults: new { controller = "Document", action = "CommentAdd" });

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

        endpointRouteBuilder.MapControllerRoute(
            name: "DocumentPortalCategoryAdmin.List",
            pattern: "Admin/DocumentCategoryAdmin/List",
            defaults: new { controller = "DocumentCategoryAdmin", action = "List", area = AreaNames.ADMIN });

        endpointRouteBuilder.MapControllerRoute(
            name: "DocumentPortalTypeAdmin.List",
            pattern: "Admin/DocumentTypeAdmin/List",
            defaults: new { controller = "DocumentTypeAdmin", action = "List", area = AreaNames.ADMIN });

        endpointRouteBuilder.MapControllerRoute(
            name: "DocumentPortalIssuerAdmin.List",
            pattern: "Admin/DocumentIssuerAdmin/List",
            defaults: new { controller = "DocumentIssuerAdmin", action = "List", area = AreaNames.ADMIN });

    }
}
