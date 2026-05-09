namespace Nop.Web.Areas.Admin.Factories;

public partial class ProductModelFactory : IProductModelFactory
{
    public async Task<object> PrepareProductSearchModelAsync(object model) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareProductListModelAsync(object model) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareProductModelAsync(object model, object product, object copy = null) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareAddRequiredProductSearchModelAsync(object model) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareAddRequiredProductListModelAsync(object model) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareAddRequiredProductListModelAsync(object model, object product) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareAddRelatedProductSearchModelAsync(object model) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareAddRelatedProductListModelAsync(object model) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareRelatedProductListModelAsync(object model, object product) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareAddCrossSellProductSearchModelAsync(object model) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareAddCrossSellProductListModelAsync(object model) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareCrossSellProductListModelAsync(object model, object product) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareAddAssociatedProductSearchModelAsync(object model) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareAddAssociatedProductListModelAsync(object model) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareAssociatedProductListModelAsync(object model, object product) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareProductTagSearchModelAsync(object model) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareProductTagListModelAsync(object model) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareProductTagModelAsync(object model) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareProductTagModelAsync(object model, object productTag) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareProductTagModelAsync(object model, object productTag, bool excludeProperties) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareTaggedProductListModelAsync(object model) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareProductAttributeMappingListModelAsync(object model) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareProductAttributeMappingListModelAsync(object model, object product) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareProductAttributeMappingModelAsync(object model, object product, object mapping, object copy = null) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareProductAttributeValueListModelAsync(object model) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareProductAttributeValueListModelAsync(object model, object product) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareProductAttributeValueModelAsync(object model, object mapping, object value, object copy = null) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareAssociateProductToAttributeValueSearchModelAsync(object model) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareAssociateProductToAttributeValueListModelAsync(object model) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareProductAttributeCombinationListModelAsync(object model) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareProductAttributeCombinationListModelAsync(object model, object product) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareProductAttributeCombinationModelAsync(object model, object product, object combination, object copy = null) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareProductPictureListModelAsync(object model, object product) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareProductVideoListModelAsync(object model, object product) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareProductSpecificationAttributeListModelAsync(object model, object product) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareAddSpecificationAttributeModelAsync(object model, object product) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareProductOrderListModelAsync(object model, object product) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareFilterLevelValueListModelAsync(object model, object product) => await Task.FromResult(model ?? new object());
    public async Task<object> PrepareStockQuantityHistoryListModelAsync(object model, object product) => await Task.FromResult(model ?? new object());
}
