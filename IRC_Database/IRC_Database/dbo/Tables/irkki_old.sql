CREATE TABLE [dbo].[irkki_old] (
    [viesti] NVARCHAR (MAX)  NULL,
    [nick]   NVARCHAR (200)  NULL,
    [maara]  BIGINT          NULL,
    [kanava] NVARCHAR (1000) NULL,
    [id]     BIGINT          IDENTITY (1, 1) NOT NULL,
    [aika]   DATETIME        NULL,
    CONSTRAINT [PK_irkki_old] PRIMARY KEY CLUSTERED ([id] ASC)
);

