-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[Get_Word_Count]
(
	-- Add the parameters for the function here
	@String nvarchar(max)
)
RETURNS int
AS
BEGIN


	declare @wordCount int


	set @wordCount =  LEN(@String) - LEN(REPLACE(@String, ' ', '')) + 1

	RETURN @wordCount

END