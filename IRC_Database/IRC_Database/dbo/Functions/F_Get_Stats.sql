-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION F_Get_Stats
(
	@start datetime, @kanava nvarchar(400), @count_choise int
)
RETURNS 
@table TABLE 
(
	-- Add the column definitions for the TABLE variable here
	nick nvarchar(500),
	count int
)
AS
BEGIN
	-- Fill the table variable with the rows for your result set
	if @count_choise = 0 begin

		insert @table
		select 
			nick, 	
			count(*)  as count
		from 
			irkki
		where 
			kanava = @kanava and aika >= @start  --and viesti <> 'x' 
		group by nick
	end
	else if @count_choise = 1 begin
		insert @table
		select 
			nick, 	
			sum(len(viesti))  as count
		from 
			irkki
		where 
			kanava = @kanava and aika >= @start --and viesti <> 'x'
		group by nick
	end
	else begin
		insert @table
		select 
			nick, 	
			sum([dbo].[Get_Word_Count](viesti))  as count
		from 
			irkki
		where 
			kanava = @kanava and aika >= @start --and viesti <> 'x'
		group by nick
	end

	RETURN 
END