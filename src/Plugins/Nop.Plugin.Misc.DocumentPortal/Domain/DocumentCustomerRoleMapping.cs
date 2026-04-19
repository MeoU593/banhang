using Nop.Core;

namespace Nop.Plugin.Misc.DocumentPortal.Domain;

public class DocumentCustomerRoleMapping : BaseEntity
{
    public int DocumentId { get; set; }
    public int CustomerRoleId { get; set; }
}
