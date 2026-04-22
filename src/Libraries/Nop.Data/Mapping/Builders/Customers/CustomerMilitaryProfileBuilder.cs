using System.Data;
using FluentMigrator.Builders.Create.Table;
using Nop.Core.Domain.Customers;
using Nop.Data.Extensions;

namespace Nop.Data.Mapping.Builders.Customers;

/// <summary>
/// Represents a customer military profile entity builder
/// </summary>
public partial class CustomerMilitaryProfileBuilder : NopEntityBuilder<CustomerMilitaryProfile>
{
    #region Methods

    /// <summary>
    /// Apply entity configuration
    /// </summary>
    /// <param name="table">Create table expression builder</param>
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(CustomerMilitaryProfile.CustomerId)).AsInt32().NotNullable().ForeignKey<Customer>(onDelete: Rule.Cascade)
            .WithColumn(nameof(CustomerMilitaryProfile.MilitaryCode)).AsString(100).NotNullable()
            .WithColumn(nameof(CustomerMilitaryProfile.Rank)).AsString(255).Nullable()
            .WithColumn(nameof(CustomerMilitaryProfile.UnitName)).AsString(500).Nullable()
            .WithColumn(nameof(CustomerMilitaryProfile.PositionTitle)).AsString(255).Nullable()
            .WithColumn(nameof(CustomerMilitaryProfile.EnlistmentDate)).AsDateTime2().Nullable();
    }

    #endregion
}
