using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Catalog;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Nop.Web.Components;

public partial class ManufacturerNavigationViewComponent : NopViewComponent
{
    protected readonly CatalogSettings _catalogSettings;
    protected readonly ICatalogModelFactory _catalogModelFactory;

    public ManufacturerNavigationViewComponent(CatalogSettings catalogSettings, ICatalogModelFactory catalogModelFactory)
    {
        _catalogSettings = catalogSettings;
        _catalogModelFactory = catalogModelFactory;
    }

    public Task<IViewComponentResult> InvokeAsync(int currentManufacturerId)
        => Task.FromResult<IViewComponentResult>(Content(""));
}
