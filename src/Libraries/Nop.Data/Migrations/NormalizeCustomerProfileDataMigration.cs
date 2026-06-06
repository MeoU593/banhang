using FluentMigrator;

namespace Nop.Data.Migrations;

[NopMigration("2026/05/27 10:00:00", "Normalize customer profile data", MigrationProcessType.Update)]
public class NormalizeCustomerProfileDataMigration : ForwardOnlyMigration
{
    public override void Up()
    {
        Execute.Sql(@"
            IF OBJECT_ID('Customer', 'U') IS NOT NULL
            BEGIN
                ;WITH Names AS
                (
                    SELECT * FROM (VALUES
                        (0, N'Nguyễn Văn', N'An', N'M'), (1, N'Trần Đức', N'Bình', N'M'),
                        (2, N'Lê Minh', N'Cường', N'M'), (3, N'Phạm Quốc', N'Dũng', N'M'),
                        (4, N'Hoàng Văn', N'Hải', N'M'), (5, N'Đỗ Thanh', N'Hùng', N'M'),
                        (6, N'Bùi Anh', N'Khoa', N'M'), (7, N'Vũ Đức', N'Long', N'M'),
                        (8, N'Đặng Văn', N'Mạnh', N'M'), (9, N'Ngô Quang', N'Nam', N'M'),
                        (10, N'Dương Minh', N'Phong', N'M'), (11, N'Phan Văn', N'Quân', N'M'),
                        (12, N'Mai Đức', N'Sơn', N'M'), (13, N'Tạ Quốc', N'Thắng', N'M'),
                        (14, N'Hà Minh', N'Tuấn', N'M'), (15, N'Cao Văn', N'Việt', N'M'),
                        (16, N'Nguyễn Thị', N'Lan', N'F'), (17, N'Trần Thu', N'Hà', N'F'),
                        (18, N'Lê Thị', N'Hương', N'F'), (19, N'Phạm Minh', N'Ngọc', N'F'),
                        (20, N'Hoàng Thị', N'Trang', N'F'), (21, N'Đỗ Thu', N'Thảo', N'F'),
                        (22, N'Bùi Thị', N'Yến', N'F'), (23, N'Vũ Thị', N'Mai', N'F')
                    ) AS source([Slot], [FirstName], [LastName], [Gender])
                ), CustomerNames AS
                (
                    SELECT c.[Id], n.[FirstName], n.[LastName], n.[Gender]
                    FROM [Customer] c
                    INNER JOIN Names n ON n.[Slot] = ABS(CHECKSUM(c.[Id])) % 24
                    WHERE c.[Deleted] = 0
                )
                UPDATE c
                SET [FirstName] = cn.[FirstName],
                    [LastName] = cn.[LastName],
                    [Gender] = cn.[Gender]
                FROM [Customer] c
                INNER JOIN CustomerNames cn ON cn.[Id] = c.[Id];
            END

            IF OBJECT_ID('CustomerMilitaryProfile', 'U') IS NOT NULL
                AND OBJECT_ID('Customer', 'U') IS NOT NULL
            BEGIN
                ;WITH RoleFlags AS
                (
                    SELECT c.[Id],
                        MAX(CASE WHEN cr.[SystemName] = N'Administrators' THEN 1 ELSE 0 END) AS IsAdministrator,
                        MAX(CASE WHEN cr.[SystemName] = N'Vendors' THEN 1 ELSE 0 END) AS IsUnitAdmin,
                        MAX(CASE WHEN cr.[SystemName] = N'Guests' THEN 1 ELSE 0 END) AS IsGuest
                    FROM [Customer] c
                    LEFT JOIN [Customer_CustomerRole_Mapping] crm ON crm.[Customer_Id] = c.[Id]
                    LEFT JOIN [CustomerRole] cr ON cr.[Id] = crm.[CustomerRole_Id]
                    WHERE c.[Deleted] = 0
                    GROUP BY c.[Id]
                ), ProfileData AS
                (
                    SELECT c.[Id] AS [CustomerId],
                        CASE
                            WHEN ISNULL(r.[IsAdministrator], 0) = 1 THEN N'Đại tá'
                            WHEN ISNULL(r.[IsUnitAdmin], 0) = 1 THEN N'Thượng tá'
                            WHEN c.[VendorId] > 0 THEN
                                CASE ABS(CHECKSUM(c.[Id])) % 5
                                    WHEN 0 THEN N'Thiếu tá'
                                    WHEN 1 THEN N'Đại úy'
                                    WHEN 2 THEN N'Thượng úy'
                                    WHEN 3 THEN N'Trung úy'
                                    ELSE N'Thiếu úy'
                                END
                            ELSE
                                CASE ABS(CHECKSUM(c.[Id])) % 3
                                    WHEN 0 THEN N'Đại úy'
                                    WHEN 1 THEN N'Thượng úy'
                                    ELSE N'Trung úy'
                                END
                        END AS [Rank],
                        COALESCE(NULLIF(v.[Name], N''), NULLIF(c.[Company], N''), N'Cơ quan Binh chủng Tăng Thiết Giáp') AS [UnitName],
                        CASE
                            WHEN ISNULL(r.[IsAdministrator], 0) = 1 THEN N'Quản trị hệ thống'
                            WHEN ISNULL(r.[IsUnitAdmin], 0) = 1 THEN N'Trưởng đơn vị'
                            WHEN ISNULL(r.[IsGuest], 0) = 1 THEN N'Cán bộ đơn vị'
                            WHEN c.[VendorId] > 0 THEN N'Nhân sự đơn vị'
                            ELSE N'Cán bộ hệ thống'
                        END AS [PositionTitle],
                        CONCAT(COALESCE(NULLIF(v.[Code], N''), CASE WHEN c.[VendorId] > 0 THEN CONCAT(N'DV', RIGHT(CONCAT(N'000', c.[VendorId]), 3)) ELSE N'HT' END), N'-', RIGHT(CONCAT(N'00000', c.[Id]), 5)) AS [MilitaryCode],
                        DATEFROMPARTS(2002 + ABS(CHECKSUM(c.[Id])) % 18, ABS(CHECKSUM(c.[Id])) % 12 + 1, 1) AS [EnlistmentDate]
                    FROM [Customer] c
                    LEFT JOIN RoleFlags r ON r.[Id] = c.[Id]
                    LEFT JOIN [Vendor] v ON v.[Id] = c.[VendorId] AND v.[Deleted] = 0
                    WHERE c.[Deleted] = 0
                )
                UPDATE p
                SET [MilitaryCode] = pd.[MilitaryCode],
                    [Rank] = pd.[Rank],
                    [UnitName] = pd.[UnitName],
                    [PositionTitle] = pd.[PositionTitle],
                    [EnlistmentDate] = COALESCE(p.[EnlistmentDate], pd.[EnlistmentDate]),
                    [UpdatedOnUtc] = SYSUTCDATETIME()
                FROM [CustomerMilitaryProfile] p
                INNER JOIN ProfileData pd ON pd.[CustomerId] = p.[CustomerId];

                ;WITH RoleFlags AS
                (
                    SELECT c.[Id],
                        MAX(CASE WHEN cr.[SystemName] = N'Administrators' THEN 1 ELSE 0 END) AS IsAdministrator,
                        MAX(CASE WHEN cr.[SystemName] = N'Vendors' THEN 1 ELSE 0 END) AS IsUnitAdmin,
                        MAX(CASE WHEN cr.[SystemName] = N'Guests' THEN 1 ELSE 0 END) AS IsGuest
                    FROM [Customer] c
                    LEFT JOIN [Customer_CustomerRole_Mapping] crm ON crm.[Customer_Id] = c.[Id]
                    LEFT JOIN [CustomerRole] cr ON cr.[Id] = crm.[CustomerRole_Id]
                    WHERE c.[Deleted] = 0
                    GROUP BY c.[Id]
                ), ProfileData AS
                (
                    SELECT c.[Id] AS [CustomerId],
                        CASE
                            WHEN ISNULL(r.[IsAdministrator], 0) = 1 THEN N'Đại tá'
                            WHEN ISNULL(r.[IsUnitAdmin], 0) = 1 THEN N'Thượng tá'
                            WHEN c.[VendorId] > 0 THEN
                                CASE ABS(CHECKSUM(c.[Id])) % 5
                                    WHEN 0 THEN N'Thiếu tá'
                                    WHEN 1 THEN N'Đại úy'
                                    WHEN 2 THEN N'Thượng úy'
                                    WHEN 3 THEN N'Trung úy'
                                    ELSE N'Thiếu úy'
                                END
                            ELSE
                                CASE ABS(CHECKSUM(c.[Id])) % 3
                                    WHEN 0 THEN N'Đại úy'
                                    WHEN 1 THEN N'Thượng úy'
                                    ELSE N'Trung úy'
                                END
                        END AS [Rank],
                        COALESCE(NULLIF(v.[Name], N''), NULLIF(c.[Company], N''), N'Cơ quan Binh chủng Tăng Thiết Giáp') AS [UnitName],
                        CASE
                            WHEN ISNULL(r.[IsAdministrator], 0) = 1 THEN N'Quản trị hệ thống'
                            WHEN ISNULL(r.[IsUnitAdmin], 0) = 1 THEN N'Trưởng đơn vị'
                            WHEN ISNULL(r.[IsGuest], 0) = 1 THEN N'Cán bộ đơn vị'
                            WHEN c.[VendorId] > 0 THEN N'Nhân sự đơn vị'
                            ELSE N'Cán bộ hệ thống'
                        END AS [PositionTitle],
                        CONCAT(COALESCE(NULLIF(v.[Code], N''), CASE WHEN c.[VendorId] > 0 THEN CONCAT(N'DV', RIGHT(CONCAT(N'000', c.[VendorId]), 3)) ELSE N'HT' END), N'-', RIGHT(CONCAT(N'00000', c.[Id]), 5)) AS [MilitaryCode],
                        DATEFROMPARTS(2002 + ABS(CHECKSUM(c.[Id])) % 18, ABS(CHECKSUM(c.[Id])) % 12 + 1, 1) AS [EnlistmentDate]
                    FROM [Customer] c
                    LEFT JOIN RoleFlags r ON r.[Id] = c.[Id]
                    LEFT JOIN [Vendor] v ON v.[Id] = c.[VendorId] AND v.[Deleted] = 0
                    WHERE c.[Deleted] = 0
                )
                INSERT INTO [CustomerMilitaryProfile] ([CustomerId], [MilitaryCode], [Rank], [UnitName], [PositionTitle], [EnlistmentDate], [CreatedOnUtc], [UpdatedOnUtc])
                SELECT pd.[CustomerId], pd.[MilitaryCode], pd.[Rank], pd.[UnitName], pd.[PositionTitle], pd.[EnlistmentDate], SYSUTCDATETIME(), SYSUTCDATETIME()
                FROM ProfileData pd
                WHERE NOT EXISTS (SELECT 1 FROM [CustomerMilitaryProfile] p WHERE p.[CustomerId] = pd.[CustomerId]);
            END
        ");
    }
}
