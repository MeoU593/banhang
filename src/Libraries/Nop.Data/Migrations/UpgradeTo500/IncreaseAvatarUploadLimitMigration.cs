using FluentMigrator;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopUpdateMigration("2026-05-05 10:20:00", "5.00", UpdateMigrationType.Data)]
public class IncreaseAvatarUploadLimitMigration : Migration
{
    private readonly INopDataProvider _dataProvider;

    public IncreaseAvatarUploadLimitMigration(INopDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    public override void Up()
    {
        const string sql = @"
IF EXISTS (SELECT 1 FROM [Setting] WHERE LOWER([Name]) = 'customersettings.avatarmaximumsizebytes' AND [StoreId] = 0)
BEGIN
    UPDATE [Setting]
    SET [Value] = '5242880'
    WHERE LOWER([Name]) = 'customersettings.avatarmaximumsizebytes'
      AND [StoreId] = 0
END
ELSE
BEGIN
    INSERT INTO [Setting] ([Name], [Value], [StoreId])
    VALUES ('customersettings.avatarmaximumsizebytes', '5242880', 0)
END";

        _dataProvider.ExecuteNonQueryAsync(sql).GetAwaiter().GetResult();
    }

    public override void Down()
    {
    }
}
