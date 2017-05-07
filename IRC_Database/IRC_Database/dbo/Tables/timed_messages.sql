CREATE TABLE [dbo].[timed_messages] (
    [rowid]     BIGINT         IDENTITY (1, 1) NOT NULL,
    [channel]   NVARCHAR (500) NULL,
    [message]   NVARCHAR (MAX) NULL,
    [timing_ID] BIGINT         NULL,
    CONSTRAINT [PK_timed_messages] PRIMARY KEY CLUSTERED ([rowid] ASC),
    CONSTRAINT [FK_timed_messages_timing_table] FOREIGN KEY ([timing_ID]) REFERENCES [dbo].[timing_table] ([rowid])
);

