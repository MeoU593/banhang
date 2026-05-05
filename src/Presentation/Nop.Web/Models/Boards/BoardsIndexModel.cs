using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Boards;

public partial record BoardsIndexModel : BaseNopModel
{
    public BoardsIndexModel()
    {
        ForumGroups = new List<ForumGroupModel>();
        TopicCards = new List<ForumTopicCardModel>();
        FeaturedTopicCards = new List<ForumTopicCardModel>();
        PopularForums = new List<ForumRowModel>();
    }

    public IList<ForumGroupModel> ForumGroups { get; set; }

    public IList<ForumTopicCardModel> TopicCards { get; set; }

    public IList<ForumTopicCardModel> FeaturedTopicCards { get; set; }

    public IList<ForumRowModel> PopularForums { get; set; }

    public int TotalForums { get; set; }

    public int TotalTopics { get; set; }

    public int TotalPosts { get; set; }
}
