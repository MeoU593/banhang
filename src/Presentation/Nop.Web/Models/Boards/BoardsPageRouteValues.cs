using Nop.Web.Infrastructure;

namespace Nop.Web.Models.Boards;

public partial record BoardsPageRouteValues : BaseRouteValues
{
    public int? Unread { get; set; }
}
