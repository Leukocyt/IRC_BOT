CREATE TABLE [dbo].[rand_messages] (
    [rowid]     BIGINT         IDENTITY (1, 1) NOT NULL,
    [rowid_irc] BIGINT         NULL,
    [kanava]    NVARCHAR (500) NULL,
    [message]   NVARCHAR (MAX) NULL,
    [initiator] NVARCHAR (500) NULL,
    CONSTRAINT [PK_rand_messages] PRIMARY KEY CLUSTERED ([rowid] ASC)
);


GO
-- =============================================
-- Author:		Nipsu
-- Create date: 1.6.2017
-- Description:	Keep size of the random message buffer reasonable.
-- =============================================
CREATE TRIGGER [dbo].[irc_randbuffer]
   ON  [dbo].[rand_messages]
   after insert
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @minRow bigint
	declare @channel nvarchar(500)
	declare @initiator nvarchar(500)

	set @channel = (select top 1 kanava from inserted)
	set @initiator = (select top 1 initiator from inserted)
	set @minRow = 1
	if ( select count(*) from rand_messages r where r.initiator = @initiator and r.kanava = @channel ) > 30 begin
			set @minRow = (select min(rowid) from rand_messages where initiator = @initiator and kanava = @channel)
			delete from rand_messages where rowid = @minRow
	end
    -- Insert statements for trigger here

END