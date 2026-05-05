using FluentMigrator;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopUpdateMigration("2026-04-27 16:30:00", "5.00", UpdateMigrationType.Data)]
public class BackfillProductCreatedByCustomerDataMigration : Migration
{
    private readonly INopDataProvider _dataProvider;

    public BackfillProductCreatedByCustomerDataMigration(INopDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    public override void Up()
    {
        const string sql = @"
IF COL_LENGTH('Product', 'CreatedByCustomerId') IS NULL
    RETURN;

;WITH LogCandidates AS
(
    SELECT
        p.Id AS ProductId,
        al.CustomerId,
        ROW_NUMBER() OVER (PARTITION BY p.Id ORDER BY al.CreatedOnUtc ASC, al.Id ASC) AS RowNum
    FROM [Product] p
    INNER JOIN [ActivityLog] al ON al.EntityId = p.Id AND al.EntityName = 'Product'
    INNER JOIN [Customer] c ON c.Id = al.CustomerId
    WHERE ISNULL(p.CreatedByCustomerId, 0) = 0
      AND p.VendorId > 0
      AND c.Active = 1
      AND c.Deleted = 0
      AND c.VendorId = p.VendorId
)
UPDATE p
SET p.CreatedByCustomerId = lc.CustomerId
FROM [Product] p
INNER JOIN LogCandidates lc ON lc.ProductId = p.Id AND lc.RowNum = 1
WHERE ISNULL(p.CreatedByCustomerId, 0) = 0;

UPDATE p
SET p.CreatedByCustomerId = fallbackCustomer.Id
FROM [Product] p
CROSS APPLY
(
    SELECT TOP 1 c.Id
    FROM [Customer] c
    WHERE c.Active = 1
      AND c.Deleted = 0
      AND c.VendorId = p.VendorId
    ORDER BY NEWID()
) fallbackCustomer
WHERE ISNULL(p.CreatedByCustomerId, 0) = 0
  AND p.VendorId > 0;

UPDATE p
SET p.CreatedByCustomerId = anyCustomer.Id
FROM [Product] p
CROSS APPLY
(
    SELECT TOP 1 c.Id
    FROM [Customer] c
    WHERE c.Active = 1
      AND c.Deleted = 0
    ORDER BY NEWID()
) anyCustomer
WHERE ISNULL(p.CreatedByCustomerId, 0) = 0;
";

        _dataProvider.ExecuteNonQueryAsync(sql).GetAwaiter().GetResult();
    }

    public override void Down()
    {
    }
}
