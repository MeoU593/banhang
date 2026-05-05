namespace Nop.Plugin.Misc.LogisticsReports.Services;

public interface IStorageService
{
    Task<string> SaveTemplateAsync(string fileName, Stream stream);
    Task<string> SaveImportAsync(string fileName, Stream stream);
    Task<string> SaveTextAsync(string relativeFolder, string fileName, string contents);
    Task DeleteAsync(string relativePath);
    Task CleanupOldFilesAsync(string relativeFolder, TimeSpan maxAge);
    string GetAbsolutePath(string relativePath);
    Stream OpenRead(string relativePath);
}
