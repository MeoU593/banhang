using FluentMigrator;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopUpdateMigration("2026-06-01 10:00:00", "5.00", UpdateMigrationType.Data)]
public class SeedUnitDirectoryEntriesMigration : Migration
{
    private static readonly string[] FamilyNames =
    {
        "Nguyễn", "Trần", "Lê", "Phạm", "Hoàng", "Đỗ", "Vũ", "Bùi", "Đặng", "Đinh",
        "Phan", "Hồ", "Cao", "Mai", "Tạ", "Dương", "Lý", "Trịnh", "Chu", "Đào"
    };

    private static readonly string[] MiddleNames =
    {
        "Văn", "Đức", "Minh", "Quang", "Hữu", "Đình", "Xuân", "Tiến", "Công", "Anh",
        "Thị", "Thu", "Ngọc", "Thanh", "Hoài", "Khánh", "Bảo", "Gia", "Mạnh", "Việt"
    };

    private static readonly string[] GivenNames =
    {
        "Nam", "Hùng", "Tuấn", "Dũng", "Long", "Sơn", "Hải", "Thắng", "Cường", "Phúc",
        "Hà", "Lan", "Hương", "Linh", "Trang", "Mai", "Thảo", "Loan", "Yến", "Nhung",
        "Khoa", "Tùng", "Hiếu", "Bình", "Kiên", "Quân", "Thành", "Đạt", "Phong", "Nghĩa"
    };

    private static readonly string[] Ranks =
    {
        "Thiếu úy", "Trung úy", "Thượng úy", "Đại úy", "Thiếu tá", "Trung tá", "Thượng tá"
    };

    private static readonly string[] Positions =
    {
        "Nhân viên quân nhu", "Nhân viên hậu cần", "Nhân viên thống kê", "Nhân viên văn thư", "Trợ lý hậu cần",
        "Trợ lý quân nhu", "Nhân viên kho", "Nhân viên kỹ thuật", "Cán bộ tổng hợp", "Cán bộ quản lý"
    };

    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        var vendorRepository = EngineContext.Current.Resolve<IRepository<Vendor>>();
        var directoryRepository = EngineContext.Current.Resolve<IRepository<UnitDirectoryEntry>>();

        var vendors = vendorRepository.Table
            .Where(vendor => !vendor.Deleted && vendor.Active)
            .OrderBy(vendor => vendor.Path ?? vendor.Name)
            .ThenBy(vendor => vendor.DisplayOrder)
            .ThenBy(vendor => vendor.Name)
            .ToList();

        var now = DateTime.UtcNow;
        foreach (var vendor in vendors)
        {
            for (var i = 1; i <= 50; i++)
            {
                var militaryCode = $"DV{vendor.Id:D3}-DB{i:D3}";
                if (directoryRepository.Table.Any(entry => entry.VendorId == vendor.Id && entry.MilitaryCode == militaryCode))
                    continue;

                var nameIndex = vendor.Id + i;
                var fullName = $"{FamilyNames[nameIndex % FamilyNames.Length]} {MiddleNames[(nameIndex * 3) % MiddleNames.Length]} {GivenNames[(nameIndex * 5) % GivenNames.Length]}";
                var rank = Ranks[(i + vendor.Level) % Ranks.Length];
                var position = Positions[(i + vendor.Id) % Positions.Length];

                directoryRepository.InsertAsync(new UnitDirectoryEntry
                {
                    VendorId = vendor.Id,
                    FullName = fullName,
                    Rank = rank,
                    PositionTitle = position,
                    Phone = $"09{vendor.Id % 100:D2}{i:D6}",
                    MilitaryCode = militaryCode,
                    UnitName = vendor.Name,
                    DisplayOrder = i,
                    Published = true,
                    Biography = BuildBiography(fullName, rank, position, vendor.Name, i),
                    CreatedOnUtc = now,
                    UpdatedOnUtc = now
                }).GetAwaiter().GetResult();
            }
        }
    }

    public override void Down()
    {
    }

    private static string BuildBiography(string fullName, string rank, string position, string unitName, int index)
    {
        return $"{fullName}, {rank}, hiện giữ chức vụ {position} tại {unitName}.\n"
            + $"Quá trình công tác: được phân công nhiệm vụ tại đơn vị từ năm {2010 + index % 12}, tham gia bảo đảm hậu cần, quân nhu và công tác quản lý nội bộ.\n"
            + "Lý lịch bản thân và gia đình: thông tin mẫu phục vụ kiểm thử, không sử dụng như dữ liệu hồ sơ thật.";
    }
}
