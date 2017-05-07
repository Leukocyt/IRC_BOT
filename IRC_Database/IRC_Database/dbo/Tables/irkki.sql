CREATE TABLE [dbo].[irkki] (
    [viesti] NVARCHAR (MAX) NULL,
    [nick]   NVARCHAR (40)  NULL,
    [maara]  BIGINT         NULL,
    [kanava] NVARCHAR (40)  NULL,
    [id]     BIGINT         IDENTITY (1, 1) NOT NULL,
    [aika]   DATETIME       NULL,
    [old]    INT            NULL,
    CONSTRAINT [PK_irkki] PRIMARY KEY CLUSTERED ([id] ASC)
);




GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create TRIGGER irkki_insertChecker
   ON  dbo.irkki
   after insert
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	if  (select top 1 kanava from inserted) = '!5R6Y2gangsterimetsa' begin
		update irkki set viesti = 'x' where irkki.id in (select id from inserted)
	end
    -- Insert statements for trigger here

END
GO
CREATE NONCLUSTERED INDEX [Time_stamp]
    ON [dbo].[irkki]([aika] ASC, [kanava] ASC, [nick] ASC);

