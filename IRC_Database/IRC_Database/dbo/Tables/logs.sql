CREATE TABLE [dbo].[logs] (
    [rowid]     BIGINT         IDENTITY (1, 1) NOT NULL,
    [message]   NVARCHAR (500) NULL,
    [severity]  INT            NULL,
    [timestamp] DATETIME       NULL,
    CONSTRAINT [PK_logs] PRIMARY KEY CLUSTERED ([rowid] ASC)
);

