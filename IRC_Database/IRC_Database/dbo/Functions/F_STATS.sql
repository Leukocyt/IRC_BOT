
-- =============================================
-- Author:		NIPU
-- Create date: 28.11.2016
-- Description:	Funktio palauttaa nick, viesti, count(*) - yhdistelmät halutulta kanavalta.
-- =============================================
CREATE FUNCTION [dbo].[F_STATS]
(	
	 @channel nvarchar(500), @count_choise int = 0
)
RETURNS TABLE 
AS
RETURN 
(
	select top 20
		nick, 	
		sum(len(viesti))  as count
		/*
		case @count_choise
		when 0 then 
				count(*)
		when 1 then 
				sum(len(viesti))  
		else
				sum([dbo].[Get_Word_Count](viesti))

		end as count
		*/

	from 
		irkki
	where 
		kanava = @channel --and viesti <> 'x'
	group by nick
	order by count desc
)