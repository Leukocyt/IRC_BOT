CREATE TABLE [dbo].[rand_messages_old] (
    [rowid]     BIGINT         IDENTITY (1, 1) NOT NULL,
    [rowid_irc] BIGINT         NULL,
    [kanava]    NVARCHAR (500) NULL,
    [message]   NVARCHAR (MAX) NULL,
    [initiator] NVARCHAR (500) NULL,
    CONSTRAINT [PK_rand_messages_old] PRIMARY KEY CLUSTERED ([rowid] ASC)
);

