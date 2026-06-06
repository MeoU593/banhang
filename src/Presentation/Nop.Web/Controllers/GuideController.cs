using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models.Guide;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Nop.Web.Controllers;

public partial class GuideController : BasePublicController
{
    private const string UserGuideTopicSystemName = "UserGuide";

    protected readonly ITopicModelFactory _topicModelFactory;

    public GuideController(ITopicModelFactory topicModelFactory)
    {
        _topicModelFactory = topicModelFactory;
    }

    public virtual async Task<IActionResult> Index()
    {
        var topic = await _topicModelFactory.PrepareTopicModelBySystemNameAsync(UserGuideTopicSystemName);
        var model = new UserGuideModel
        {
            TopicSystemName = UserGuideTopicSystemName,
            CustomTitle = topic?.Title ?? string.Empty,
            CustomBody = topic?.Body ?? string.Empty,
            Sections = PrepareTopicSections(topic?.Body).Any() ? PrepareTopicSections(topic?.Body) : PrepareDefaultSections()
        };

        return View(model);
    }

    protected virtual IList<UserGuideSectionModel> PrepareTopicSections(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return new List<UserGuideSectionModel>();

        var headingMatches = Regex.Matches(body, @"<h[2-3][^>]*>(.*?)</h[2-3]>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        if (headingMatches.Count == 0)
            return new List<UserGuideSectionModel>();

        var result = new List<UserGuideSectionModel>();
        for (var i = 0; i < headingMatches.Count; i++)
        {
            var heading = headingMatches[i];
            var title = StripHtml(heading.Groups[1].Value);
            if (string.IsNullOrWhiteSpace(title))
                continue;

            var sectionStart = heading.Index + heading.Length;
            var sectionEnd = i + 1 < headingMatches.Count ? headingMatches[i + 1].Index : body.Length;
            var sectionHtml = body.Substring(sectionStart, Math.Max(0, sectionEnd - sectionStart));
            var paragraphs = Regex.Matches(sectionHtml, @"<p[^>]*>(.*?)</p>", RegexOptions.IgnoreCase | RegexOptions.Singleline)
                .Select(match => StripHtml(match.Groups[1].Value))
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .ToList();
            var steps = Regex.Matches(sectionHtml, @"<li[^>]*>(.*?)</li>", RegexOptions.IgnoreCase | RegexOptions.Singleline)
                .Select(match => StripHtml(match.Groups[1].Value))
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .ToList();

            if (!steps.Any() && paragraphs.Count > 1)
                steps = paragraphs.Skip(1).ToList();

            var style = GetSectionStyle(title);
            result.Add(new UserGuideSectionModel
            {
                Anchor = BuildAnchor(title),
                Icon = style.Icon,
                IconColorClass = style.IconColorClass,
                Title = title,
                Description = paragraphs.FirstOrDefault() ?? "Hướng dẫn thao tác trong hệ thống.",
                Steps = steps.Any() ? steps : new List<string> { "Theo dõi nội dung hướng dẫn được cập nhật trong Admin." }
            });
        }

        return result;
    }

    protected virtual (string Icon, string IconColorClass) GetSectionStyle(string title)
    {
        var normalized = NormalizeForSearch(title);
        if (normalized.Contains("dang nhap") || normalized.Contains("dang ky"))
            return ("login", "text-emerald-600");
        if (normalized.Contains("ho so") || normalized.Contains("ca nhan"))
            return ("account_circle", "text-lime-600");
        if (normalized.Contains("san pham") || normalized.Contains("nang luc"))
            return ("restaurant", "text-amber-600");
        if (normalized.Contains("tin tuc") || normalized.Contains("van ban"))
            return ("article", "text-purple-600");
        if (normalized.Contains("tai lieu") || normalized.Contains("kho"))
            return ("folder_open", "text-teal-600");
        if (normalized.Contains("dien dan"))
            return ("forum", "text-pink-600");
        if (normalized.Contains("danh ba"))
            return ("contacts", "text-sky-600");
        if (normalized.Contains("quan tri"))
            return ("admin_panel_settings", "text-blue-600");
        if (normalized.Contains("import") || normalized.Contains("them"))
            return ("upload_file", "text-orange-600");

        return ("support_agent", "text-rose-600");
    }

    protected virtual string StripHtml(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var noTags = Regex.Replace(value, "<.*?>", " ", RegexOptions.Singleline);
        noTags = WebUtility.HtmlDecode(noTags);
        return Regex.Replace(noTags, @"\s+", " ").Trim();
    }

    protected virtual string BuildAnchor(string title)
    {
        var normalized = NormalizeForSearch(title);
        var anchor = Regex.Replace(normalized, "[^a-z0-9]+", "-").Trim('-');
        return string.IsNullOrWhiteSpace(anchor) ? "huong-dan" : anchor;
    }

    protected virtual string NormalizeForSearch(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);
        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
                continue;

            builder.Append(c == 'đ' ? 'd' : c);
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }

