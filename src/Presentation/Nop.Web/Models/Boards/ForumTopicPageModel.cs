using Nop.Core.Domain.Forums;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Boards;

public partial record ForumTopicPageModel : BaseNopModel
{
    public ForumTopicPageModel()
    {
        ForumPostModels = new List<ForumPostModel>();
        RelatedTopics = new List<ForumTopicCardModel>();
    }

    public int Id { get; set; }
    public string Subject { get; set; }
    public string SeName { get; set; }

    public string WatchTopicText { get; set; }

    public bool IsCustomerAllowedToEditTopic { get; set; }
    public bool IsCustomerAllowedToDeleteTopic { get; set; }
    public bool IsCustomerAllowedToMoveTopic { get; set; }
    public bool IsCustomerAllowedToSubscribe { get; set; }
    public bool IsCustomerAllowedToCreatePost { get; set; }

    public IList<ForumPostModel> ForumPostModels { get; set; }
    public EditorType ForumEditor { get; set; }
    public int PostsPageIndex { get; set; }
    public int PostsPageSize { get; set; }
    public int PostsTotalRecords { get; set; }

    public string MetaDescription { get; set; }
    public string MetaTitle { get; set; }

    public string JsonLd { get; set; }

    public int MainPostId { get; set; }

    public int ForumId { get; set; }

    public string ForumName { get; set; }

    public string ForumSeName { get; set; }

    public IList<ForumTopicCardModel> RelatedTopics { get; set; }
}
