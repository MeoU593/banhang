namespace Nop.Plugin.Misc.DocumentPortal;

public static class DocumentPortalDefaults
{
    public static class Permissions
    {
        public const string MANAGE_DOCUMENTS = "DocumentPortal.ManageDocuments";
        public const string MANAGE_DOCUMENT_SETTINGS = "DocumentPortal.ManageDocumentSettings";
    }

    public static class Routes
    {
        public const string LIST = "DocumentPortal.List";
        public const string SEARCH = "DocumentPortal.Search";
        public const string DETAIL = "DocumentPortal.Detail";
        public const string PREVIEW = "DocumentPortal.Preview";
        public const string DOWNLOAD = "DocumentPortal.Download";
        public const string UPLOAD = "DocumentPortal.Upload";
        public const string DELETE = "DocumentPortal.Delete";
        public const string EDIT_OWN = "DocumentPortal.EditOwn";
        public const string DELETE_OWN = "DocumentPortal.DeleteOwn";
    }
}
