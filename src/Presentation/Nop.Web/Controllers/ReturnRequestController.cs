using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Controllers;

[AutoValidateAntiforgeryToken]
public partial class ReturnRequestController : BasePublicController
{
    [Route("returnrequest/{*path}")]
    [Route("uploadfilereturnrequest")]
    public virtual IActionResult Disabled(string path = null)
    {
        return InvokeHttp404();
    }
}
