using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopUpdateMigration("2026-05-27 16:00:00", "5.00", UpdateMigrationType.Data)]
public class SeedUnitAccountsAndProfilesMigration : Migration
{
    private const string DefaultPassword = "123456";
    private const string SeedDomain = "ttg.local";

    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        var vendorRepository = EngineContext.Current.Resolve<IRepository<Vendor>>();
        var customerRepository = EngineContext.Current.Resolve<IRepository<Customer>>();
        var customerRoleRepository = EngineContext.Current.Resolve<IRepository<CustomerRole>>();
        var roleMappingRepository = EngineContext.Current.Resolve<IRepository<CustomerCustomerRoleMapping>>();
        var passwordRepository = EngineContext.Current.Resolve<IRepository<CustomerPassword>>();
        var militaryProfileRepository = EngineContext.Current.Resolve<IRepository<CustomerMilitaryProfile>>();

        var now = DateTime.UtcNow;
        var registeredRoleId = customerRoleRepository.Table
            .Where(role => role.SystemName == NopCustomerDefaults.RegisteredRoleName)
            .Select(role => role.Id)
            .FirstOrDefault();
        var vendorsRoleId = customerRoleRepository.Table
            .Where(role => role.SystemName == NopCustomerDefaults.VendorsRoleName)
            .Select(role => role.Id)
            .FirstOrDefault();

        var vendors = vendorRepository.Table
            .Where(vendor => !vendor.Deleted && vendor.Active)
            .OrderBy(vendor => vendor.Path ?? vendor.Name)
            .ThenBy(vendor => vendor.DisplayOrder)
            .ThenBy(vendor => vendor.Name)
            .ToList();

        foreach (var vendor in vendors)
            EnsureVendorAccounts(vendor, customerRepository, vendorRepository, roleMappingRepository, passwordRepository,
                militaryProfileRepository, registeredRoleId, vendorsRoleId, now);

