namespace Nop.Web.Models.Guide;

public partial record UserGuideModel
{
    public string TopicSystemName { get; set; } = "UserGuide";
    public string CustomTitle { get; set; } = string.Empty;
    public string CustomBody { get; set; } = string.Empty;
    public IList<UserGuideSectionModel> Sections { get; set; } = new List<UserGuideSectionModel>();

    public bool HasCustomBody => !string.IsNullOrWhiteSpace(CustomBody);
}

public partial record UserGuideSectionModel
{
    public string Anchor { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string IconColorClass { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IList<string> Steps { get; set; } = new List<string>();
}
