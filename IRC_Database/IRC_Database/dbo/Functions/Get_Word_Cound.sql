-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION Get_Word_Cound
(
	-- Add the parameters for the function here
	@String nvarchar(max)
)
RETURNS int
AS
BEGIN


	declare @wordCount int


	set @wordCount =  LEN(@String) - LEN(REPLACE(@String, ' ', '')) + 1

	RETURN 1

END