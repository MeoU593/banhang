using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class CheckoutController : BasePublicController
{
    [Route("checkout/{*path}")]
    [Route("onepagecheckout/{*path}")]
    public virtual IActionResult Disabled(string path = null)
    {
        return InvokeHttp404();
    }
}
