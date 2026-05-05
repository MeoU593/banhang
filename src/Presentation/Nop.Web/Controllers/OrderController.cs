using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class OrderController : BasePublicController
{
    [Route("order/{*path}")]
    [Route("orderdetails/{*path}")]
    [Route("reorder/{*path}")]
    public virtual IActionResult Disabled(string path = null)
    {
        return InvokeHttp404();
    }
}
