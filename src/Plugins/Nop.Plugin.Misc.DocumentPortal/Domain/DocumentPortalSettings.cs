using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.DocumentPortal.Domain;

public class DocumentPortalSettings : ISettings
{
    public int DefaultPageSize { get; set; } = 20;
    public bool SearchInContent { get; set; } = false;
    public bool ShowRelatedDocuments { get; set; } = true;
    public bool ShowDownloadCount { get; set; } = true;
    public bool AutoGenerateSlug { get; set; } = true;
    public bool RequireLoginForPrivateDocuments { get; set; } = true;
}
