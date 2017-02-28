CREATE TABLE [dbo].[stocks] (
    [id]         INT            NOT NULL,
    [stock_name] NVARCHAR (500) NULL,
    CONSTRAINT [PK_stocks] PRIMARY KEY CLUSTERED ([id] ASC)
);

