CREATE TABLE [dbo].[EAS_Submission_Values] (
    [Submission_Id]    UNIQUEIDENTIFIER NOT NULL,
    [CollectionPeriod] INT              NOT NULL,
    [Payment_Id]       INT              NOT NULL,
    [PaymentValue]     DECIMAL (10, 2)  NOT NULL,
    CONSTRAINT [PK_EAS_Submission_Values] PRIMARY KEY CLUSTERED ([Submission_Id] ASC, [CollectionPeriod] ASC, [Payment_Id] ASC)
);

