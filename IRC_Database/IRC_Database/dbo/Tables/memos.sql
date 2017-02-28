CREATE TABLE [dbo].[memos] (
    [rowid] INT            IDENTITY (1, 1) NOT NULL,
    [ts]    DATETIME       NULL,
    [url]   NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_memos] PRIMARY KEY CLUSTERED ([rowid] ASC)
);

