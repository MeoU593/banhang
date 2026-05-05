using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Boards;

public partial record ForumTopicCardModel : BaseNopModel
{
    public int Id { get; set; }

    public string Subject { get; set; }

    public string SeName { get; set; }

    public int ForumId { get; set; }

    public string ForumName { get; set; }

    public string ForumSeName { get; set; }

    public int CustomerId { get; set; }

    public bool AllowViewingProfiles { get; set; }

    public string CustomerName { get; set; }

    public string CustomerAvatarUrl { get; set; }

    public string Rank { get; set; }

    public string UnitName { get; set; }

    public string CreatedOnStr { get; set; }

    public int Views { get; set; }

    public int NumReplies { get; set; }

    public string Excerpt { get; set; }
}
