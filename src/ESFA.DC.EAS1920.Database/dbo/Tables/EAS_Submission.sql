CREATE TABLE [dbo].[EAS_Submission] (
    [Submission_Id]      UNIQUEIDENTIFIER NOT NULL,
    [UKPRN]              NVARCHAR (10)    NOT NULL,
    [CollectionPeriod]   INT              NOT NULL,
    [ProviderName]       NVARCHAR (250)   NOT NULL,
    [UpdatedOn]          DATETIME         NOT NULL,
    [DeclarationChecked] BIT              NOT NULL,
    [NilReturn]          BIT              NOT NULL,
    [UpdatedBy]          NVARCHAR (250)   NULL,
    CONSTRAINT [PK_EAS_Submission] PRIMARY KEY CLUSTERED ([Submission_Id] ASC, [CollectionPeriod] ASC)
);

