CREATE TABLE [dbo].[timing_table] (
    [rowid]        BIGINT   IDENTITY (1, 1) NOT NULL,
    [valid_from]   DATETIME NULL,
    [valid_to]     DATETIME NULL,
    [trigger_time] TIME (7) NULL,
    CONSTRAINT [PK_timing_table] PRIMARY KEY CLUSTERED ([rowid] ASC)
);

