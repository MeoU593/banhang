using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Vendors;
using Nop.Data;
using Nop.Services.Seo;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models.Directory;

namespace Nop.Web.Controllers;

public partial class DirectoryController : BasePublicController
{
    protected readonly IRepository<UnitDirectoryEntry> _directoryRepository;
    protected readonly IRepository<Vendor> _vendorRepository;
    protected readonly IUrlRecordService _urlRecordService;

    public DirectoryController(
        IRepository<UnitDirectoryEntry> directoryRepository,
        IRepository<Vendor> vendorRepository,
        IUrlRecordService urlRecordService)
    {
        _directoryRepository = directoryRepository;
        _vendorRepository = vendorRepository;
        _urlRecordService = urlRecordService;
    }

    public virtual async Task<IActionResult> Index(int vendorId = 0, int personId = 0, string q = null, string rank = null, string positionTitle = null)
    {
        var vendors = await _vendorRepository.Table
            .Where(vendor => !vendor.Deleted && vendor.Active)
            .OrderBy(vendor => vendor.Path ?? vendor.Name)
            .ThenBy(vendor => vendor.DisplayOrder)
            .ThenBy(vendor => vendor.Name)
            .ToListAsync();

        if (vendorId <= 0)
            vendorId = vendors.FirstOrDefault()?.Id ?? 0;

        var vendorIds = vendorId > 0 ? new[] { vendorId } : Array.Empty<int>();
        q = q?.Trim();
        rank = rank?.Trim();
        positionTitle = positionTitle?.Trim();

        var peopleQuery = _directoryRepository.Table
            .Where(entry => entry.Published && (vendorIds.Length == 0 || vendorIds.Contains(entry.VendorId)));

        if (!string.IsNullOrWhiteSpace(q))
            peopleQuery = peopleQuery.Where(entry => entry.FullName.Contains(q) || entry.Phone.Contains(q) || entry.MilitaryCode.Contains(q) || entry.UnitName.Contains(q));

        if (!string.IsNullOrWhiteSpace(rank))
            peopleQuery = peopleQuery.Where(entry => entry.Rank.Contains(rank));

        if (!string.IsNullOrWhiteSpace(positionTitle))
            peopleQuery = peopleQuery.Where(entry => entry.PositionTitle.Contains(positionTitle));

        var people = await peopleQuery
            .OrderBy(entry => entry.DisplayOrder)
            .ThenBy(entry => entry.FullName)
            .ToListAsync();

        var selected = people.FirstOrDefault(entry => entry.Id == personId) ?? people.FirstOrDefault();
        var vendorNames = vendors.ToDictionary(vendor => vendor.Id, vendor => vendor.Name);

        var model = new UnitDirectoryModel
        {
            SelectedVendorId = vendorId,
            SelectedPersonId = selected?.Id ?? 0,
            SearchKeyword = q ?? string.Empty,
            SearchRank = rank ?? string.Empty,
            SearchPositionTitle = positionTitle ?? string.Empty,
            Vendors = vendors.Select(vendor => new UnitDirectoryVendorNodeModel
            {
                Id = vendor.Id,
                Name = vendor.Name,
                Level = vendor.Level,
                Active = vendor.Id == vendorId
            }).ToList(),
            People = people.Select(entry => ToPersonModel(entry, vendorNames)).ToList(),
            SelectedPerson = selected == null ? null : ToPersonModel(selected, vendorNames)
        };

        return View(model);
    }

    protected virtual UnitDirectoryPersonModel ToPersonModel(UnitDirectoryEntry entry, IDictionary<int, string> vendorNames)
    {
        return new UnitDirectoryPersonModel
        {
            Id = entry.Id,
            VendorId = entry.VendorId,
            FullName = entry.FullName,
            Rank = entry.Rank,
            PositionTitle = entry.PositionTitle,
            Phone = entry.Phone,
            MilitaryCode = entry.MilitaryCode,
            UnitName = string.IsNullOrWhiteSpace(entry.UnitName) && vendorNames.TryGetValue(entry.VendorId, out var vendorName) ? vendorName : entry.UnitName,
            Biography = entry.Biography
        };
    }
}
