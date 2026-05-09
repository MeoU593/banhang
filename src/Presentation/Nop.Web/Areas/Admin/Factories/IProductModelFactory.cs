namespace Nop.Web.Areas.Admin.Factories;

public partial interface IProductModelFactory
{
    Task<object> PrepareProductSearchModelAsync(object model);
    Task<object> PrepareProductListModelAsync(object model);
    Task<object> PrepareProductModelAsync(object model, object product, object copy = null);
    Task<object> PrepareAddRequiredProductSearchModelAsync(object model);
    Task<object> PrepareAddRequiredProductListModelAsync(object model);
    Task<object> PrepareAddRequiredProductListModelAsync(object model, object product);
    Task<object> PrepareAddRelatedProductSearchModelAsync(object model);
    Task<object> PrepareAddRelatedProductListModelAsync(object model);
    Task<object> PrepareRelatedProductListModelAsync(object model, object product);
    Task<object> PrepareAddCrossSellProductSearchModelAsync(object model);
    Task<object> PrepareAddCrossSellProductListModelAsync(object model);
    Task<object> PrepareCrossSellProductListModelAsync(object model, object product);
    Task<object> PrepareAddAssociatedProductSearchModelAsync(object model);
    Task<object> PrepareAddAssociatedProductListModelAsync(object model);
    Task<object> PrepareAssociatedProductListModelAsync(object model, object product);
    Task<object> PrepareProductTagSearchModelAsync(object model);
    Task<object> PrepareProductTagListModelAsync(object model);
    Task<object> PrepareProductTagModelAsync(object model);
    Task<object> PrepareProductTagModelAsync(object model, object productTag);
    Task<object> PrepareProductTagModelAsync(object model, object productTag, bool excludeProperties);
    Task<object> PrepareTaggedProductListModelAsync(object model);
    Task<object> PrepareProductAttributeMappingListModelAsync(object model);
    Task<object> PrepareProductAttributeMappingListModelAsync(object model, object product);
    Task<object> PrepareProductAttributeMappingModelAsync(object model, object product, object mapping, object copy = null);
    Task<object> PrepareProductAttributeValueListModelAsync(object model);
    Task<object> PrepareProductAttributeValueListModelAsync(object model, object product);
    Task<object> PrepareProductAttributeValueModelAsync(object model, object mapping, object value, object copy = null);
    Task<object> PrepareAssociateProductToAttributeValueSearchModelAsync(object model);
    Task<object> PrepareAssociateProductToAttributeValueListModelAsync(object model);
    Task<object> PrepareProductAttributeCombinationListModelAsync(object model);
    Task<object> PrepareProductAttributeCombinationListModelAsync(object model, object product);
    Task<object> PrepareProductAttributeCombinationModelAsync(object model, object product, object combination, object copy = null);
    Task<object> PrepareProductPictureListModelAsync(object model, object product);
    Task<object> PrepareProductVideoListModelAsync(object model, object product);
    Task<object> PrepareProductSpecificationAttributeListModelAsync(object model, object product);
    Task<object> PrepareAddSpecificationAttributeModelAsync(object model, object product);
    Task<object> PrepareProductOrderListModelAsync(object model, object product);
    Task<object> PrepareFilterLevelValueListModelAsync(object model, object product);
    Task<object> PrepareStockQuantityHistoryListModelAsync(object model, object product);
}
