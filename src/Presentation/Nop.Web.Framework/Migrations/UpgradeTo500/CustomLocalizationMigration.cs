using System.Collections.Generic;
using FluentMigrator;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Web.Framework.Extensions;

namespace Nop.Web.Framework.Migrations.UpgradeTo500;

[NopUpdateMigration("2026-04-17 10:00:00", "5.00.1", UpdateMigrationType.Localization)]
public class CustomLocalizationMigration : MigrationBase
{
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        this.AddOrUpdateLocaleResource(new Dictionary<string, string>
        {
            ["Admin.Common.Export"] = "Xuất",
            ["Account.Gdpr.Export.Button"] = "Xuất"
        });
    }

    public override void Down()
    {
    }
}
