using FluentMigrator;
using Nop.Core;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.LogisticsReports.Domain;
using Nop.Services.Blogs;
using Nop.Services.Catalog;
using Nop.Services.Forums;
using Nop.Services.Seo;

namespace Nop.Plugin.Misc.LogisticsReports.Data;

[NopMigration("2026/04/23 18:30:00", "Misc.LogisticsReports seed generated TTG demo portal content", MigrationProcessType.Update)]
public class SeedGeneratedDemoPortalContentMigration : MigrationBase
{
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        var vendorRepository = EngineContext.Current.Resolve<IRepository<Vendor>>();
        var blogPostRepository = EngineContext.Current.Resolve<IRepository<BlogPost>>();
        var languageRepository = EngineContext.Current.Resolve<IRepository<Language>>();
        var categoryRepository = EngineContext.Current.Resolve<IRepository<Category>>();
        var productRepository = EngineContext.Current.Resolve<IRepository<Product>>();
        var productCategoryRepository = EngineContext.Current.Resolve<IRepository<ProductCategory>>();
        var forumTopicRepository = EngineContext.Current.Resolve<IRepository<ForumTopic>>();
        var customerRepository = EngineContext.Current.Resolve<IRepository<Customer>>();
        var blogService = EngineContext.Current.Resolve<IBlogService>();
        var categoryService = EngineContext.Current.Resolve<ICategoryService>();
        var productService = EngineContext.Current.Resolve<IProductService>();
        var forumService = EngineContext.Current.Resolve<IForumService>();
        var urlRecordService = EngineContext.Current.Resolve<IUrlRecordService>();

        var languageId = GetVietnameseLanguageId(languageRepository);
        var adminCustomerId = GetAdministratorCustomerId(customerRepository);

        var vendorsByCode = SeedVendors(vendorRepository, urlRecordService);
        var categoriesByName = SeedCategories(categoryRepository, categoryService, urlRecordService);

        SeedBlogPosts(blogPostRepository, blogService, urlRecordService, languageId, GetNewsSeeds(), 0, allowComments: true);
        SeedBlogPosts(blogPostRepository, blogService, urlRecordService, languageId, GetDocumentSeeds(), 1, allowComments: false);

        SeedProducts(
            productRepository,
            productCategoryRepository,
            productService,
            categoryService,
            urlRecordService,
            vendorsByCode,
            categoriesByName,
            GetProductSeeds());

