using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using FluentMigrator;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Topics;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopUpdateMigration("2026-05-27 17:00:00", "5.00", UpdateMigrationType.Data)]
public class NormalizeSeoSlugsMigration : Migration
{
    private readonly INopDataProvider _dataProvider;

    public NormalizeSeoSlugsMigration(INopDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    public override void Up()
    {
        ForceAsciiSeoSettings();
        NormalizeUrlRecords();
        NormalizeDocumentSlugs();
    }

    public override void Down()
    {
    }

    private void ForceAsciiSeoSettings()
    {
        const string sql = @"
IF EXISTS (SELECT 1 FROM [Setting] WHERE LOWER([Name]) = 'seosettings.convertnonwesternchars' AND [StoreId] = 0)
    UPDATE [Setting] SET [Value] = 'True' WHERE LOWER([Name]) = 'seosettings.convertnonwesternchars' AND [StoreId] = 0
ELSE
    INSERT INTO [Setting] ([Name], [Value], [StoreId]) VALUES ('seosettings.convertnonwesternchars', 'True', 0)

IF EXISTS (SELECT 1 FROM [Setting] WHERE LOWER([Name]) = 'seosettings.allowunicodecharsinurls' AND [StoreId] = 0)
    UPDATE [Setting] SET [Value] = 'False' WHERE LOWER([Name]) = 'seosettings.allowunicodecharsinurls' AND [StoreId] = 0
ELSE
    INSERT INTO [Setting] ([Name], [Value], [StoreId]) VALUES ('seosettings.allowunicodecharsinurls', 'False', 0)";

        _dataProvider.ExecuteNonQueryAsync(sql).GetAwaiter().GetResult();
    }

    private static void NormalizeUrlRecords()
    {
        var urlRecordRepository = EngineContext.Current.Resolve<IRepository<UrlRecord>>();
        var products = EngineContext.Current.Resolve<IRepository<Product>>().Table.ToDictionary(x => x.Id, x => x.Name);
        var categories = EngineContext.Current.Resolve<IRepository<Category>>().Table.ToDictionary(x => x.Id, x => x.Name);
        var manufacturers = EngineContext.Current.Resolve<IRepository<Manufacturer>>().Table.ToDictionary(x => x.Id, x => x.Name);
        var vendors = EngineContext.Current.Resolve<IRepository<Vendor>>().Table.ToDictionary(x => x.Id, x => x.Name);
        var topics = EngineContext.Current.Resolve<IRepository<Topic>>().Table.ToDictionary(x => x.Id, x => x.Title);
        var blogPosts = EngineContext.Current.Resolve<IRepository<BlogPost>>().Table.ToDictionary(x => x.Id, x => x.Title);
        var productTags = EngineContext.Current.Resolve<IRepository<ProductTag>>().Table.ToDictionary(x => x.Id, x => x.Name);

        var records = urlRecordRepository.Table
            .Where(record => record.IsActive)
            .OrderBy(record => record.Id)
            .ToList();
        var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var record in records)
        {
            var source = record.EntityName switch
            {
                nameof(Product) => products.GetValueOrDefault(record.EntityId),
                nameof(Category) => categories.GetValueOrDefault(record.EntityId),
                nameof(Manufacturer) => manufacturers.GetValueOrDefault(record.EntityId),
                nameof(Vendor) => vendors.GetValueOrDefault(record.EntityId),
                nameof(Topic) => topics.GetValueOrDefault(record.EntityId),
                nameof(BlogPost) => blogPosts.GetValueOrDefault(record.EntityId),
                nameof(ProductTag) => productTags.GetValueOrDefault(record.EntityId),
                _ => null
            };

            var baseSlug = NormalizeSlug(string.IsNullOrWhiteSpace(source) ? record.Slug : source);
            if (string.IsNullOrWhiteSpace(baseSlug))
                baseSlug = $"{record.EntityName.ToLowerInvariant()}-{record.EntityId}";

            var slug = EnsureUnique(baseSlug, used);
            used.Add(slug);
            if (string.Equals(record.Slug, slug, StringComparison.OrdinalIgnoreCase))
                continue;

            record.Slug = slug;
            urlRecordRepository.UpdateAsync(record).GetAwaiter().GetResult();
        }
    }

    private void NormalizeDocumentSlugs()
    {
        const string existsSql = @"SELECT CASE WHEN OBJECT_ID('Document', 'U') IS NULL THEN 0 ELSE 1 END AS [Value]";
        var tableExists = _dataProvider.QueryAsync<IntValueRow>(existsSql).GetAwaiter().GetResult().FirstOrDefault()?.Value == 1;
        if (!tableExists)
            return;

        var documents = _dataProvider.QueryAsync<DocumentSlugRow>(@"
            SELECT [Id], [Title], [Slug]
            FROM [Document]
            WHERE [Deleted] = 0
            ORDER BY [Id]").GetAwaiter().GetResult().ToList();

        var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var document in documents)
        {
            var baseSlug = NormalizeSlug(string.IsNullOrWhiteSpace(document.Title) ? document.Slug : document.Title);
            if (string.IsNullOrWhiteSpace(baseSlug))
                baseSlug = $"tai-lieu-{document.Id}";

            var slug = EnsureUnique(baseSlug, used);
            used.Add(slug);
            if (string.Equals(document.Slug, slug, StringComparison.OrdinalIgnoreCase))
                continue;

            _dataProvider.ExecuteNonQueryAsync($"UPDATE [Document] SET [Slug] = N'{SqlEscape(slug)}' WHERE [Id] = {document.Id}").GetAwaiter().GetResult();
        }
    }

    private static string EnsureUnique(string baseSlug, ISet<string> used)
    {
        var slug = baseSlug;
        var suffix = 2;
        while (used.Contains(slug))
            slug = $"{baseSlug}-{suffix++}";

        return slug;
    }

    private static string NormalizeSlug(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        value = PrepareSlugSource(value);
        var normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);
        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
                continue;

            if (c == 'đ')
                builder.Append('d');
            else if (char.IsLetterOrDigit(c))
                builder.Append(c);
            else if (char.IsWhiteSpace(c) || c == '-' || c == '_')
                builder.Append('-');
        }

        var slug = Regex.Replace(builder.ToString(), "[^a-z0-9-]", string.Empty);
        slug = Regex.Replace(slug, "-+", "-").Trim('-');
        return slug.Length > 200 ? slug[..200].TrimEnd('-') : slug;
    }

    private static string PrepareSlugSource(string value)
    {
        if (value.Contains('%'))
        {
            try
            {
                value = Uri.UnescapeDataString(value);
            }
            catch
            {
            }
        }

        if (!LooksLikeMojibake(value))
            return value;

        try
        {
            var fixedValue = Encoding.UTF8.GetString(Encoding.Latin1.GetBytes(value));
            if (!string.IsNullOrWhiteSpace(fixedValue) && !fixedValue.Contains('\uFFFD'))
                return fixedValue;
        }
        catch
        {
        }

        return value;
    }

    private static bool LooksLikeMojibake(string value)
    {
        return value.Contains('Ã')
            || value.Contains('Â')
            || value.Contains("áº", StringComparison.Ordinal)
            || value.Contains("á»", StringComparison.Ordinal)
            || value.Contains("Ä", StringComparison.Ordinal);
    }

    private static string SqlEscape(string value)
    {
        return value.Replace("'", "''");
    }

    private sealed class IntValueRow
    {
        public int Value { get; set; }
    }

    private sealed class DocumentSlugRow
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
    }
}
