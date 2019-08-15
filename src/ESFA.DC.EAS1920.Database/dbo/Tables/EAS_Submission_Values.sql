CREATE TABLE [dbo].[EAS_Submission_Values] (
    [Submission_Id]    UNIQUEIDENTIFIER NOT NULL,
    [CollectionPeriod] INT              NOT NULL,
    [Payment_Id]       INT              NOT NULL,
    [PaymentValue]     DECIMAL (10, 2)  NOT NULL,
    [DevolvedAreaSoF] [INT] Default(-1),
    CONSTRAINT [PK_EAS_Submission_Values] PRIMARY KEY CLUSTERED ([Submission_Id] ASC, [CollectionPeriod] ASC, [Payment_Id] ASC, [DevolvedAreaSoF] ASC),
	CONSTRAINT [FK_EAS_Submission_Values_Payment_Types] FOREIGN KEY([Payment_Id]) REFERENCES [dbo].[Payment_Types] ([Payment_Id]),
	CONSTRAINT [FK_EAS_Submission_Values_EAS_Submission] FOREIGN KEY([Submission_Id], [CollectionPeriod]) REFERENCES [dbo].[EAS_Submission] ([Submission_Id], [CollectionPeriod])
);