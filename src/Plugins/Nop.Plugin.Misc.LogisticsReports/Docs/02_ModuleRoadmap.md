# MVP 1 module breakdown

## Module 1 - Organization
- Domain: `OrganizationUnit`, `CustomerOrganizationUnit`
- Service: `OrganizationUnitService`
- Controller: `OrganizationAdminController`
- Views: `Views/OrganizationAdmin/*`

## Module 2 - Report template
- Domain: `ReportTemplate`, `ReportIndicator`
- Service: `ReportTemplateService`
- Controller: `ReportTemplateAdminController`
- Views: `Views/ReportTemplateAdmin/*`

## Module 3 - Report period
- Domain: `ReportPeriod`
- Service: `ReportPeriodService`
- Controller: `ReportPeriodAdminController`
- Views: `Views/ReportPeriodAdmin/*`

## Module 4 - Unit report workflow
- Domain: `UnitReport`, `UnitReportValue`, `UnitReportImportHistory`, `UnitReportWorkflowLog`
- Service: `UnitReportService`, `AggregationService`, `ExcelImportService`
- Controller: `UnitReportAdminController`
- Views: `Views/UnitReportAdmin/*`

## Module 5 - Infrastructure
- Permissions
- Admin menu
- Startup registration
- Plugin install/uninstall
