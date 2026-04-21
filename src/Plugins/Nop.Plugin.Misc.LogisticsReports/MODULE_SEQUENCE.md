# Logistics Reports MVP 1 - Build Order

## 1. Database and domain
- `Data/SchemaMigration.cs`
- `sql_01_schema.sql`
- `Domain/*`
- `Enums/*`

## 2. Organization module
- `OrganizationUnitService`
- `OrganizationAdminController`
- `Views/OrganizationAdmin/*`

## 3. Report template module
- `ReportTemplateService`
- `ReportTemplateAdminController`
- `Views/ReportTemplateAdmin/*`

## 4. Report period module
- `ReportPeriodService`
- `ReportPeriodAdminController`
- `Views/ReportPeriodAdmin/*`

## 5. Unit report module
- `UnitReportService`
- `AggregationService`
- `ExcelImportService`
- `UnitReportAdminController`
- `Views/UnitReportAdmin/*`

## 6. Infrastructure
- Permissions
- Admin menu
- Plugin install/uninstall

## 7. Next suggested module after MVP 1
- Customer <-> Organization mapping CRUD UI
- Progress dashboard
- Excel export
- Validation rules by template version
