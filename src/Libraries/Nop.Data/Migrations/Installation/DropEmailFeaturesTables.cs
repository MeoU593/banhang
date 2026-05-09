using FluentMigrator;

namespace Nop.Data.Migrations.Installation;

[NopSchemaMigration("2026/05/06 00:00:00:0000000", "Drop email features tables", MigrationProcessType.Installation)]
public class DropEmailFeaturesTables : ForwardOnlyMigration
{
    #region Methods

    public override void Up()
    {
        //Drop foreign keys first before dropping tables
        Execute.Sql(@"
            IF OBJECT_ID('FK_QueuedEmail_EmailAccount', 'F') IS NOT NULL
                ALTER TABLE [QueuedEmail] DROP CONSTRAINT [FK_QueuedEmail_EmailAccount];
        ");

        //Drop email-related tables
        if (Schema.Table("QueuedEmail").Exists())
            Delete.Table("QueuedEmail");

        if (Schema.Table("MessageTemplate").Exists())
            Delete.Table("MessageTemplate");

        if (Schema.Table("Campaign").Exists())
            Delete.Table("Campaign");

        if (Schema.Table("CampaignAffiliatePercentage").Exists())
            Delete.Table("CampaignAffiliatePercentage");

        if (Schema.Table("EmailAccount").Exists())
            Delete.Table("EmailAccount");

        if (Schema.Table("NewsLetterSubscription").Exists())
            Delete.Table("NewsLetterSubscription");

        if (Schema.Table("NewsLetterSubscriptionType").Exists())
            Delete.Table("NewsLetterSubscriptionType");
    }

    #endregion
}
