CREATE TABLE [dbo].[stock_guess_table] (
    [rowid]           INT            IDENTITY (1, 1) NOT NULL,
    [timestamp]       DATETIME       NULL,
    [stock_day]       DATETIME       NULL,
    [direction_guess] INT            NULL,
    [guess_correct]   BIT            NULL,
    [result_checked]  DATETIME       NULL,
    [target]          INT            NULL,
    [nick]            NVARCHAR (500) NULL,
    CONSTRAINT [PK_stock_guess_table] PRIMARY KEY CLUSTERED ([rowid] ASC),
    CONSTRAINT [FK_stock_guess_table_stocks] FOREIGN KEY ([target]) REFERENCES [dbo].[stocks] ([id])
);

