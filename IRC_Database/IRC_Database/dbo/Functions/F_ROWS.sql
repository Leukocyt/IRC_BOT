-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[F_ROWS]
(	
	@start datetime, @end datetime, @channel nvarchar(500)
)
RETURNS TABLE 
AS
RETURN 
(
	select 
		nick, count(*) as Count
	from 
		irkki
	where 
		kanava = @channel and aika between @start and @end and old is null
	group by
		nick
)