    protected virtual IList<UserGuideSectionModel> PrepareDefaultSections()
    {
        return new List<UserGuideSectionModel>
        {
            new()
            {
                Anchor = "dang-nhap-dang-ky",
                Icon = "login",
                IconColorClass = "text-emerald-600",
                Title = "Đăng nhập / đăng ký",
                Description = "Hướng dẫn truy cập hệ thống, tạo tài khoản và chờ phê duyệt.",
                Steps = new List<string>
                {
                    "Mở trang đăng nhập từ sidebar hoặc màn hình chính.",
                    "Nhập đúng email/tên đăng nhập và mật khẩu được cấp.",
                    "Nếu chưa có tài khoản, dùng trang đăng ký và chọn đúng đơn vị công tác."
                }
            },
            new()
            {
                Anchor = "ho-so-ca-nhan",
                Icon = "account_circle",
                IconColorClass = "text-lime-600",
                Title = "Cập nhật hồ sơ cá nhân",
                Description = "Cập nhật thông tin quân nhân, liên hệ và ảnh đại diện.",
                Steps = new List<string>
                {
                    "Vào mục Hồ sơ cá nhân trên sidebar.",
                    "Cập nhật họ tên, số điện thoại, cấp bậc, chức vụ và số hiệu quân nhân.",
                    "Sử dụng ô ảnh đại diện trong bảng thông tin để thêm hoặc thay ảnh."
                }
            },
            new()
            {
                Anchor = "san-pham-nang-luc",
                Icon = "restaurant",
                IconColorClass = "text-amber-600",
                Title = "Sản phẩm / năng lực",
                Description = "Tra cứu danh mục sản phẩm, năng lực hậu cần và thông tin người đăng.",
                Steps = new List<string>
                {
                    "Mở mục Sản phẩm / Năng lực trên sidebar.",
                    "Dùng bộ lọc hoặc ô tìm kiếm để thu hẹp danh sách.",
                    "Mở chi tiết sản phẩm để xem mô tả và người đăng sản phẩm."
                }
            },
            new()
            {
                Anchor = "tin-tuc-van-ban",
                Icon = "article",
                IconColorClass = "text-purple-600",
                Title = "Tin tức / văn bản",
                Description = "Theo dõi thông báo, tin hoạt động và văn bản mới ban hành.",
                Steps = new List<string>
                {
                    "Mở Tin tức & Văn bản từ sidebar.",
                    "Chọn tab phù hợp để xem tin tức hoặc văn bản.",
                    "Dùng bộ lọc đơn vị, tác giả hoặc thời gian nếu trang hỗ trợ."
                }
            },
            new()
            {
                Anchor = "kho-tai-lieu",
                Icon = "folder_open",
                IconColorClass = "text-teal-600",
                Title = "Kho tài liệu",
                Description = "Tra cứu tài liệu nghiệp vụ, tải file và xem thông tin ban hành.",
                Steps = new List<string>
                {
                    "Mở Kho tài liệu trên sidebar.",
                    "Tìm kiếm theo từ khóa hoặc lọc theo loại tài liệu.",
                    "Mở chi tiết để xem nội dung, tải file nếu được phân quyền."
                }
            },
            new()
            {
                Anchor = "dien-dan",
                Icon = "forum",
                IconColorClass = "text-pink-600",
                Title = "Diễn đàn",
                Description = "Trao đổi nghiệp vụ, theo dõi chủ đề và phản hồi thảo luận.",
                Steps = new List<string>
                {
                    "Mở Diễn đàn từ sidebar.",
                    "Chọn chuyên mục hoặc xem thảo luận đang hoạt động.",
                    "Tạo chủ đề/phản hồi nếu tài khoản được cấp quyền."
                }
            },
            new()
            {
                Anchor = "danh-ba-don-vi",
                Icon = "contacts",
                IconColorClass = "text-sky-600",
                Title = "Danh bạ đơn vị",
                Description = "Tra cứu hồ sơ liên hệ nhân sự theo cây đơn vị.",
                Steps = new List<string>
                {
                    "Mở Tiện ích rồi chọn Danh bạ.",
                    "Chọn đơn vị ở cây bên trái.",
                    "Chọn nhân sự để xem tên, quân hàm, chức vụ, số điện thoại, số hiệu và đơn vị."
                }
            },
            new()
            {
                Anchor = "quan-tri-don-vi",
                Icon = "admin_panel_settings",
                IconColorClass = "text-blue-600",
                Title = "Quản trị đơn vị",
                Description = "Dành cho trưởng đơn vị hoặc tài khoản được cấp quyền quản trị.",
                Steps = new List<string>
                {
                    "Mở Quản trị đơn vị từ sidebar nếu tài khoản có quyền.",
                    "Vào chi tiết đơn vị để xem thống kê, sản phẩm, tin tức, tài liệu, báo cáo và nhân sự.",
                    "Dùng các nút quản lý để cập nhật thông tin theo phân quyền."
                }
            },
            new()
            {
                Anchor = "import-danh-ba",
                Icon = "upload_file",
                IconColorClass = "text-orange-600",
                Title = "Thêm / import nhân sự",
                Description = "Chuẩn bị dữ liệu nhân sự cho đơn vị bằng nhập tay hoặc Excel.",
                Steps = new List<string>
                {
                    "Trong Admin, mở Chi tiết đơn vị rồi chọn Quản lý nhân sự.",
                    "Thêm nhân sự thủ công hoặc import file Excel theo mẫu cột trên trang.",
                    "Kiểm tra trạng thái hiển thị public trước khi lưu dữ liệu."
                }
            },
            new()
            {
                Anchor = "ho-tro",
                Icon = "support_agent",
                IconColorClass = "text-rose-600",
                Title = "Hỗ trợ sử dụng",
                Description = "Kênh liên hệ khi cần hỗ trợ tài khoản, phân quyền hoặc dữ liệu.",
                Steps = new List<string>
                {
                    "Kiểm tra thông báo lỗi hoặc quyền truy cập đang thiếu.",
                    "Liên hệ cán bộ phụ trách hệ thống của đơn vị.",
                    "Cung cấp đường dẫn trang, thao tác đã thực hiện và ảnh chụp lỗi nếu có."
                }
            }
        };
    }
}
