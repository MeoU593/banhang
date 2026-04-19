using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.DocumentPortal.Models.Admin;

public record ConfigurationModel : BaseNopModel
{
    [NopResourceDisplayName("Plugins.Misc.DocumentPortal.Settings.Fields.DefaultPageSize")]
    public int DefaultPageSize { get; set; }

    [NopResourceDisplayName("Plugins.Misc.DocumentPortal.Settings.Fields.SearchInContent")]
    public bool SearchInContent { get; set; }

    [NopResourceDisplayName("Plugins.Misc.DocumentPortal.Settings.Fields.ShowRelatedDocuments")]
    public bool ShowRelatedDocuments { get; set; }

    [NopResourceDisplayName("Plugins.Misc.DocumentPortal.Settings.Fields.ShowDownloadCount")]
    public bool ShowDownloadCount { get; set; }

    [NopResourceDisplayName("Plugins.Misc.DocumentPortal.Settings.Fields.AutoGenerateSlug")]
    public bool AutoGenerateSlug { get; set; }

    [NopResourceDisplayName("Plugins.Misc.DocumentPortal.Settings.Fields.RequireLoginForPrivateDocuments")]
    public bool RequireLoginForPrivateDocuments { get; set; }
}
