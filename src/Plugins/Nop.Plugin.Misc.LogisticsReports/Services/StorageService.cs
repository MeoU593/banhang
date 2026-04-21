using Nop.Core;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.Misc.LogisticsReports.Services;

public class StorageService : IStorageService
{
    private readonly INopFileProvider _fileProvider;
    private const string RootFolder = "App_Data/LogisticsReports";

    public StorageService(INopFileProvider fileProvider)
    {
        _fileProvider = fileProvider;
    }

    public async Task<string> SaveTemplateAsync(string fileName, Stream stream)
        => await SaveAsync("Templates", fileName, stream);

    public async Task<string> SaveImportAsync(string fileName, Stream stream)
        => await SaveAsync("Imports", fileName, stream);

    public async Task<string> SaveTextAsync(string relativeFolder, string fileName, string contents)
    {
        var folder = EnsureFolder(relativeFolder);
        var finalName = $"{Guid.NewGuid():N}_{fileName}";
        var absolute = _fileProvider.Combine(folder, finalName);
        await File.WriteAllTextAsync(absolute, contents);
        return absolute.Replace(_fileProvider.MapPath("~/"), string.Empty).Replace('\\', '/');
    }

    public string GetAbsolutePath(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return string.Empty;

        return _fileProvider.Combine(_fileProvider.MapPath("~/"), relativePath.Replace('/', Path.DirectorySeparatorChar));
    }

    public Stream OpenRead(string relativePath)
    {
        var absolute = GetAbsolutePath(relativePath);
        return File.OpenRead(absolute);
    }

    private async Task<string> SaveAsync(string childFolder, string fileName, Stream stream)
    {
        var folder = EnsureFolder(childFolder);
        var finalName = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}_{Path.GetFileName(fileName)}";
        var absolute = _fileProvider.Combine(folder, finalName);

        await using var fileStream = File.Create(absolute);
        await stream.CopyToAsync(fileStream);

        return absolute.Replace(_fileProvider.MapPath("~/"), string.Empty).Replace('\\', '/');
    }

    private string EnsureFolder(string childFolder)
    {
        var root = _fileProvider.MapPath($"~/{RootFolder}");
        if (!_fileProvider.DirectoryExists(root))
            _fileProvider.CreateDirectory(root);

        var folder = _fileProvider.Combine(root, childFolder);
        if (!_fileProvider.DirectoryExists(folder))
            _fileProvider.CreateDirectory(folder);

        return folder;
    }
}
