using FluentMigrator;

namespace Nop.Data.Migrations;

[NopMigration("2026/05/06 12:30:00", "Cleanup obsolete email schedule tasks", MigrationProcessType.Update)]
public class CleanupObsoleteEmailScheduleTasksMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        Execute.Sql(@"
            IF OBJECT_ID('ScheduleTask', 'U') IS NOT NULL
            BEGIN
                DELETE FROM [ScheduleTask]
                WHERE [Type] = 'Nop.Services.Messages.QueuedMessagesSendTask, Nop.Services'
                   OR [Type] LIKE '%QueuedMessagesSendTask%'
                   OR [Name] = N'Gửi email';
            END
        ");
    }
}
