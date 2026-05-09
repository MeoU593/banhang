using Microsoft.AspNetCore.Mvc;

namespace Nop.Web.Areas.Admin.Components;

public partial class SettingModeViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}
