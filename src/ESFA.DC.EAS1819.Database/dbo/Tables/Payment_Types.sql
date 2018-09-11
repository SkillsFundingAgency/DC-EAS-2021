CREATE TABLE [dbo].[Payment_Types] (
    [Payment_Id]  INT            NOT NULL,
    [PaymentName] NVARCHAR (250) NOT NULL,
    CONSTRAINT [PK_Payment_Types] PRIMARY KEY CLUSTERED ([Payment_Id] ASC)
);