        BackfillExistingCustomers(customerRepository, vendorRepository, militaryProfileRepository, now);
    }

    public override void Down()
    {
    }

    private static void EnsureVendorAccounts(
        Vendor vendor,
        IRepository<Customer> customerRepository,
        IRepository<Vendor> vendorRepository,
        IRepository<CustomerCustomerRoleMapping> roleMappingRepository,
        IRepository<CustomerPassword> passwordRepository,
        IRepository<CustomerMilitaryProfile> militaryProfileRepository,
        int registeredRoleId,
        int vendorsRoleId,
        DateTime now)
    {
        var leader = vendor.PmCustomerId.HasValue
            ? customerRepository.Table.FirstOrDefault(customer => customer.Id == vendor.PmCustomerId.Value && !customer.Deleted)
            : null;

        if (leader == null)
        {
            leader = EnsureCustomer(vendor, "truongdonvi", 1, "Nguyễn Văn", "Thành", "M", "Thượng tá", "Trưởng đơn vị",
                customerRepository, passwordRepository, militaryProfileRepository, now);
            vendor.PmCustomerId = leader.Id;
            vendorRepository.UpdateAsync(vendor).GetAwaiter().GetResult();
        }
        else
        {
            FillCustomerInfo(leader, vendor, "Nguyễn Văn", "Thành", "M", 1, now);
            customerRepository.UpdateAsync(leader).GetAwaiter().GetResult();
            EnsureMilitaryProfile(leader, vendor, "Thượng tá", "Trưởng đơn vị", militaryProfileRepository, now);
        }

        EnsureRoleMapping(leader.Id, registeredRoleId, roleMappingRepository);
        EnsureRoleMapping(leader.Id, vendorsRoleId, roleMappingRepository);
        EnsurePassword(leader.Id, passwordRepository, now);

        EnsureCustomer(vendor, "nguoidung", 1, "Trần Đức", "Minh", "M", "Thiếu tá", "Người dùng đơn vị",
                customerRepository, passwordRepository, militaryProfileRepository, now);
        EnsureCustomer(vendor, "nguoidung", 2, "Phạm Thu", "Hà", "F", "Đại úy", "Người dùng đơn vị",
                customerRepository, passwordRepository, militaryProfileRepository, now);

        EnsureCustomer(vendor, "nhanvien", 1, "Lê Văn", "Nam", "M", "Thượng úy", "Nhân viên quân nhu",
                customerRepository, passwordRepository, militaryProfileRepository, now);
        EnsureCustomer(vendor, "nhanvien", 2, "Hoàng Minh", "Tuấn", "M", "Trung úy", "Nhân viên thống kê",
                customerRepository, passwordRepository, militaryProfileRepository, now);
        EnsureCustomer(vendor, "nhanvien", 3, "Đỗ Thị", "Lan", "F", "Thiếu úy", "Nhân viên văn thư",
                customerRepository, passwordRepository, militaryProfileRepository, now);

        return;

        Customer EnsureCustomer(
            Vendor unit,
            string roleKey,
            int index,
            string lastName,
            string firstName,
            string gender,
            string rank,
            string positionTitle,
            IRepository<Customer> customers,
            IRepository<CustomerPassword> passwords,
            IRepository<CustomerMilitaryProfile> profiles,
            DateTime timestamp)
        {
            var unitKey = BuildUnitKey(unit);
            var email = $"{roleKey}{index}.{unitKey}@{SeedDomain}";
            var username = $"{roleKey}{index}.{unitKey}";
            var customer = customers.Table.FirstOrDefault(existing => existing.Email == email || existing.Username == username);
            if (customer == null)
            {
                customer = new Customer
                {
                    CustomerGuid = Guid.NewGuid(),
                    Email = email,
                    Username = username,
                    Active = true,
                    Deleted = false,
                    AdminComment = "Tài khoản mẫu PM CDS",
                    CreatedOnUtc = timestamp,
                    LastActivityDateUtc = timestamp,
                    RegisteredInStoreId = 1,
                    VendorId = unit.Id,
                    CompanyId = unit.Id
                };

                FillCustomerInfo(customer, unit, lastName, firstName, gender, index, timestamp);
                customers.InsertAsync(customer).GetAwaiter().GetResult();
            }
            else
            {
                FillCustomerInfo(customer, unit, lastName, firstName, gender, index, timestamp);
                customer.Active = true;
                customer.Deleted = false;
                customers.UpdateAsync(customer).GetAwaiter().GetResult();
            }

            EnsureRoleMapping(customer.Id, registeredRoleId, roleMappingRepository);
            EnsurePassword(customer.Id, passwords, timestamp);
            EnsureMilitaryProfile(customer, unit, rank, positionTitle, profiles, timestamp);

            return customer;
        }
    }

    private static void BackfillExistingCustomers(
        IRepository<Customer> customerRepository,
        IRepository<Vendor> vendorRepository,
        IRepository<CustomerMilitaryProfile> militaryProfileRepository,
        DateTime now)
    {
        var vendorsById = vendorRepository.Table
            .Where(vendor => !vendor.Deleted)
            .ToDictionary(vendor => vendor.Id);
        var leaderCustomerIds = vendorsById.Values
            .Where(vendor => vendor.PmCustomerId.HasValue)
            .Select(vendor => vendor.PmCustomerId.Value)
            .ToHashSet();

        var customers = customerRepository.Table
            .Where(customer => !customer.Deleted)
            .ToList();

        foreach (var customer in customers)
        {
            var vendorId = customer.VendorId > 0 ? customer.VendorId : customer.CompanyId.GetValueOrDefault();
            vendorsById.TryGetValue(vendorId, out var vendor);
            var changed = FillMissingCustomerInfo(customer, vendor, now);
            if (changed)
                customerRepository.UpdateAsync(customer).GetAwaiter().GetResult();

            var rank = leaderCustomerIds.Contains(customer.Id)
                ? "Thượng tá"
                : vendor != null
                    ? GetUnitStaffRank(customer.Id)
                    : GetSystemStaffRank(customer.Id);
            var positionTitle = leaderCustomerIds.Contains(customer.Id)
                ? "Trưởng đơn vị"
                : vendor != null ? "Nhân sự đơn vị" : "Cán bộ hệ thống";
            EnsureMilitaryProfile(customer, vendor, rank, positionTitle, militaryProfileRepository, now);
        }
    }

    private static void FillCustomerInfo(Customer customer, Vendor vendor, string lastName, string firstName, string gender, int index, DateTime now)
    {
        customer.FirstName = lastName;
        customer.LastName = firstName;
        customer.Gender = gender;
        customer.DateOfBirth ??= new DateTime(1982 + index, Math.Min(index + 1, 12), 10 + index);
        customer.Company = string.IsNullOrWhiteSpace(customer.Company) ? vendor.Name : customer.Company;
        customer.CompanyId ??= vendor.Id;
        customer.VendorId = vendor.Id;
        customer.Phone = string.IsNullOrWhiteSpace(customer.Phone) ? BuildPhone(vendor.Id, index) : customer.Phone;
        customer.StreetAddress = string.IsNullOrWhiteSpace(customer.StreetAddress) ? $"Doanh trại {vendor.Name}" : customer.StreetAddress;
        customer.City = string.IsNullOrWhiteSpace(customer.City) ? "Hà Nội" : customer.City;
        customer.County = string.IsNullOrWhiteSpace(customer.County) ? "Binh chủng Tăng Thiết Giáp" : customer.County;
        customer.ZipPostalCode = string.IsNullOrWhiteSpace(customer.ZipPostalCode) ? "100000" : customer.ZipPostalCode;
        customer.TimeZoneId = string.IsNullOrWhiteSpace(customer.TimeZoneId) ? "SE Asia Standard Time" : customer.TimeZoneId;
        customer.LastActivityDateUtc = customer.LastActivityDateUtc == default ? now : customer.LastActivityDateUtc;
    }

    private static bool FillMissingCustomerInfo(Customer customer, Vendor vendor, DateTime now)
    {
        var changed = false;

        void SetStringIfMissing(Func<string> getter, Action<string> setter, string value)
        {
            if (!string.IsNullOrWhiteSpace(getter()))
                return;

            setter(value);
            changed = true;
        }

        var nameSeed = BuildNameSeed(customer);
        if (IsPlaceholderName(customer.FirstName, customer.LastName))
        {
            customer.FirstName = nameSeed.FirstName;
            customer.LastName = nameSeed.LastName;
            changed = true;
        }
        else
        {
            SetStringIfMissing(() => customer.FirstName, value => customer.FirstName = value, nameSeed.FirstName);
            SetStringIfMissing(() => customer.LastName, value => customer.LastName = value, nameSeed.LastName);
        }
        SetStringIfMissing(() => customer.Gender, value => customer.Gender = value, customer.Id % 5 == 0 ? "F" : "M");
        if (!customer.DateOfBirth.HasValue)
        {
            customer.DateOfBirth = new DateTime(1978 + customer.Id % 20, customer.Id % 12 + 1, customer.Id % 20 + 1);
            changed = true;
        }

        if (vendor != null)
        {
            if (customer.VendorId != vendor.Id)
            {
                customer.VendorId = vendor.Id;
                changed = true;
            }

            if (customer.CompanyId != vendor.Id)
            {
                customer.CompanyId = vendor.Id;
                changed = true;
            }

            SetStringIfMissing(() => customer.Company, value => customer.Company = value, vendor.Name);
            SetStringIfMissing(() => customer.StreetAddress, value => customer.StreetAddress = value, $"Doanh trại {vendor.Name}");
        }
        else
        {
            SetStringIfMissing(() => customer.Company, value => customer.Company = value, "Hệ thống quản trị");
            SetStringIfMissing(() => customer.StreetAddress, value => customer.StreetAddress = value, "Cơ quan Binh chủng Tăng Thiết Giáp");
        }

        SetStringIfMissing(() => customer.Phone, value => customer.Phone = value, BuildPhone(Math.Max(customer.VendorId, 1), customer.Id));
        SetStringIfMissing(() => customer.City, value => customer.City = value, "Hà Nội");
        SetStringIfMissing(() => customer.County, value => customer.County = value, "Binh chủng Tăng Thiết Giáp");
        SetStringIfMissing(() => customer.ZipPostalCode, value => customer.ZipPostalCode = value, "100000");
        SetStringIfMissing(() => customer.TimeZoneId, value => customer.TimeZoneId = value, "SE Asia Standard Time");

        if (customer.LastActivityDateUtc == default)
        {
            customer.LastActivityDateUtc = now;
            changed = true;
        }

        return changed;
    }

    private static void EnsureRoleMapping(int customerId, int roleId, IRepository<CustomerCustomerRoleMapping> roleMappingRepository)
    {
        if (customerId <= 0 || roleId <= 0)
            return;

        var exists = roleMappingRepository.Table.Any(mapping => mapping.CustomerId == customerId && mapping.CustomerRoleId == roleId);
        if (exists)
            return;

        roleMappingRepository.InsertAsync(new CustomerCustomerRoleMapping
        {
            CustomerId = customerId,
            CustomerRoleId = roleId
        }).GetAwaiter().GetResult();
    }

    private static void EnsurePassword(int customerId, IRepository<CustomerPassword> passwordRepository, DateTime now)
    {
        if (customerId <= 0 || passwordRepository.Table.Any(password => password.CustomerId == customerId))
            return;

        passwordRepository.InsertAsync(new CustomerPassword
        {
            CustomerId = customerId,
            PasswordFormat = PasswordFormat.Clear,
            PasswordSalt = string.Empty,
            Password = DefaultPassword,
            CreatedOnUtc = now
        }).GetAwaiter().GetResult();
    }

    private static void EnsureMilitaryProfile(
        Customer customer,
        Vendor vendor,
        string rank,
        string positionTitle,
        IRepository<CustomerMilitaryProfile> militaryProfileRepository,
        DateTime now)
    {
        var profile = militaryProfileRepository.Table.FirstOrDefault(existing => existing.CustomerId == customer.Id);
        var unitName = vendor?.Name ?? customer.Company ?? "Hệ thống quản trị";
        var militaryCode = BuildMilitaryCode(customer, vendor);
        if (profile == null)
        {
            militaryProfileRepository.InsertAsync(new CustomerMilitaryProfile
            {
                CustomerId = customer.Id,
                MilitaryCode = militaryCode,
                Rank = rank,
                UnitName = unitName,
                PositionTitle = positionTitle,
                EnlistmentDate = new DateTime(2002 + customer.Id % 18, customer.Id % 12 + 1, 1),
                CreatedOnUtc = now,
                UpdatedOnUtc = now
            }).GetAwaiter().GetResult();
            return;
        }

        var changed = false;
        if (string.IsNullOrWhiteSpace(profile.MilitaryCode))
        {
            profile.MilitaryCode = militaryCode;
            changed = true;
        }

        if (string.IsNullOrWhiteSpace(profile.Rank) || profile.Rank == "Đồng chí")
        {
            profile.Rank = rank;
            changed = true;
        }

        if (string.IsNullOrWhiteSpace(profile.UnitName))
        {
            profile.UnitName = unitName;
            changed = true;
        }

        if (string.IsNullOrWhiteSpace(profile.PositionTitle))
        {
            profile.PositionTitle = positionTitle;
            changed = true;
        }

        if (!profile.EnlistmentDate.HasValue)
        {
            profile.EnlistmentDate = new DateTime(2002 + customer.Id % 18, customer.Id % 12 + 1, 1);
            changed = true;
        }

        if (!changed)
            return;

        profile.UpdatedOnUtc = now;
        militaryProfileRepository.UpdateAsync(profile).GetAwaiter().GetResult();
    }

    private static string BuildUnitKey(Vendor vendor)
    {
        var source = !string.IsNullOrWhiteSpace(vendor.Code) ? vendor.Code : vendor.Name;
        var normalized = source.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();
        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
                continue;

            builder.Append(char.IsLetterOrDigit(c) ? char.ToLowerInvariant(c) : '-');
        }

        var key = Regex.Replace(builder.ToString(), "-+", "-").Trim('-');
        if (string.IsNullOrWhiteSpace(key))
            key = "donvi";

        return $"{key}-{vendor.Id}";
    }

    private static string BuildMilitaryCode(Customer customer, Vendor vendor)
    {
        var prefix = !string.IsNullOrWhiteSpace(vendor?.Code) ? vendor.Code : vendor != null ? $"DV{vendor.Id:000}" : "HT";
        return $"{prefix}-{customer.Id:00000}";
    }

    private static string BuildPhone(int seedA, int seedB)
    {
        var value = Math.Abs(seedA * 7919 + seedB * 3571) % 100000000;
        return $"09{value:00000000}";
    }

    private static string GetUnitStaffRank(int customerId)
    {
        var ranks = new[] { "Thiếu tá", "Đại úy", "Thượng úy", "Trung úy", "Thiếu úy" };
        return ranks[Math.Abs(customerId) % ranks.Length];
    }

    private static string GetSystemStaffRank(int customerId)
    {
        var ranks = new[] { "Đại úy", "Thượng úy", "Trung úy" };
        return ranks[Math.Abs(customerId) % ranks.Length];
    }

    private static bool IsPlaceholderName(string firstName, string lastName)
    {
        var value = $"{firstName} {lastName}".Trim();
        return string.IsNullOrWhiteSpace(value)
            || value.Any(char.IsDigit)
            || value.Contains("hieu", StringComparison.OrdinalIgnoreCase)
            || value.Contains("cán bộ", StringComparison.OrdinalIgnoreCase)
            || value.Contains("can bo", StringComparison.OrdinalIgnoreCase)
            || value.Contains("builtin", StringComparison.OrdinalIgnoreCase);
    }

    private static (string FirstName, string LastName) BuildNameSeed(Customer customer)
    {
        var names = new[]
        {
            ("Nguyễn Văn", "An"), ("Trần Đức", "Bình"), ("Lê Minh", "Cường"), ("Phạm Quốc", "Dũng"),
            ("Hoàng Văn", "Hải"), ("Đỗ Thanh", "Hùng"), ("Bùi Anh", "Khoa"), ("Vũ Đức", "Long"),
            ("Đặng Văn", "Mạnh"), ("Ngô Quang", "Nam"), ("Dương Minh", "Phong"), ("Phan Văn", "Quân"),
            ("Mai Đức", "Sơn"), ("Tạ Quốc", "Thắng"), ("Hà Minh", "Tuấn"), ("Cao Văn", "Việt"),
            ("Nguyễn Thị", "Lan"), ("Trần Thu", "Hà"), ("Lê Thị", "Hương"), ("Phạm Minh", "Ngọc"),
            ("Hoàng Thị", "Trang"), ("Đỗ Thu", "Thảo"), ("Bùi Thị", "Yến"), ("Vũ Thị", "Mai")
        };

        return names[Math.Abs(customer.Id) % names.Length];
    }
}
