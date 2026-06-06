namespace Nop.Services.Logging;

public static partial class NopActivityLogDefaults
{
    public const string PublicStoreSuccessfulLogin = "PublicStore.SuccessfulLogin";
    public const string PublicStoreLogout = "PublicStore.Logout";
    public const string AddNewNewsPost = "AddNewNewsPost";
    public const string AddNewBlogDocument = "AddNewBlogDocument";
    public const string AddNewPortalDocument = "AddNewPortalDocument";
    public const string AddNewProduct = "AddNewProduct";
    public const string SubmitUnitReport = "SubmitUnitReport";

    public static readonly string[] SystemActivityLogKeywords =
    [
        PublicStoreSuccessfulLogin,
        PublicStoreLogout,
        AddNewNewsPost,
        AddNewBlogDocument,
        AddNewPortalDocument,
        AddNewProduct,
        SubmitUnitReport
    ];
}