        SeedForumContent(forumTopicRepository, forumService, adminCustomerId, GetForumTopicSeeds());
    }

    public override void Down()
    {
    }

    private static Dictionary<string, Vendor> SeedVendors(IRepository<Vendor> vendorRepository, IUrlRecordService urlRecordService)
    {
        var allVendors = vendorRepository.Table.ToList();
        var vendorsByCode = allVendors
            .Where(x => !x.Deleted && !string.IsNullOrWhiteSpace(x.Code))
            .GroupBy(x => x.Code!, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);

        foreach (var seed in GetVendorSeeds())
        {
            vendorsByCode.TryGetValue(seed.Code, out var vendor);
            vendor ??= allVendors.FirstOrDefault(x => !x.Deleted && string.Equals(x.Name, seed.Name, StringComparison.OrdinalIgnoreCase));

            var isNew = vendor is null;
            vendor ??= CreateVendorShell();

            Vendor? parentVendor = null;
            if (!string.IsNullOrWhiteSpace(seed.ParentCode) && vendorsByCode.TryGetValue(seed.ParentCode, out var matchedParent))
                parentVendor = matchedParent;

            ApplyVendorSeed(vendor!, seed, parentVendor);

            if (isNew)
            {
                vendorRepository.InsertAsync(vendor!).GetAwaiter().GetResult();
                allVendors.Add(vendor!);
            }
            else
                vendorRepository.UpdateAsync(vendor!).GetAwaiter().GetResult();

            SaveSlug(vendor!, vendor!.Name, 0, urlRecordService);
            vendorsByCode[seed.Code] = vendor!;
        }

        return vendorsByCode;
    }

    private static Dictionary<string, Category> SeedCategories(
        IRepository<Category> categoryRepository,
        ICategoryService categoryService,
        IUrlRecordService urlRecordService)
    {
        var categoriesByName = categoryRepository.Table
            .ToList()
            .GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);

        foreach (var seed in GetCategorySeeds())
        {
            categoriesByName.TryGetValue(seed.Name, out var category);
            var isNew = category is null;
            if (isNew)
            {
                category = new Category
                {
                    CategoryTemplateId = 1,
                    ParentCategoryId = 0,
                    PictureId = 0,
                    PageSize = 12,
                    AllowCustomersToSelectPageSize = true,
                    PageSizeOptions = "12,24,36",
                    ShowOnHomepage = false,
                    SubjectToAcl = false,
                    LimitedToStores = false,
                    PriceRangeFiltering = false,
                    PriceFrom = 0,
                    PriceTo = 0,
                    ManuallyPriceRange = false,
                    RestrictFromVendors = false,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow
                };
            }

            category!.Name = seed.Name;
            category.Description = seed.Description;
            category.MetaTitle = seed.Name;
            category.MetaDescription = seed.Description;
            category.MetaKeywords = seed.MetaKeywords;
            category.Published = true;
            category.Deleted = false;
            category.DisplayOrder = seed.DisplayOrder;
            category.UpdatedOnUtc = DateTime.UtcNow;

            if (isNew)
                categoryService.InsertCategoryAsync(category).GetAwaiter().GetResult();
            else
                categoryService.UpdateCategoryAsync(category).GetAwaiter().GetResult();

            SaveSlug(category, category.Name, 0, urlRecordService);
            categoriesByName[seed.Name] = category;
        }

        return categoriesByName;
    }

    private static void SeedBlogPosts(
        IRepository<BlogPost> blogPostRepository,
        IBlogService blogService,
        IUrlRecordService urlRecordService,
        int languageId,
        IReadOnlyList<BlogSeed> seeds,
        int postTypeId,
        bool allowComments)
    {
        foreach (var seed in seeds)
        {
            var entity = blogPostRepository.Table.FirstOrDefault(x => x.PostTypeId == postTypeId && x.Title == seed.Title);
            var isNew = entity is null;
            entity ??= new BlogPost();

            entity.LanguageId = languageId;
            entity.IncludeInSitemap = true;
            entity.Title = seed.Title;
            entity.BodyOverview = seed.Overview;
            entity.Body = seed.Body;
            entity.AllowComments = allowComments;
            entity.PostTypeId = postTypeId;
            entity.ThumbnailPictureId = 0;
            entity.AuthorName = "Admin";
            entity.PdfDownloadId = 0;
            entity.DocDownloadId = 0;
            entity.Tags = seed.Tags;
            entity.StartDateUtc = seed.CreatedOnUtc;
            entity.EndDateUtc = null;
            entity.MetaTitle = seed.Title;
            entity.MetaDescription = seed.Overview;
            entity.MetaKeywords = seed.Tags;
            entity.LimitedToStores = false;
            entity.CreatedOnUtc = seed.CreatedOnUtc;

            if (isNew)
                blogService.InsertBlogPostAsync(entity).GetAwaiter().GetResult();
            else
                blogService.UpdateBlogPostAsync(entity).GetAwaiter().GetResult();

            SaveSlug(entity, entity.Title, languageId, urlRecordService);
        }
    }

    private static void SeedProducts(
        IRepository<Product> productRepository,
        IRepository<ProductCategory> productCategoryRepository,
        IProductService productService,
        ICategoryService categoryService,
        IUrlRecordService urlRecordService,
        IReadOnlyDictionary<string, Vendor> vendorsByCode,
        IReadOnlyDictionary<string, Category> categoriesByName,
        IReadOnlyList<ProductSeed> seeds)
    {
        foreach (var seed in seeds)
        {
            var entity = productRepository.Table.FirstOrDefault(x => x.Sku == seed.Sku);
            var isNew = entity is null;
            entity ??= new Product();

            vendorsByCode.TryGetValue(seed.VendorCode, out var vendor);
            categoriesByName.TryGetValue(seed.CategoryName, out var category);

            entity.ProductType = ProductType.SimpleProduct;
            entity.ParentGroupedProductId = 0;
            entity.VisibleIndividually = true;
            entity.Name = seed.Name;
            entity.ShortDescription = seed.ShortDescription;
            entity.FullDescription = $"<p>{seed.FullDescription}</p><p><strong>Đơn vị báo cáo:</strong> {seed.VendorName}</p><p><strong>Năng lực / số lượng:</strong> {seed.ShortDescription}</p>";
            entity.AdminComment = "Generated TTG demo product";
            entity.ProductTemplateId = 1;
            entity.VendorId = vendor?.Id ?? 0;
            entity.ShowOnHomepage = seed.ShowOnHomepage;
            entity.MetaKeywords = seed.MetaKeywords;
            entity.MetaDescription = seed.ShortDescription;
            entity.MetaTitle = seed.Name;
            entity.AllowCustomerReviews = true;
            entity.SubjectToAcl = false;
            entity.LimitedToStores = false;
            entity.Sku = seed.Sku;
            entity.IsGiftCard = false;
            entity.IsDownload = false;
            entity.UnlimitedDownloads = false;
            entity.MaxNumberOfDownloads = 0;
            entity.DownloadExpirationDays = null;
            entity.DownloadActivationTypeId = 0;
            entity.HasSampleDownload = false;
            entity.SampleDownloadId = 0;
            entity.HasUserAgreement = false;
            entity.UserAgreementText = string.Empty;
            entity.IsRecurring = false;
            entity.RecurringCycleLength = 0;
            entity.RecurringCyclePeriodId = 0;
            entity.RecurringTotalCycles = 0;
            entity.IsRental = false;
            entity.RentalPriceLength = 0;
            entity.RentalPricePeriodId = 0;
            entity.ManageInventoryMethod = ManageInventoryMethod.DontManageStock;
            entity.ProductAvailabilityRangeId = 0;
            entity.StockQuantity = 0;
            entity.DisplayStockAvailability = false;
            entity.DisplayStockQuantity = false;
            entity.MinStockQuantity = 0;
            entity.LowStockActivity = LowStockActivity.Nothing;
            entity.NotifyAdminForQuantityBelow = 0;
            entity.BackorderMode = BackorderMode.NoBackorders;
            entity.OrderMinimumQuantity = 1;
            entity.OrderMaximumQuantity = 10000;
            entity.AllowedQuantities = string.Empty;
            entity.AllowAddingOnlyExistingAttributeCombinations = false;
            entity.DisplayAttributeCombinationImagesOnly = false;
            entity.NotReturnable = false;
            entity.DisableBuyButton = true;
            entity.DisableWishlistButton = true;
            entity.AvailableForPreOrder = false;
            entity.PreOrderAvailabilityStartDateTimeUtc = null;
            entity.CallForPrice = true;
            entity.Price = 0;
            entity.OldPrice = 0;
            entity.ProductCost = 0;
            entity.CustomerEntersPrice = false;
            entity.MinimumCustomerEnteredPrice = 0;
            entity.MaximumCustomerEnteredPrice = 0;
            entity.BasepriceEnabled = false;
            entity.BasepriceAmount = 0;
            entity.BasepriceUnitId = 0;
            entity.BasepriceBaseAmount = 0;
            entity.BasepriceBaseUnitId = 0;
            entity.MarkAsNew = seed.ShowOnHomepage;
            entity.MarkAsNewStartDateTimeUtc = seed.CreatedOnUtc;
            entity.MarkAsNewEndDateTimeUtc = null;
            entity.Weight = 0;
            entity.Length = 0;
            entity.Width = 0;
            entity.Height = 0;
            entity.AvailableStartDateTimeUtc = null;
            entity.AvailableEndDateTimeUtc = null;
            entity.DisplayOrder = seed.DisplayOrder;
            entity.Published = true;
            entity.Deleted = false;
            entity.CreatedOnUtc = seed.CreatedOnUtc;
            entity.UpdatedOnUtc = DateTime.UtcNow;
            entity.AllowBackInStockSubscriptions = false;

            if (isNew)
                productService.InsertProductAsync(entity).GetAwaiter().GetResult();
            else
                productService.UpdateProductAsync(entity).GetAwaiter().GetResult();

            SaveSlug(entity, entity.Name, 0, urlRecordService);

            if (category is null)
                continue;

            var mappingExists = productCategoryRepository.Table.Any(x => x.ProductId == entity.Id && x.CategoryId == category.Id);
            if (mappingExists)
                continue;

            categoryService.InsertProductCategoryAsync(new ProductCategory
            {
                ProductId = entity.Id,
                CategoryId = category.Id,
                IsFeaturedProduct = false,
                DisplayOrder = 1
            }).GetAwaiter().GetResult();
        }
    }

    private static void SeedForumContent(
        IRepository<ForumTopic> forumTopicRepository,
        IForumService forumService,
        int adminCustomerId,
        IReadOnlyList<ForumTopicSeed> seeds)
    {
        var forum = EnsureForum(forumService);
        foreach (var seed in seeds)
        {
            var exists = forumTopicRepository.Table.Any(x => x.ForumId == forum.Id && x.Subject == seed.Subject);
            if (exists)
                continue;

            var topic = new ForumTopic
            {
                ForumId = forum.Id,
                CustomerId = adminCustomerId,
                ForumTopicType = seed.TopicType,
                Subject = seed.Subject,
                NumPosts = 0,
                Views = seed.InitialViews,
                LastPostId = 0,
                LastPostCustomerId = 0,
                LastPostTime = seed.CreatedOnUtc,
                CreatedOnUtc = seed.CreatedOnUtc,
                UpdatedOnUtc = seed.CreatedOnUtc
            };
            forumService.InsertTopicAsync(topic, false).GetAwaiter().GetResult();

            var allPosts = new List<string>
            {
                $"Tác giả gốc: {seed.OriginalAuthor}\n\n{seed.OpeningPost}"
            };
            allPosts.AddRange(seed.Replies);

            var offset = 0;
            foreach (var postText in allPosts)
            {
                var timestamp = seed.CreatedOnUtc.AddHours(offset * 4);
                var post = new ForumPost
                {
                    TopicId = topic.Id,
                    CustomerId = adminCustomerId,
                    Text = postText,
                    IPAddress = "127.0.0.1",
                    CreatedOnUtc = timestamp,
                    UpdatedOnUtc = timestamp,
                    VoteCount = 0
                };
                forumService.InsertPostAsync(post, false).GetAwaiter().GetResult();
                offset++;
            }
        }
    }

    private static Forum EnsureForum(IForumService forumService)
    {
        var forum = forumService.GetForumByIdAsync(37).GetAwaiter().GetResult();
        if (forum != null)
            return forum;

        var forumGroup = forumService.GetAllForumGroupsAsync().GetAwaiter().GetResult()
            .FirstOrDefault(x => string.Equals(x.Name, "Diễn đàn", StringComparison.OrdinalIgnoreCase));

        if (forumGroup == null)
        {
            forumGroup = new ForumGroup
            {
                Name = "Diễn đàn",
                DisplayOrder = 1,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
            forumService.InsertForumGroupAsync(forumGroup).GetAwaiter().GetResult();
        }

        forum = forumService.GetAllForumsByGroupIdAsync(forumGroup.Id).GetAwaiter().GetResult()
            .FirstOrDefault(x => string.Equals(x.Name, "Diễn đàn", StringComparison.OrdinalIgnoreCase));

        if (forum != null)
            return forum;

        forum = new Forum
        {
            ForumGroupId = forumGroup.Id,
            Name = "Diễn đàn",
            Description = "Không gian trao đổi kinh nghiệm hậu cần, quân nhu và bảo đảm kỹ thuật trong toàn Binh chủng.",
            NumTopics = 0,
            NumPosts = 0,
            LastTopicId = 0,
            LastPostId = 0,
            LastPostCustomerId = 0,
            LastPostTime = null,
            DisplayOrder = 1,
            CreatedOnUtc = DateTime.UtcNow,
            UpdatedOnUtc = DateTime.UtcNow
        };
        forumService.InsertForumAsync(forum).GetAwaiter().GetResult();
        return forum;
    }

    private static int GetVietnameseLanguageId(IRepository<Language> languageRepository)
    {
        var languageId = languageRepository.Table
            .Where(x => x.Published)
            .OrderByDescending(x => x.LanguageCulture == "vi-VN")
            .ThenBy(x => x.DisplayOrder)
            .Select(x => x.Id)
            .FirstOrDefault();

        if (languageId > 0)
            return languageId;

        return languageRepository.Table.OrderBy(x => x.Id).Select(x => x.Id).FirstOrDefault();
    }

    private static int GetAdministratorCustomerId(IRepository<Customer> customerRepository)
    {
        return customerRepository.Table
            .OrderBy(x => x.Id)
            .Select(x => x.Id)
            .FirstOrDefault();
    }

    private static void ApplyVendorSeed(Vendor vendor, VendorSeed seed, Vendor? parentVendor)
    {
        vendor.Name = seed.Name;
        vendor.Code = seed.Code;
        vendor.ParentId = parentVendor?.Id;
        vendor.Level = parentVendor == null ? 0 : parentVendor.Level + 1;
        vendor.Path = parentVendor == null ? "/BTTG" : $"{parentVendor.Path}/{seed.Code}";
        vendor.DisplayOrder = seed.DisplayOrder;
        vendor.Active = true;
        vendor.Deleted = false;
        vendor.Email = seed.Email;
        vendor.Description = seed.Description;
        vendor.AdminComment = "Generated TTG demo organization unit";
        vendor.MetaKeywords = "tang thiet giap, hau can, quan nhu";
        vendor.MetaDescription = seed.Description;
        vendor.MetaTitle = seed.Name;
        vendor.PageSize = 9;
        vendor.AllowCustomersToSelectPageSize = true;
        vendor.PageSizeOptions = "9,18,27";
        vendor.PriceRangeFiltering = true;
        vendor.ManuallyPriceRange = true;
    }

    private static Vendor CreateVendorShell()
    {
        return new Vendor
        {
            Name = string.Empty,
            Email = string.Empty,
            Description = string.Empty,
            AdminComment = string.Empty,
            Active = true,
            Deleted = false,
            DisplayOrder = 0,
            MetaKeywords = string.Empty,
            MetaDescription = string.Empty,
            MetaTitle = string.Empty,
            PageSize = 9,
            AllowCustomersToSelectPageSize = true,
            PageSizeOptions = "9,18,27",
            PriceRangeFiltering = true,
            PriceFrom = 0,
            PriceTo = 0,
            ManuallyPriceRange = true,
            Code = string.Empty,
            ParentId = null,
            Level = 0,
            Path = string.Empty
        };
    }

    private static void SaveSlug<T>(T entity, string name, int languageId, IUrlRecordService urlRecordService)
        where T : BaseEntity, ISlugSupported
    {
        var slug = urlRecordService.ValidateSeNameAsync(entity, string.Empty, name, true).GetAwaiter().GetResult();
        urlRecordService.SaveSlugAsync(entity, slug, languageId).GetAwaiter().GetResult();
    }

    private static IReadOnlyList<VendorSeed> GetVendorSeeds()
    {
        return new List<VendorSeed>
        {
            new("BTTG", "Binh chủng Tăng Thiết Giáp", null, 0, "Cơ quan đầu mối chỉ đạo công tác hậu cần, quân nhu và bảo đảm kỹ thuật trong toàn Binh chủng Tăng Thiết Giáp.", "bttg@demo.local"),
            new("LĐ201", "Lữ đoàn Xe tăng 201", "BTTG", 10, "Đơn vị chiến đấu chủ lực, trọng điểm trong bảo đảm hậu cần chính quy và tổ chức bếp ăn tập trung phục vụ huấn luyện cường độ cao.", "ld201@demo.local"),
            new("LĐ215", "Lữ đoàn Xe tăng 215", "BTTG", 20, "Đơn vị có phong trào tăng gia sản xuất mạnh, nổi bật về mô hình trồng rau an toàn và tự túc thực phẩm theo mùa.", "ld215@demo.local"),
            new("LĐ405", "Lữ đoàn Xe tăng 405", "BTTG", 30, "Đơn vị thường xuyên huấn luyện dã ngoại phân tán, chú trọng bảo đảm lương thực, nước uống và cơ động bếp ăn thực địa.", "ld405@demo.local"),
            new("THS1", "Trường Hạ sĩ quan Xe tăng I", "BTTG", 40, "Cơ sở đào tạo kíp xe và nhân viên chuyên môn kỹ thuật, đồng thời là điểm thực hành tổ chức nuôi quân chính quy cho học viên.", "ths1@demo.local"),
            new("TCKT", "Trường Trung cấp Kỹ thuật TTG", "BTTG", 50, "Nhà trường đào tạo thợ sửa chữa xe tăng, kỹ thuật viên hậu cần và nhân viên bảo đảm kỹ thuật chuyên sâu.", "tckt@demo.local"),
            new("CHC_TTG", "Cục Hậu cần", "BTTG", 60, "Cơ quan tham mưu, chỉ đạo công tác quân nhu, doanh trại, xăng dầu và bảo đảm đời sống bộ đội trong toàn Binh chủng.", "chc@demo.local"),
            new("CKT_TTG", "Cục Kỹ thuật", "BTTG", 70, "Đầu mối bảo đảm trang bị kỹ thuật, phụ tùng và năng lực sửa chữa phục vụ huấn luyện, sẵn sàng chiến đấu.", "ckt@demo.local"),
            new("XM_T72", "Xưởng sửa chữa xe tăng", "BTTG", 80, "Đơn vị bảo đảm kỹ thuật chuyên sửa chữa, đại tu và gia công phụ tùng thay thế cho xe tăng, xe thiết giáp.", "xmt72@demo.local"),
            new("X20", "Xưởng X20", "BTTG", 90, "Cơ sở sản xuất quân trang và trang bị chuyên dùng, tập trung vào mũ công tác, quần áo dã ngoại và găng tay bảo hộ cho kíp xe.", "x20@demo.local")
        };
    }

    private static IReadOnlyList<CategorySeed> GetCategorySeeds()
    {
        return new List<CategorySeed>
        {
            new("Thực phẩm tươi sống", "Nhóm thực phẩm tươi phục vụ bếp ăn, tăng gia và bảo đảm khẩu phần thường xuyên cho bộ đội.", "thực phẩm tươi sống, hậu cần, quân nhu", 10),
            new("Lương thực chính", "Nhóm lương thực thiết yếu phục vụ cấp phát theo định mức ăn hằng ngày và dự trữ cơ động.", "lương thực chính, gạo, quân nhu", 20),
            new("Thực phẩm khô - dự trữ", "Nhóm thực phẩm khô, khẩu phần đóng gói và nguồn dự trữ bảo đảm dài ngày, dã ngoại hoặc hành quân.", "thực phẩm khô, dự trữ, hậu cần", 30),
            new("Trang bị - Quân nhu", "Trang bị quân nhu, vật tư phục vụ bếp ăn, quân trang và bảo đảm sinh hoạt chính quy cho các đơn vị tăng thiết giáp.", "trang bị quân nhu, quân trang, vật tư", 110),
            new("Nhiên liệu - Vật tư kỹ thuật", "Nhóm nhiên liệu và vật tư kỹ thuật dùng cho bảo đảm vận hành, huấn luyện và cơ động xe tăng thiết giáp.", "nhiên liệu, vật tư kỹ thuật, diesel", 120),
            new("Phụ tùng - Bảo đảm kỹ thuật", "Nhóm phụ tùng thay thế, chi tiết sửa chữa và bộ đồng bộ kỹ thuật cho xe tăng, xe thiết giáp.", "phụ tùng, bảo đảm kỹ thuật, xe tăng", 130)
        };
    }

    private static IReadOnlyList<BlogSeed> GetNewsSeeds()
    {
        return new List<BlogSeed>
        {
            new("Khai mạc đợt kiểm tra bếp ăn chính quy tại các lữ đoàn tăng thiết giáp", "Cục Hậu cần triển khai đợt kiểm tra đồng bộ nhằm chuẩn hóa quy trình nuôi quân và bảo đảm vệ sinh an toàn thực phẩm trong toàn Binh chủng.", "<p>Đợt kiểm tra tập trung vào công tác tiếp nhận thực phẩm, bảo quản kho lạnh, chế biến tập trung và duy trì bếp ăn mẫu tại các lữ đoàn. Nhiều đơn vị đã chủ động đầu tư thiết bị bếp điện công suất lớn, hệ thống thoát khói và khu sơ chế riêng biệt.</p><p>Kết quả sơ bộ cho thấy các đơn vị đã cải thiện rõ rệt chất lượng khẩu phần, giảm thất thoát nguyên liệu và nâng cao điều kiện phục vụ bộ đội trong ngày huấn luyện cường độ cao.</p>", "hậu cần, bếp ăn, kiểm tra", new DateTime(2026, 4, 2, 1, 0, 0, DateTimeKind.Utc)),
            new("Lữ đoàn 215 mở rộng khu tăng gia tập trung theo hướng VietGAP", "Mô hình rau an toàn và chăn nuôi khép kín tiếp tục được mở rộng để tăng tỷ lệ tự túc thực phẩm trong mùa huấn luyện hè.", "<p>Đơn vị đã hoàn thiện thêm các nhà lưới, hệ thống tưới tiết kiệm và khu sơ chế rau an toàn phục vụ bếp ăn tập trung. Nguồn rau xanh tự sản xuất hiện đáp ứng ổn định cho bữa ăn bộ đội theo tuần.</p><p>Bên cạnh đó, khu chăn nuôi được tổ chức tách biệt với nhà bếp, đảm bảo quy trình thú y và vệ sinh môi trường theo đúng hướng dẫn của cơ quan chuyên môn.</p>", "tăng gia, thực phẩm, vietgap", new DateTime(2026, 4, 4, 1, 0, 0, DateTimeKind.Utc)),
            new("Trường HSQ I nâng cấp dây chuyền chế biến thực phẩm phục vụ học viên", "Nhà trường đưa vào sử dụng thêm máy xay, tủ lạnh bảo quản và khu chia suất ăn mới nhằm đáp ứng quân số học viên tăng cao.", "<p>Dây chuyền mới giúp giảm thời gian chuẩn bị bữa ăn, tăng khả năng kiểm soát định lượng từng khẩu phần và bảo đảm thức ăn được đưa tới từng đại đội đúng giờ quy định.</p><p>Cùng với việc chuẩn hóa thực đơn, nhà trường chú trọng cải tiến quy trình vệ sinh dụng cụ và tổ chức kiểm thực ba bước hằng ngày.</p>", "nhà trường, chế biến tập trung, học viên", new DateTime(2026, 4, 6, 1, 0, 0, DateTimeKind.Utc)),
            new("Cục Kỹ thuật diễn tập cấp phát nhiên liệu cho đội hình cơ động dài ngày", "Phương án cấp phát diesel theo từng chặng được tổ chức gọn, nhanh, an toàn, phù hợp điều kiện huấn luyện phân tán trên nhiều hướng.", "<p>Trong diễn tập, cán bộ kỹ thuật kết hợp chặt chẽ với lực lượng hậu cần để lập điểm cấp phát nhiên liệu dã chiến, tổ chức niêm phong vòi cấp và kiểm soát sản lượng theo từng xe.</p><p>Kinh nghiệm rút ra là phải bám chắc đường cơ động, dự báo mức tiêu hao và duy trì tổ bảo đảm kỹ thuật đi cùng đội hình ngay từ đầu.</p>", "nhiên liệu, kỹ thuật, cơ động", new DateTime(2026, 4, 8, 1, 0, 0, DateTimeKind.Utc)),
            new("Lữ đoàn 405 hoàn thiện mô hình bếp dã ngoại tiết kiệm chất đốt", "Từ thực tiễn huấn luyện phân tán, đơn vị đã cải tiến khu bếp dã ngoại, giảm tiêu hao nhiên liệu và rút ngắn thời gian nấu ăn cho bộ đội.", "<p>Mô hình mới sử dụng bếp điện di động kết hợp bếp dầu dự phòng, đồng thời tổ chức lại sơ đồ bếp theo hướng một chiều. Nhờ đó, khâu chia suất ăn nhanh hơn, hạn chế ùn tắc trong giờ cao điểm.</p><p>Đơn vị cũng kết hợp xe vận tải làm kho thực phẩm lưu động, tăng khả năng cơ động đến các điểm trú quân xa trung tâm.</p>", "bếp dã ngoại, lữ đoàn 405, tiết kiệm", new DateTime(2026, 4, 10, 1, 0, 0, DateTimeKind.Utc)),
            new("Xưởng X20 đẩy mạnh sản xuất quân trang cho kíp xe huấn luyện mùa mưa", "Nhiều sản phẩm mới được hoàn thiện theo hướng bền, nhẹ, thoát nhiệt tốt và đáp ứng tốt yêu cầu sử dụng trong điều kiện dã ngoại.", "<p>Đợt sản xuất mới tập trung vào mũ công tác cải tiến, găng tay chống cháy và quần áo dã ngoại nhiều túi tiện dụng. Tổ kỹ thuật của xưởng đồng thời lấy ý kiến trực tiếp từ các kíp xe để điều chỉnh chi tiết thiết kế.</p><p>Quy trình kiểm tra chất lượng đầu ra được siết chặt ngay tại chuyền hoàn thiện, hạn chế tối đa lỗi đường may và sai khác kích cỡ.</p>", "x20, quân trang, sản xuất", new DateTime(2026, 4, 12, 1, 0, 0, DateTimeKind.Utc)),
            new("Tập huấn nghiệp vụ kiểm thực và lưu mẫu thức ăn tại cấp tiểu đoàn", "Đợt tập huấn giúp đội ngũ nuôi quân nắm chắc quy trình kiểm soát nguồn thực phẩm, chế biến và lưu mẫu theo quy định mới.", "<p>Nội dung tập huấn bao gồm nhận biết thực phẩm không bảo đảm, quy trình lưu mẫu thức ăn đủ thời gian, ghi chép sổ kiểm thực và xử trí khi phát hiện dấu hiệu mất an toàn thực phẩm.</p><p>Sau tập huấn, các đơn vị sẽ đồng loạt rà soát lại hệ thống tủ lưu mẫu, biểu mẫu sổ sách và phân công cán bộ trực tiếp chịu trách nhiệm từng khâu.</p>", "kiểm thực, an toàn thực phẩm, tập huấn", new DateTime(2026, 4, 14, 1, 0, 0, DateTimeKind.Utc)),
            new("Bổ sung khẩu phần giàu năng lượng cho kíp xe huấn luyện cường độ cao", "Các đơn vị đang thử nghiệm thực đơn tăng cường đạm, rau xanh và nước điện giải cho bộ đội làm nhiệm vụ kéo dài trên xe tăng.", "<p>Thực đơn được xây dựng trên cơ sở định mức hiện hành nhưng linh hoạt thay đổi theo cường độ huấn luyện và điều kiện thời tiết. Những bữa ăn sau huấn luyện chiều được tăng cường thực phẩm giàu đạm và vitamin.</p><p>Nhiều đầu bếp quân đội đã sáng tạo thêm món ăn phù hợp khẩu vị bộ đội, dễ bảo quản và thuận tiện chế biến tại thao trường.</p>", "khẩu phần, huấn luyện, bộ đội", new DateTime(2026, 4, 16, 1, 0, 0, DateTimeKind.Utc)),
            new("Xưởng sửa chữa xe tăng nâng năng lực gia công phụ tùng thay thế", "Đơn vị đã hoàn chỉnh dây chuyền gia công bánh chịu nặng, chốt liên kết và bộ gioăng kỹ thuật phục vụ bảo đảm huấn luyện cao điểm.", "<p>Song song với sửa chữa thường xuyên, xưởng tập trung dự trữ nhóm phụ tùng hao mòn nhanh để rút ngắn thời gian khắc phục hỏng hóc. Năng lực gia công mới giúp chủ động hơn trong bảo đảm kỹ thuật cho nhiều dòng xe tăng đang khai thác.</p><p>Việc quản lý kho phụ tùng cũng được số hóa từng bước, thuận lợi cho tra cứu, cấp phát và theo dõi vòng đời sử dụng.</p>", "phụ tùng, sửa chữa, kỹ thuật", new DateTime(2026, 4, 18, 1, 0, 0, DateTimeKind.Utc)),
            new("Đẩy mạnh xây dựng nhà ăn mẫu, cảnh quan sáng - xanh - sạch - gọn", "Phong trào chỉnh trang doanh trại gắn với nâng chất lượng phục vụ bữa ăn đang được triển khai đồng bộ từ cấp đại đội đến cấp lữ đoàn.", "<p>Nhiều đơn vị chủ động bố trí lại khu chế biến, bàn chia thức ăn, hệ thống rửa tay và khu vực ăn uống thông thoáng. Cảnh quan khu bếp, nhà ăn được cải tạo đồng bộ, góp phần nâng cao tinh thần bộ đội.</p><p>Kết quả bước đầu cho thấy mô hình nhà ăn mẫu giúp tăng tính chính quy, tạo nền nếp tốt trong công tác nuôi quân thường xuyên.</p>", "nhà ăn mẫu, doanh trại, chính quy", new DateTime(2026, 4, 20, 1, 0, 0, DateTimeKind.Utc)),
            new("Cục Hậu cần kiểm tra công tác bảo quản lương thực trong điều kiện nồm ẩm", "Đợt kiểm tra chuyên đề tập trung vào chống ẩm, chống mốc và duy trì định mức dự trữ tại kho các đơn vị phía Bắc.", "<p>Các tổ kiểm tra rà soát hệ thống kê lót, thông gió, hút ẩm và việc sắp xếp hàng hóa cách tường, cách nền đúng quy định. Một số mô hình kho cải tiến đã cho thấy hiệu quả rõ rệt trong mùa nồm ẩm kéo dài.</p><p>Qua kiểm tra, các đơn vị cũng được hướng dẫn thống nhất cách ghi thẻ kho và tổ chức luân chuyển vật chất theo nguyên tắc nhập trước xuất trước.</p>", "bảo quản lương thực, kho tàng, nồm ẩm", new DateTime(2026, 4, 22, 1, 0, 0, DateTimeKind.Utc)),
            new("Lữ đoàn 201 phát động đợt thi đua bảo đảm bếp ăn sạch, suất ăn tốt", "Phong trào thi đua mới hướng vào nâng cao chất lượng phục vụ, giảm hao hụt nguyên liệu và giữ vững an toàn vệ sinh thực phẩm.", "<p>Đợt thi đua kéo dài trong toàn mùa huấn luyện cao điểm, lấy chất lượng bữa ăn và tính nền nếp của khu bếp làm tiêu chí chấm điểm trọng tâm. Từng tiểu đoàn đăng ký mô hình bếp ăn sạch, thực hiện đầy đủ chế độ kiểm thực, lưu mẫu.</p><p>Bên cạnh kết quả thi đua, phong trào còn tạo động lực để cán bộ, nhân viên hậu cần tiếp tục đề xuất các sáng kiến nhỏ nhưng hiệu quả ngay tại cơ sở.</p>", "thi đua, lữ đoàn 201, bếp ăn", new DateTime(2026, 4, 24, 1, 0, 0, DateTimeKind.Utc))
        };
    }

    private static IReadOnlyList<BlogSeed> GetDocumentSeeds()
    {
        return new List<BlogSeed>
        {
            new("[124/QĐ-HC] Điều chỉnh định mức vật tư sản xuất quân dụng năm 2024", "Quyết định điều chỉnh định mức nguyên liệu đầu vào phục vụ sản xuất quân dụng trong điều kiện huấn luyện tăng cường.", "<p>Quyết định quy định việc điều chỉnh tăng định mức một số loại vải kỹ thuật, vật liệu chịu mài mòn và phụ kiện may mặc chuyên dùng để đáp ứng yêu cầu huấn luyện với cường độ cao.</p><p>Các đơn vị sản xuất và tiếp nhận quân trang có trách nhiệm rà soát kế hoạch, bảo đảm nguyên liệu đúng tiêu chuẩn và hạch toán vật tư theo định mức mới.</p>", "quyết định, vật tư, quân dụng", new DateTime(2026, 4, 1, 2, 0, 0, DateTimeKind.Utc)),
            new("[45/CT-QN] Tăng cường kiểm tra chất lượng quân trang nhập kho", "Chỉ thị siết chặt quy trình nghiệm thu quân trang, bảo đảm đồng bộ về chất lượng, mẫu mã và hồ sơ kiểm định.", "<p>Các đơn vị tiếp nhận quân trang phải thành lập tổ giám sát độc lập, đối chiếu hồ sơ kỹ thuật, mẫu chuẩn và số lượng thực tế trước khi ký nhận nhập kho.</p><p>Những lô hàng không đúng quy cách, không rõ nguồn gốc hoặc sai khác màu sắc, chất liệu phải được lập biên bản xử lý ngay từ khâu đầu vào.</p>", "chỉ thị, quân trang, nhập kho", new DateTime(2026, 4, 3, 2, 0, 0, DateTimeKind.Utc)),
            new("[89/TB-BTL] Kế hoạch đấu thầu cung ứng nguyên liệu dệt may quý III", "Thông báo tổ chức đấu thầu rộng rãi cho các gói nguyên liệu dệt may phục vụ sản xuất quân trang và đồ bảo hộ chuyên ngành.", "<p>Thông báo xác định rõ danh mục nguyên liệu trọng điểm như vải chịu mài mòn, sợi tổng hợp, da kỹ thuật và thuốc nhuộm chống cháy cấp độ 2.</p><p>Đơn vị tham gia phải bảo đảm hồ sơ pháp lý đầy đủ, cung cấp mẫu thử đạt yêu cầu và cam kết tiến độ giao hàng theo từng đợt sản xuất.</p>", "thông báo, đấu thầu, dệt may", new DateTime(2026, 4, 5, 2, 0, 0, DateTimeKind.Utc)),
            new("[21/HD-CHC] Hướng dẫn kỹ thuật bảo quản lương thực dã ngoại", "Hướng dẫn thống nhất các biện pháp chống ẩm, chống mốc và tổ chức kho dã ngoại trong điều kiện hành quân, trú quân phân tán.", "<p>Hướng dẫn nêu rõ yêu cầu về kê lót, bao gói, chống thấm, niêm phong và tổ chức luân chuyển lương thực trong môi trường độ ẩm cao hoặc mưa kéo dài.</p><p>Chỉ huy các cấp phải thường xuyên kiểm tra thực tế, kịp thời xử lý điểm đọng ẩm và chủ động dự phòng vật tư bao gói thay thế.</p>", "hướng dẫn, lương thực, dã ngoại", new DateTime(2026, 4, 7, 2, 0, 0, DateTimeKind.Utc)),
            new("[15/QĐ-BTL] Phê duyệt mô hình nhà ăn chính quy mẫu năm 2024", "Quyết định phê duyệt thiết kế và quy chuẩn vận hành nhà ăn kiểu mẫu áp dụng trong toàn Binh chủng.", "<p>Mô hình nhà ăn chính quy mẫu bao gồm yêu cầu về bố trí mặt bằng, dây chuyền bếp một chiều, hệ thống điện, cấp thoát nước và khu rửa dụng cụ.</p><p>Các đơn vị căn cứ điều kiện thực tế để triển khai phù hợp nhưng phải giữ vững yêu cầu cốt lõi về vệ sinh, thông thoáng và tính nền nếp chính quy.</p>", "quyết định, nhà ăn mẫu, chính quy", new DateTime(2026, 4, 9, 2, 0, 0, DateTimeKind.Utc)),
            new("[07/KH-CKT] Kế hoạch dự trữ phụ tùng thay thế phục vụ huấn luyện cao điểm", "Kế hoạch bảo đảm phụ tùng trọng điểm cho xe tăng, xe thiết giáp trong giai đoạn huấn luyện và diễn tập cường độ cao.", "<p>Kế hoạch xác định các nhóm phụ tùng ưu tiên cần dự trữ, trong đó tập trung vào chi tiết hao mòn nhanh, bộ làm kín, bánh chịu tải và một số cụm cơ khí dễ thay thế tại đơn vị cơ sở.</p><p>Các đơn vị kỹ thuật phải đối chiếu thực trạng trang bị, cập nhật số liệu tiêu hao và tổ chức dự trữ theo từng phân cấp bảo đảm.</p>", "kế hoạch, phụ tùng, kỹ thuật", new DateTime(2026, 4, 11, 2, 0, 0, DateTimeKind.Utc)),
            new("[38/TB-CHC] Tổ chức tập huấn an toàn thực phẩm cho đội ngũ nuôi quân", "Thông báo triệu tập lớp tập huấn chuyên sâu về kiểm thực, lưu mẫu và xử lý tình huống mất an toàn thực phẩm tại đơn vị cơ sở.", "<p>Thành phần tham gia gồm cán bộ hậu cần, nhân viên nuôi quân và tổ kiểm thực của các lữ đoàn, nhà trường, xưởng và cơ quan trực thuộc.</p><p>Nội dung tập huấn gắn chặt với các tình huống thực tế, chú trọng trách nhiệm cá nhân trong từng khâu của dây chuyền chế biến.</p>", "thông báo, an toàn thực phẩm, tập huấn", new DateTime(2026, 4, 13, 2, 0, 0, DateTimeKind.Utc)),
            new("[11/HD-CKT] Hướng dẫn bảo quản nhiên liệu diesel chuyên dụng trong mùa lạnh", "Hướng dẫn quy trình kiểm tra, bảo quản và sử dụng nhiên liệu chuyên dụng nhằm bảo đảm an toàn, hạn chế lắng cặn trong điều kiện nhiệt độ thấp.", "<p>Hướng dẫn yêu cầu các đơn vị tổ chức che chắn khu bồn chứa, kiểm tra định kỳ bơm lọc, van xả nước đáy và thực hiện đảo bồn theo đúng quy trình kỹ thuật.</p><p>Việc cấp phát nhiên liệu cho đội hình cơ động phải kết hợp chặt chẽ với tổ kỹ thuật, đảm bảo vừa đủ cơ số vừa hạn chế hao hụt.</p>", "hướng dẫn, diesel, bảo quản", new DateTime(2026, 4, 15, 2, 0, 0, DateTimeKind.Utc)),
            new("[54/QĐ-CHC] Ban hành quy trình lưu trữ hồ sơ nghiệm thu quân nhu", "Quyết định ban hành quy trình mới về lưu trữ hồ sơ, mẫu chuẩn và biên bản nghiệm thu đối với vật chất quân nhu toàn Binh chủng.", "<p>Quy trình mới thống nhất thời hạn lưu trữ, biểu mẫu theo dõi và trách nhiệm bàn giao hồ sơ giữa cơ quan cấp trên với đơn vị tiếp nhận trực tiếp.</p><p>Nội dung đặc biệt nhấn mạnh việc lưu mẫu chuẩn theo lô, phục vụ đối chiếu khi phát sinh phản ánh về chất lượng sau cấp phát.</p>", "quyết định, hồ sơ, quân nhu", new DateTime(2026, 4, 17, 2, 0, 0, DateTimeKind.Utc)),
            new("[26/TB-BTL] Phát động đợt thi đua xây dựng kho tàng sáng - sạch - an toàn", "Thông báo phát động phong trào thi đua cải tiến kho tàng, nhà ăn và khu bảo quản vật chất hậu cần trong toàn lực lượng.", "<p>Đợt thi đua tập trung vào ba nhóm nội dung: sắp xếp kho khoa học, chủ động chống ẩm mốc và chuẩn hóa biển bảng, sổ sách theo quy định hiện hành.</p><p>Kết quả thi đua sẽ là căn cứ đánh giá cuối năm đối với các cơ quan, đơn vị trực thuộc và các tập thể hậu cần cơ sở.</p>", "thông báo, kho tàng, thi đua", new DateTime(2026, 4, 19, 2, 0, 0, DateTimeKind.Utc))
        };
    }

    private static IReadOnlyList<ProductSeed> GetProductSeeds()
    {
        return new List<ProductSeed>
        {
            new("TTG-DEMO-BEPDIEN-24", "Hệ thống bếp điện TTG-24", "15 bộ", "Hệ thống bếp điện công nghiệp 3 pha tích hợp 4 nồi nấu dung tích lớn, phù hợp bếp ăn cấp tiểu đoàn và khu chế biến tập trung.", "CHC_TTG", "Cục Hậu cần", "Trang bị - Quân nhu", true, 10, "bếp điện, quân nhu, bếp ăn"),
            new("TTG-DEMO-MU-X20-01", "Mũ công tác xe tăng cải tiến", "500 chiếc", "Mũ công tác cho kíp xe với kết cấu nhẹ, chịu va đập tốt, tích hợp lớp lót thoát nhiệt và khoang lắp thiết bị thông tin cá nhân.", "X20", "Xưởng X20", "Trang bị - Quân nhu", true, 20, "mũ công tác, quân trang, kíp xe"),
            new("TTG-DEMO-THITLON-201", "Thịt lợn sạch LĐ201", "3.500 kg/tháng", "Nguồn thực phẩm chăn nuôi khép kín, kiểm soát thú y chặt chẽ, đáp ứng ổn định cho bếp ăn đơn vị huấn luyện cường độ cao.", "LĐ201", "Lữ đoàn Xe tăng 201", "Thực phẩm tươi sống", true, 30, "thịt lợn, thực phẩm sạch, hậu cần"),
            new("TTG-DEMO-RAU-215", "Rau xanh VietGAP", "200 kg/ngày", "Rau xanh canh tác theo tiêu chuẩn an toàn, sử dụng hệ thống tưới tiết kiệm và luân canh theo mùa nhằm ổn định nguồn cung quanh năm.", "LĐ215", "Lữ đoàn Xe tăng 215", "Thực phẩm tươi sống", true, 40, "rau xanh, vietgap, tăng gia"),
            new("TTG-DEMO-DIESEL-L62", "Dầu Diesel DO-L62", "50.000 lít", "Nhiên liệu chuyên dụng cho động cơ xe tăng, có phụ gia ổn định nhiệt và tăng hiệu suất đốt cháy trong điều kiện vận hành khắc nghiệt.", "CKT_TTG", "Cục Kỹ thuật", "Nhiên liệu - Vật tư kỹ thuật", true, 50, "diesel, nhiên liệu, kỹ thuật"),
            new("TTG-DEMO-QUANAO-X20", "Bộ quần áo công tác TTG", "1.200 bộ", "Quần áo công tác sử dụng vải dệt bền mài mòn, nhiều túi chức năng, phù hợp cho kíp xe, tổ sửa chữa và lực lượng cơ động kỹ thuật.", "X20", "Xưởng X20", "Trang bị - Quân nhu", false, 60, "quần áo công tác, quân trang"),
            new("TTG-DEMO-BANH-T54", "Bánh chịu nặng xe tăng T-54", "40 chiếc", "Phụ tùng thay thế chế tạo từ thép cường độ cao và cao su đúc áp lực, đảm bảo độ bền và giảm chấn khi vận hành tốc độ cao.", "XM_T72", "Xưởng sửa chữa xe tăng", "Phụ tùng - Bảo đảm kỹ thuật", false, 70, "bánh chịu nặng, t54, phụ tùng"),
            new("TTG-DEMO-GANGTAY-X20", "Găng tay bảo hộ kíp xe", "800 đôi", "Găng tay da kết hợp vật liệu chống cháy, tăng độ bám khi thao tác kỹ thuật, phù hợp sử dụng trong khoang chiến đấu và khu sửa chữa.", "X20", "Xưởng X20", "Trang bị - Quân nhu", true, 80, "găng tay, bảo hộ, kíp xe"),
            new("TTG-DEMO-KHAUPHAN-07", "Khẩu phần dã ngoại số 7", "1.500 suất", "Khẩu phần đóng gói gọn nhẹ gồm lương khô, thực phẩm bổ sung và nước điện giải, đáp ứng nhiệm vụ huấn luyện, cơ động kéo dài.", "CHC_TTG", "Cục Hậu cần", "Thực phẩm khô - dự trữ", false, 90, "khẩu phần dã ngoại, lương khô, dự trữ"),
            new("TTG-DEMO-GAO-DUTRU", "Gạo dự trữ đóng túi 10kg", "800 túi", "Lô gạo đóng túi bảo quản kín, thuận lợi cho cấp phát nhanh theo cơ số dã ngoại và duy trì dự trữ tại kho đơn vị cơ sở.", "CHC_TTG", "Cục Hậu cần", "Lương thực chính", false, 100, "gạo dự trữ, lương thực"),
            new("TTG-DEMO-LOCNUOC-01", "Bộ lọc nước dã chiến cá nhân", "250 bộ", "Bộ lọc nước cơ động dùng tại thao trường, trạm trú quân xa hoặc khu vực nguồn nước chưa ổn định, phục vụ bảo đảm nước uống an toàn.", "LĐ405", "Lữ đoàn Xe tăng 405", "Trang bị - Quân nhu", false, 110, "lọc nước, dã chiến, hậu cần"),
            new("TTG-DEMO-GIOANG-T54", "Bộ gioăng làm kín T-54", "120 bộ", "Bộ gioăng đồng bộ phục vụ bảo dưỡng định kỳ và khắc phục hỏng hóc nhanh, giúp duy trì hệ số kỹ thuật cho dòng xe tăng T-54.", "XM_T72", "Xưởng sửa chữa xe tăng", "Phụ tùng - Bảo đảm kỹ thuật", false, 120, "gioăng, t54, sửa chữa")
        };
    }

    private static IReadOnlyList<ForumTopicSeed> GetForumTopicSeeds()
    {
        return new List<ForumTopicSeed>
        {
            new("Kinh nghiệm chống ẩm mốc kho quân trang trong mùa nồm", "Trung tá Hoàng Thị Lan", "Đơn vị tôi đang duy trì nền kho cao, xếp hàng cách tường và kết hợp máy hút ẩm theo khung giờ cố định. Cách này giảm hẳn hiện tượng mốc vải và ố vàng quân trang trong đợt nồm ẩm kéo dài.", new[] { "Các đồng chí nên bổ sung thêm kiểm tra định kỳ gói hút ẩm và cập nhật nhật ký kho theo từng tuần để dễ theo dõi biến động.", "Ở khu vực sát tường, nên bố trí thêm kệ hở và luân chuyển hàng theo nguyên tắc nhập trước xuất trước để tránh tồn đọng dài ngày." }, ForumTopicType.Sticky, new DateTime(2026, 4, 2, 3, 0, 0, DateTimeKind.Utc), 24),
            new("Tổ chức thực đơn tăng cường cho kíp xe huấn luyện dài ngày", "Đại úy Lê Minh Đức", "Kíp xe luyện tập liên tục rất cần bữa ăn giàu năng lượng nhưng vẫn gọn, dễ chia suất. Tôi muốn trao đổi thêm về cách kết hợp thực phẩm tươi với khẩu phần bổ sung trong ngày cao điểm.", new[] { "Một số đơn vị đang tăng thêm bữa phụ bằng sữa, hoa quả và bánh dinh dưỡng ngay sau buổi huấn luyện chiều.", "Khi xây dựng thực đơn, nên ưu tiên món dễ tiêu hóa, ít dầu mỡ nhưng đủ đạm và chất điện giải để bộ đội hồi phục nhanh." }, ForumTopicType.Normal, new DateTime(2026, 4, 4, 3, 0, 0, DateTimeKind.Utc), 17),
            new("Bảo quản nhiên liệu diesel chuyên dụng tại điểm cấp phát dã chiến", "Thiếu tá Nguyễn Văn An", "Ở điều kiện cơ động, việc che chắn bồn chứa và kiểm tra lắng cặn cần làm kỹ hơn nhiều so với kho cố định. Đồng chí nào có mô hình hiệu quả xin chia sẻ thêm.", new[] { "Kinh nghiệm của chúng tôi là bố trí mái che tạm, van xả đáy thuận tiện và duy trì kiểm tra đầu ca, cuối ca để phát hiện bất thường sớm.", "Nếu cấp phát trên nhiều hướng cơ động, nên chia cơ số theo đợt ngắn để giảm thời gian nhiên liệu lưu ngoài trời." }, ForumTopicType.Normal, new DateTime(2026, 4, 6, 3, 0, 0, DateTimeKind.Utc), 13),
            new("Giải pháp quản lý bếp điện công nghiệp tại nhà ăn cấp tiểu đoàn", "Trung tá Trần Văn Thu", "Sau thời gian sử dụng bếp điện công suất lớn, đơn vị tôi nhận thấy cần quy định chặt chẽ hơn về phân công vận hành, vệ sinh và ghi chép tiêu hao điện năng.", new[] { "Nên giao rõ một tổ phụ trách từng ca, có sổ theo dõi thời gian vận hành và lịch bảo dưỡng định kỳ theo tuần.", "Kết hợp bếp điện với bếp dầu dự phòng sẽ giúp đơn vị chủ động hơn khi mất điện đột xuất hoặc khi nấu số lượng lớn." }, ForumTopicType.Normal, new DateTime(2026, 4, 8, 3, 0, 0, DateTimeKind.Utc), 18),
            new("Xây dựng nguồn rau xanh cơ động phục vụ huấn luyện phân tán", "Thiếu tá Ngô Văn Long", "Mô hình vườn rau trên xe vận tải cũ vẫn còn nhiều nội dung cần hoàn thiện, đặc biệt là giá thể, cách cố định khay trồng và duy trì độ ẩm khi hành quân.", new[] { "Nếu có điều kiện, nên dùng khay nhẹ và khung giữ có khóa chốt để giảm rung xóc khi cơ động đường xấu.", "Chúng tôi đang thử thêm phương án trồng rau ngắn ngày theo từng đợt, bù lại bằng rau dự trữ đóng gói ở chặng dài." }, ForumTopicType.Normal, new DateTime(2026, 4, 10, 3, 0, 0, DateTimeKind.Utc), 15),
            new("Quy trình lưu mẫu thức ăn và xử trí khi nghi ngờ mất an toàn thực phẩm", "Thiếu tá Phạm Văn Hòa", "Đề nghị các đồng chí chia sẻ biểu mẫu ghi chép và cách phân công trách nhiệm lưu mẫu thức ăn hằng ngày sao cho chặt chẽ nhưng vẫn gọn việc.", new[] { "Nên thống nhất mỗi ca có một người phụ trách chính, niêm phong mẫu ngay sau khi chia suất ăn và ghi giờ cụ thể trên hộp mẫu.", "Khi có dấu hiệu bất thường, phải giữ nguyên toàn bộ mẫu liên quan, báo cáo ngay chỉ huy và phối hợp quân y xử lý theo đúng quy trình." }, ForumTopicType.Announcement, new DateTime(2026, 4, 12, 3, 0, 0, DateTimeKind.Utc), 21),
            new("Bảo quản mũ công tác tích hợp thiết bị thông tin sau huấn luyện", "Thiếu tá Nguyễn Văn An", "Loại mũ tích hợp tai nghe, micro rất tiện nhưng cũng phát sinh yêu cầu bảo quản riêng. Đề nghị trao đổi cách vệ sinh, hong khô và cất giữ để tránh chập ẩm.", new[] { "Sau mỗi buổi huấn luyện nên tháo phần lót, lau khô cụm tai nghe và để nơi thông thoáng trước khi cất vào tủ riêng.", "Không nên để mũ ở khoang xe quá lâu sau khi hoạt động mưa, dễ gây hấp hơi và ảnh hưởng đến đầu nối thiết bị." }, ForumTopicType.Normal, new DateTime(2026, 4, 14, 3, 0, 0, DateTimeKind.Utc), 12),
            new("Tổ chức kho phụ tùng nhanh tra cứu, dễ cấp phát tại xưởng sửa chữa", "Đại úy Lưu Minh Hải", "Xưởng chúng tôi đang sắp xếp lại kho phụ tùng theo nhóm chi tiết và vòng đời hao mòn. Mục tiêu là tra cứu nhanh hơn khi có xe vào sửa chữa đột xuất.", new[] { "Nên chia kho thành nhóm phụ tùng thay nhanh, nhóm phụ tùng đại tu và nhóm vật tư gia công để giảm thời gian tìm kiếm.", "Thẻ kho cần gắn rõ mã vật tư, dòng xe sử dụng và vị trí giá kệ để thợ mới vẫn có thể lấy đúng chủng loại." }, ForumTopicType.Normal, new DateTime(2026, 4, 16, 3, 0, 0, DateTimeKind.Utc), 14),
            new("Kinh nghiệm chuẩn bị suất ăn cơ động cho hành quân ban đêm", "Đại úy Bùi Văn Nam", "Suất ăn cơ động ban đêm phải gọn, giữ nhiệt tốt và hạn chế thao tác chia phát tại điểm dừng ngắn. Đồng chí nào có mô hình phù hợp xin trao đổi.", new[] { "Nên chuẩn bị theo túi suất ăn đồng bộ gồm món chính, nước uống và đồ bổ sung để phát nhanh theo xe hoặc theo tổ.", "Khâu đóng gói cần làm sớm, kiểm soát nhiệt độ bằng thùng giữ nhiệt và ghi rõ thời điểm sử dụng để tránh kéo dài quá giới hạn an toàn." }, ForumTopicType.Normal, new DateTime(2026, 4, 18, 3, 0, 0, DateTimeKind.Utc), 16),
            new("Đề xuất tiêu chí chấm điểm nhà ăn mẫu cấp đại đội", "Trung tá Vũ Mạnh Cường", "Đơn vị đang xây dựng bộ tiêu chí nội bộ để tự chấm điểm nhà ăn mẫu hằng tháng. Tôi muốn tham khảo thêm các tiêu chí thực tế, dễ đo lường và bám sát nghiệp vụ.", new[] { "Theo tôi nên có ba nhóm tiêu chí chính: cơ sở vật chất, quy trình phục vụ và kết quả thực chất về vệ sinh, khẩu phần, nền nếp bộ đội.", "Nếu lượng hóa được từng tiêu chí theo điểm, việc tự kiểm tra và so sánh giữa các bếp ăn sẽ khách quan hơn rất nhiều." }, ForumTopicType.Normal, new DateTime(2026, 4, 20, 3, 0, 0, DateTimeKind.Utc), 19)
        };
    }

    private sealed record VendorSeed(string Code, string Name, string? ParentCode, int DisplayOrder, string Description, string Email);

    private sealed record CategorySeed(string Name, string Description, string MetaKeywords, int DisplayOrder);

    private sealed record BlogSeed(string Title, string Overview, string Body, string Tags, DateTime CreatedOnUtc);

    private sealed record ProductSeed(
        string Sku,
        string Name,
        string ShortDescription,
        string FullDescription,
        string VendorCode,
        string VendorName,
        string CategoryName,
        bool ShowOnHomepage,
        int DisplayOrder,
        string MetaKeywords,
        DateTime CreatedOnUtc)
    {
        public ProductSeed(
            string sku,
            string name,
            string shortDescription,
            string fullDescription,
            string vendorCode,
            string vendorName,
            string categoryName,
            bool showOnHomepage,
            int displayOrder,
            string metaKeywords)
            : this(sku, name, shortDescription, fullDescription, vendorCode, vendorName, categoryName, showOnHomepage, displayOrder, metaKeywords, new DateTime(2026, 4, 23, 4, 0, 0, DateTimeKind.Utc).AddHours(displayOrder))
        {
        }
    }

    private sealed record ForumTopicSeed(
        string Subject,
        string OriginalAuthor,
        string OpeningPost,
        IReadOnlyList<string> Replies,
        ForumTopicType TopicType,
        DateTime CreatedOnUtc,
        int InitialViews);
}
