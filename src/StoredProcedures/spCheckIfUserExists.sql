/****** Object:  StoredProcedure [dbo].[spCheckIfUserExists]    Script Date: 6/1/2023 2:06:16 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		JAMES KARANJA MAINA
-- Create date: MONDAY, 30TH MAY 2023 15:51HRS
-- Description:	This procedure checks if a user exists in the main catalogue.
-- =============================================

ALTER PROCEDURE [dbo].[spCheckIfUserExists]
	@UserName nvarchar(200)

AS
BEGIN
BEGIN TRY 
BEGIN TRANSACTION
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF exists(SELECT UserName FROM sadsignups WHERE UserName = @UserName)
		BEGIN
            SELECT 1
		END
	ELSE
		BEGIN
			SELECT 0
		END

COMMIT TRANSACTION
END TRY

BEGIN CATCH
	IF @@TRANCOUNT > 0
		ROLLBACK TRANSACTION
		--select ERROR_MESSAGE () --'Transaction Failed'
		declare @ErrorMessage nvarchar(500)
		declare @ErrorSeverity nvarchar(50)
		declare @ErrorState nvarchar(50)
		SET @ErrorMessage  = ERROR_MESSAGE()
		SET @ErrorSeverity = ERROR_SEVERITY()
		SET @ErrorState    = ERROR_STATE()
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH
END
