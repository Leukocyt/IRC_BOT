CREATE TABLE [dbo].[irkkiTest] (
    [viesti] NVARCHAR (MAX)  NULL,
    [nick]   NVARCHAR (200)  NULL,
    [maara]  BIGINT          NULL,
    [kanava] NVARCHAR (1000) NULL,
    [id]     BIGINT          IDENTITY (1, 1) NOT NULL,
    [aika]   DATETIME        NULL,
    CONSTRAINT [PK_irkkiTest] PRIMARY KEY CLUSTERED ([id] ASC)
);


GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER irkkiTest_insert
   ON  irkkiTest
   after insert
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	if  (select top 1 kanava from inserted) = '!5R6Y2gangsterimetsa' begin
		update irkkiTest set viesti = 'x' where irkkiTest.id in (select id from inserted)
	end
    -- Insert statements for trigger here

END