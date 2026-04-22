namespace Nop.Core.Domain.Customers;

/// <summary>
/// Represents a customer's military profile
/// </summary>
public partial class CustomerMilitaryProfile : BaseEntity
{
    /// <summary>
    /// Gets or sets the customer identifier
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the military code
    /// </summary>
    public string MilitaryCode { get; set; }

    /// <summary>
    /// Gets or sets the military rank
    /// </summary>
    public string Rank { get; set; }

    /// <summary>
    /// Gets or sets the military unit name
    /// </summary>
    public string UnitName { get; set; }

    /// <summary>
    /// Gets or sets the position title
    /// </summary>
    public string PositionTitle { get; set; }

    /// <summary>
    /// Gets or sets the enlistment date
    /// </summary>
    public DateTime? EnlistmentDate { get; set; }

    /// <summary>
    /// Gets or sets the date and time of creation
    /// </summary>
    public DateTime CreatedOnUtc { get; set; }

    /// <summary>
    /// Gets or sets the date and time of last update
    /// </summary>
    public DateTime UpdatedOnUtc { get; set; }
}
