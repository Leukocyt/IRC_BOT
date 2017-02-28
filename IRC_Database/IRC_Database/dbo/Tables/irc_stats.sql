CREATE TABLE [dbo].[irc_stats] (
    [rowid]  BIGINT         NOT NULL,
    [date]   DATETIME       NULL,
    [kanava] NVARCHAR (500) NULL,
    [nick]   NVARCHAR (500) NULL,
    [rows]   INT            NULL,
    [chars]  INT            NULL,
    [words]  INT            NULL,
    CONSTRAINT [PK_irc_stats] PRIMARY KEY CLUSTERED ([rowid] ASC)
);

