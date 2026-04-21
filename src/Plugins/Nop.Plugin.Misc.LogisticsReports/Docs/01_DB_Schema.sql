CREATE TABLE [dbo].[LR_OrganizationUnit](
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Code] NVARCHAR(128) NOT NULL,
    [Name] NVARCHAR(512) NOT NULL,
    [ParentId] INT NULL,
    [Level] INT NOT NULL,
    [Path] NVARCHAR(2000) NOT NULL,
    [DisplayOrder] INT NOT NULL DEFAULT(0),
    [IsActive] BIT NOT NULL DEFAULT(1),
    [CreatedOnUtc] DATETIME2 NOT NULL,
    [UpdatedOnUtc] DATETIME2 NOT NULL
);
GO
CREATE TABLE [dbo].[LR_CustomerOrganizationUnit](
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [CustomerId] INT NOT NULL,
    [OrganizationUnitId] INT NOT NULL,
    [IsPrimary] BIT NOT NULL DEFAULT(0),
    [CanInputReport] BIT NOT NULL DEFAULT(0),
    [CanReviewChildReports] BIT NOT NULL DEFAULT(0),
    [CanSubmitReport] BIT NOT NULL DEFAULT(0),
    [CanLockReport] BIT NOT NULL DEFAULT(0)
);
GO
CREATE TABLE [dbo].[LR_ReportTemplate](
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Code] NVARCHAR(128) NOT NULL,
    [Name] NVARCHAR(512) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [PeriodTypeId] INT NOT NULL,
    [TemplateStoredFileName] NVARCHAR(1000) NULL,
    [Version] NVARCHAR(64) NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT(1),
    [EffectiveFromUtc] DATETIME2 NULL,
    [EffectiveToUtc] DATETIME2 NULL
);
GO
CREATE TABLE [dbo].[LR_ReportIndicator](
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ReportTemplateId] INT NOT NULL,
    [Code] NVARCHAR(128) NOT NULL,
    [Name] NVARCHAR(512) NOT NULL,
    [ParentIndicatorId] INT NULL,
    [DisplayOrder] INT NOT NULL DEFAULT(0),
    [UnitOfMeasure] NVARCHAR(128) NULL,
    [DataTypeId] INT NOT NULL,
    [InputModeId] INT NOT NULL,
    [AggregateMethodId] INT NOT NULL,
    [FormulaExpression] NVARCHAR(1000) NULL,
    [ExcelSheetName] NVARCHAR(128) NULL,
    [ExcelCellAddress] NVARCHAR(32) NULL,
    [IsRequired] BIT NOT NULL DEFAULT(0),
    [IsActive] BIT NOT NULL DEFAULT(1)
);
GO
CREATE TABLE [dbo].[LR_ReportPeriod](
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ReportTemplateId] INT NOT NULL,
    [Code] NVARCHAR(128) NOT NULL,
    [Name] NVARCHAR(512) NOT NULL,
    [PeriodTypeId] INT NOT NULL,
    [FromDateUtc] DATETIME2 NOT NULL,
    [ToDateUtc] DATETIME2 NOT NULL,
    [DeadlineUtc] DATETIME2 NOT NULL,
    [StatusId] INT NOT NULL,
    [IsLocked] BIT NOT NULL DEFAULT(0),
    [CreatedOnUtc] DATETIME2 NOT NULL
);
GO
CREATE TABLE [dbo].[LR_UnitReport](
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [ReportPeriodId] INT NOT NULL,
    [OrganizationUnitId] INT NOT NULL,
    [ParentOrganizationUnitId] INT NULL,
    [StatusId] INT NOT NULL,
    [CurrentVersion] INT NOT NULL DEFAULT(1),
    [SubmittedOnUtc] DATETIME2 NULL,
    [SubmittedByCustomerId] INT NULL,
    [ReturnedReason] NVARCHAR(2000) NULL,
    [LockedOnUtc] DATETIME2 NULL,
    [LockedByCustomerId] INT NULL,
    [LastAggregatedOnUtc] DATETIME2 NULL
);
GO
CREATE TABLE [dbo].[LR_UnitReportValue](
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UnitReportId] INT NOT NULL,
    [ReportIndicatorId] INT NOT NULL,
    [SelfValueNumber] DECIMAL(18,4) NULL,
    [SelfValueText] NVARCHAR(2000) NULL,
    [ChildAggregateValueNumber] DECIMAL(18,4) NULL,
    [AdjustmentValueNumber] DECIMAL(18,4) NULL,
    [FinalValueNumber] DECIMAL(18,4) NULL,
    [FinalValueText] NVARCHAR(2000) NULL,
    [Note] NVARCHAR(2000) NULL,
    [SourceTypeId] INT NOT NULL DEFAULT(0),
    [LastUpdatedByCustomerId] INT NULL,
    [LastUpdatedOnUtc] DATETIME2 NULL
);
GO
CREATE TABLE [dbo].[LR_UnitReportImportHistory](
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UnitReportId] INT NOT NULL,
    [FileName] NVARCHAR(512) NOT NULL,
    [StoredFilePath] NVARCHAR(1000) NOT NULL,
    [ImportedByCustomerId] INT NOT NULL,
    [ImportedOnUtc] DATETIME2 NOT NULL,
    [SuccessCount] INT NOT NULL DEFAULT(0),
    [ErrorCount] INT NOT NULL DEFAULT(0),
    [ErrorLogPath] NVARCHAR(1000) NULL
);
GO
CREATE TABLE [dbo].[LR_UnitReportWorkflowLog](
    [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [UnitReportId] INT NOT NULL,
    [Action] NVARCHAR(128) NOT NULL,
    [FromStatusId] INT NOT NULL,
    [ToStatusId] INT NOT NULL,
    [ActionByCustomerId] INT NOT NULL,
    [ActionOnUtc] DATETIME2 NOT NULL,
    [Comment] NVARCHAR(2000) NULL
);
GO
CREATE INDEX [IX_LR_OrganizationUnit_ParentId] ON [dbo].[LR_OrganizationUnit]([ParentId]);
CREATE INDEX [IX_LR_OrganizationUnit_Path] ON [dbo].[LR_OrganizationUnit]([Path]);
CREATE INDEX [IX_LR_CustomerOrganizationUnit_CustomerId] ON [dbo].[LR_CustomerOrganizationUnit]([CustomerId]);
CREATE INDEX [IX_LR_CustomerOrganizationUnit_OrganizationUnitId] ON [dbo].[LR_CustomerOrganizationUnit]([OrganizationUnitId]);
CREATE INDEX [IX_LR_ReportIndicator_ReportTemplateId] ON [dbo].[LR_ReportIndicator]([ReportTemplateId]);
CREATE INDEX [IX_LR_ReportPeriod_ReportTemplateId] ON [dbo].[LR_ReportPeriod]([ReportTemplateId]);
CREATE INDEX [IX_LR_UnitReport_ReportPeriodId] ON [dbo].[LR_UnitReport]([ReportPeriodId]);
CREATE INDEX [IX_LR_UnitReport_OrganizationUnitId] ON [dbo].[LR_UnitReport]([OrganizationUnitId]);
CREATE INDEX [IX_LR_UnitReportValue_UnitReportId] ON [dbo].[LR_UnitReportValue]([UnitReportId]);
CREATE INDEX [IX_LR_UnitReportValue_ReportIndicatorId] ON [dbo].[LR_UnitReportValue]([ReportIndicatorId]);
GO
