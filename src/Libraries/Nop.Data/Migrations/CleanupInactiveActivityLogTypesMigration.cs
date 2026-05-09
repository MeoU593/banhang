using FluentMigrator;

namespace Nop.Data.Migrations;

[NopMigration("2026/05/06 12:00:00", "Cleanup inactive activity log types", MigrationProcessType.Update)]
public class CleanupInactiveActivityLogTypesMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        Execute.Sql(@"
            IF OBJECT_ID('ActivityLogType', 'U') IS NOT NULL
            BEGIN
                DELETE FROM [ActivityLog]
                WHERE [ActivityLogTypeId] IN
                (
                    SELECT [Id]
                    FROM [ActivityLogType]
                    WHERE [SystemKeyword] IN
                    (
                        'AddNewCampaign',
                        'AddNewCheckoutAttribute',
                        'AddNewDiscount',
                        'AddNewEmailAccount',
                        'AddNewTopic',
                        'AddNewWarehouse',
                        'AddNewWidget',
                        'AddSubscriptionType',
                        'DeleteCampaign',
                        'DeleteCheckoutAttribute',
                        'DeleteDiscount',
                        'DeleteEmailAccount',
                        'DeleteMessageTemplate',
                        'DeleteOrder',
                        'DeleteReturnRequest',
                        'DeleteSubscriptionType',
                        'DeleteTopic',
                        'DeleteWarehouse',
                        'DeleteWidget',
                        'EditCampaign',
                        'EditCheckoutAttribute',
                        'EditDiscount',
                        'EditEmailAccount',
                        'EditMessageTemplate',
                        'EditOrder',
                        'EditPromotionProviders',
                        'EditReturnRequest',
                        'EditSubscriptionType',
                        'EditTopic',
                        'EditWarehouse',
                        'EditWidget',
                        'PublicStore.AddForumPost',
                        'PublicStore.AddForumTopic',
                        'PublicStore.AddToShoppingCart',
                        'PublicStore.AddToWishlist',
                        'PublicStore.ContactUs',
                        'PublicStore.DeleteForumPost',
                        'PublicStore.DeleteForumTopic',
                        'PublicStore.EditForumPost',
                        'PublicStore.EditForumTopic',
                        'PublicStore.ViewManufacturer'
                    )
                );

                DELETE FROM [ActivityLogType]
                WHERE [SystemKeyword] IN
                (
                    'AddNewCampaign',
                    'AddNewCheckoutAttribute',
                    'AddNewDiscount',
                    'AddNewEmailAccount',
                    'AddNewTopic',
                    'AddNewWarehouse',
                    'AddNewWidget',
                    'AddSubscriptionType',
                    'DeleteCampaign',
                    'DeleteCheckoutAttribute',
                    'DeleteDiscount',
                    'DeleteEmailAccount',
                    'DeleteMessageTemplate',
                    'DeleteOrder',
                    'DeleteReturnRequest',
                    'DeleteSubscriptionType',
                    'DeleteTopic',
                    'DeleteWarehouse',
                    'DeleteWidget',
                    'EditCampaign',
                    'EditCheckoutAttribute',
                    'EditDiscount',
                    'EditEmailAccount',
                    'EditMessageTemplate',
                    'EditOrder',
                    'EditPromotionProviders',
                    'EditReturnRequest',
                    'EditSubscriptionType',
                    'EditTopic',
                    'EditWarehouse',
                    'EditWidget',
                    'PublicStore.AddForumPost',
                    'PublicStore.AddForumTopic',
                    'PublicStore.AddToShoppingCart',
                    'PublicStore.AddToWishlist',
                    'PublicStore.ContactUs',
                    'PublicStore.DeleteForumPost',
                    'PublicStore.DeleteForumTopic',
                    'PublicStore.EditForumPost',
                    'PublicStore.EditForumTopic',
                    'PublicStore.ViewManufacturer'
                );
            END
        ");
    }
}
