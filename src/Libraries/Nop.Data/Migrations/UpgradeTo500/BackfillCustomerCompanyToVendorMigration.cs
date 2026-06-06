using FluentMigrator;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;

namespace Nop.Data.Migrations.UpgradeTo500;

[NopUpdateMigration("2026-05-24 11:00:00", "5.00", UpdateMigrationType.Data)]
public class BackfillCustomerCompanyToVendorMigration : Migration
{
    public override void Up()
    {
        if (!DataSettingsManager.IsDatabaseInstalled())
            return;

        var customerRepository = EngineContext.Current.Resolve<IRepository<Customer>>();
        var vendorRepository = EngineContext.Current.Resolve<IRepository<Vendor>>();

        var vendorsById = vendorRepository.Table
            .Where(vendor => !vendor.Deleted)
            .ToDictionary(vendor => vendor.Id);

        if (!vendorsById.Any())
            return;

        var customers = customerRepository.Table
            .Where(customer => !customer.Deleted)
            .ToList();

        foreach (var customer in customers)
        {
            var unitVendorId = customer.VendorId > 0 ? customer.VendorId : customer.CompanyId.GetValueOrDefault();
            if (unitVendorId <= 0 || !vendorsById.TryGetValue(unitVendorId, out var vendor))
                continue;

            var changed = false;

            if (customer.VendorId != unitVendorId)
            {
                customer.VendorId = unitVendorId;
                changed = true;
            }

            if (customer.CompanyId != unitVendorId)
            {
                customer.CompanyId = unitVendorId;
                changed = true;
            }

            if (!string.Equals(customer.Company, vendor.Name, StringComparison.Ordinal))
            {
                customer.Company = vendor.Name;
                changed = true;
            }

            if (changed)
                customerRepository.UpdateAsync(customer).GetAwaiter().GetResult();
        }
    }

    public override void Down()
    {
    }
}
