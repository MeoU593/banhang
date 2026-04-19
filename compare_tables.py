import sys, io, os, subprocess, re
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding='utf-8', errors='replace')

builders_dir = r'D:\thayhai\banhang\src\Libraries\Nop.Data\Mapping\Builders'

# Collect all builder names
builders = []
for root, dirs, files in os.walk(builders_dir):
    for f in files:
        if f.endswith('Builder.cs') and f not in ('IEntityBuilder.cs', 'NopEntityBuilder.cs'):
            builders.append(f.replace('Builder.cs', ''))

builders.sort()

# DB table names (hardcoded from query)
dbtables = set([
    'AclRecord','ActivityLog','ActivityLogType','Address','AddressAttribute','AddressAttributeValue',
    'Affiliate','BlogComment','BlogPost','BlogPostAttachment','Category','CategoryTemplate',
    'ChatConversation','ChatMessage','CheckoutAttribute','CheckoutAttributeValue','Country',
    'CrossSellProduct','Currency','Customer','Customer_CustomerRole_Mapping','CustomerAddresses',
    'CustomerAttribute','CustomerAttributeValue','CustomerPassword','CustomerRole','CustomWishlist',
    'DeliveryDate','Discount','Discount_AppliedToCategories','Discount_AppliedToManufacturers',
    'Discount_AppliedToProducts','DiscountRequirement','DiscountUsageHistory','Download',
    'EmailAccount','ExternalAuthenticationRecord','FilterLevelValue','FilterLevelValueProductMapping',
    'ForumPostAttachment','Forums_Forum','Forums_Group','Forums_Post','Forums_PostVote',
    'Forums_Subscription','Forums_Topic','GenericAttribute','Language','LocaleStringResource',
    'LocalizedProperty','Log','Manufacturer','ManufacturerTemplate','MeasureDimension','MeasureWeight',
    'Menu','MenuItem','MessageTemplate','MigrationVersionInfo','NewsLetterSubscription',
    'NewsLetterSubscriptionType','Order','OrderItem','OrderNote','PermissionRecord',
    'PermissionRecord_Role_Mapping','Picture','PictureBinary','PredefinedProductAttributeValue',
    'Product','Product_Category_Mapping','Product_Manufacturer_Mapping','Product_Picture_Mapping',
    'Product_ProductAttribute_Mapping','Product_ProductTag_Mapping','Product_SpecificationAttribute_Mapping',
    'ProductAttribute','ProductAttributeCombination','ProductAttributeCombinationPicture',
    'ProductAttributeValue','ProductAttributeValuePicture','ProductAvailabilityRange','ProductReview',
    'ProductReviewHelpfulness','ProductReviewReviewTypeMapping','ProductTag','ProductTemplate',
    'ProductVideo','ProductWarehouseInventory','QueuedEmail','RelatedProduct','ReviewType',
    'ScheduleTask','SearchTerm','Setting','Shipment','ShipmentItem','ShippingByWeightByTotalRecord',
    'ShippingMethod','ShippingMethodRestrictions','ShoppingCartItem','SpecificationAttribute',
    'SpecificationAttributeGroup','SpecificationAttributeOption','StateProvince','StockQuantityHistory',
    'Store','StoreMapping','StorePickupPoint','TaxCategory','TierPrice','Topic','TopicTemplate',
    'UrlRecord','Vendor','VendorAttribute','VendorAttributeValue','VendorEmployeePermission',
    'VendorNote','Video','Warehouse',
])

# Name mapping: builder name -> DB table name
name_map = {
    'CustomerAddressMapping': 'CustomerAddresses',
    'CustomerCustomerRoleMapping': 'Customer_CustomerRole_Mapping',
    'PermissionRecordCustomerRoleMapping': 'PermissionRecord_Role_Mapping',
    'DiscountCategoryMapping': 'Discount_AppliedToCategories',
    'DiscountManufacturerMapping': 'Discount_AppliedToManufacturers',
    'DiscountProductMapping': 'Discount_AppliedToProducts',
    'ProductCategory': 'Product_Category_Mapping',
    'ProductManufacturer': 'Product_Manufacturer_Mapping',
    'ProductPicture': 'Product_Picture_Mapping',
    'ProductAttributeMapping': 'Product_ProductAttribute_Mapping',
    'ProductProductTagMapping': 'Product_ProductTag_Mapping',
    'ProductSpecificationAttribute': 'Product_SpecificationAttribute_Mapping',
    'ShippingMethodCountryMapping': 'ShippingMethodRestrictions',
    'Forum': 'Forums_Forum',
    'ForumGroup': 'Forums_Group',
    'ForumPost': 'Forums_Post',
    'ForumPostVote': 'Forums_PostVote',
    'ForumSubscription': 'Forums_Subscription',
    'ForumTopic': 'Forums_Topic',
}

missing = []
for b in builders:
    db_name = name_map.get(b, b)
    if db_name not in dbtables:
        missing.append((b, db_name))

print(f"Total builders: {len(builders)}")
print(f"Total DB tables: {len(dbtables)}")
print(f"\nMISSING TABLES ({len(missing)}):")
for b, db in missing:
    print(f"  {b} -> {db}")

# Also find DB tables NOT in builders (extra/custom tables)
all_mapped = set(name_map.get(b, b) for b in builders)
extra = [t for t in dbtables if t not in all_mapped and t not in (
    'MigrationVersionInfo','SearchTerm',  # system tables
    'BlogPostAttachment','ForumPostAttachment',  # custom
    'ChatConversation','ChatMessage',  # custom
    'VendorEmployeePermission',  # custom
    'CustomWishlist',  # custom (has builder)
    'FilterLevelValue','FilterLevelValueProductMapping',  # custom (has builder)
    'Menu','MenuItem',  # custom (has builder)
    'ExternalAuthenticationRecord',  # social auth
    'ShippingByWeightByTotalRecord',  # plugin
    'StorePickupPoint',  # plugin
)]
if extra:
    print(f"\nDB tables NOT covered by any builder (may be extra): {extra}")
