
-- =============================================
-- Author:		NIPU
-- Create date: 28.11.2016
-- Description:	Funktio palauttaa nick, viesti, count(*) - yhdistelmät halutulta kanavalta.
-- =============================================
CREATE FUNCTION [dbo].[F_WORDS_BY_NICK_AND_CHANNEL]
(	
	 @channel nvarchar(500), @nick nvarchar(500) = null
)
RETURNS TABLE 
AS
RETURN 
(
	select top 100
		nick, viesti, count(*) as count
	from 
		irkki
	where 
		kanava = @channel and (@nick is null or nick = @nick) and viesti <> 'x'
	group by
		nick, viesti
	order by count desc
)