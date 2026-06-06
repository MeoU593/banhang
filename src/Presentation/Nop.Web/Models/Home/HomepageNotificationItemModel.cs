using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Home;

public partial record HomepageNotificationItemModel : BaseNopModel
{
    public HomepageNotificationItemModel()
    {
        Entries = new List<HomepageNotificationEntryModel>();
    }

    public string AreaTitle { get; set; }
    public string Badge { get; set; }
    public string EmptyText { get; set; }
    public string Icon { get; set; }
    public string IconColorClass { get; set; }
    public string Url { get; set; }
    public IList<HomepageNotificationEntryModel> Entries { get; set; }
}

public partial record HomepageNotificationEntryModel : BaseNopModel
{
    public string Badge { get; set; }
    public string DateText { get; set; }
    public string Subtitle { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
}
