using FluentMigrator;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopUpdateMigration("2026-05-05 10:10:00", "5.00", UpdateMigrationType.Data)]
public class AddUnitContactChatCleanupTaskMigration : Migration
{
    private readonly INopDataProvider _dataProvider;

    public AddUnitContactChatCleanupTaskMigration(INopDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    public override void Up()
    {
        const string sql = @"
IF NOT EXISTS (SELECT 1 FROM [ScheduleTask] WHERE [Type] = 'Nop.Services.Forums.DeleteUnitContactPrivateMessagesTask, Nop.Services')
BEGIN
    INSERT INTO [ScheduleTask] ([Name], [Seconds], [Type], [Enabled], [StopOnError], [LastEnabledUtc])
    VALUES ('Delete unit contact chat messages', 86400, 'Nop.Services.Forums.DeleteUnitContactPrivateMessagesTask, Nop.Services', 1, 0, GETUTCDATE())
END";

        _dataProvider.ExecuteNonQueryAsync(sql).GetAwaiter().GetResult();
    }

    public override void Down()
    {
    }
}
