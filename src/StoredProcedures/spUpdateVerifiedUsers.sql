/****** Object:  StoredProcedure [dbo].[spUpdateVerifiedUsers]    Script Date: 6/1/2023 2:03:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		JAMES KARANJA MAINA
-- Create date: MONDAY, 29TH MAY 2023 14:10HRS
-- Description:	This procedure saves verified users information to the catalogue on onboarding iBusiness.
-- =============================================

ALTER PROCEDURE [dbo].[spUpdateVerifiedUsers]
	@UserName nvarchar(200),
	@FullNames nvarchar(200),
	@EmpNo nvarchar(200),
	@Password nvarchar(500),
	@ConfirmPassword nvarchar(500),
	@Question nvarchar(200),
	@Answer nvarchar(50),
	@Email nvarchar(200),
	@Telephone nvarchar(50),
	@ExpiryDate date,
	@ExpiresAfter int,
	@LockAfter int,
	@ImmediateChange bit,
	@IsActive bit,
	@SuperUser bit,
	@BioUserID nvarchar(100),
	@CCCode nvarchar(50),
	@RegSource nvarchar(200),
	@Narration nvarchar(500),
	@DOB date = '1900-01-01',
	@IDType nvarchar(50) = 'National ID',
	@Profession nvarchar(300) = 'Other', 
	@Company nvarchar(500) = 'New Company',
	@Employees int = 10,
	@Country nvarchar(100) = 'KE',
	@AcceptTerms bit = 1,
	@Notifications bit = 1,
	@DBIdentity nvarchar(500) = 'X',
	@InitReady bit = 0,
	@ExternalDB bit = 0,
	@PrincipalUser bit = 1,
	@TimeZone nvarchar(300) = 'E. Africa Standard Time',
	@CreatedUser nvarchar(200),
	@CreatedDate date,
	@UpdatedUser nvarchar(200),
	@UpdatedDate date,
	@Terminus nvarchar(200)

AS
BEGIN
BEGIN TRY 
BEGIN TRANSACTION
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF exists(SELECT UserName FROM sadsignups WHERE UserName = @UserName)
		BEGIN
			INSERT INTO sadsignupsHistory SELECT * FROM sadsignups WHERE UserName = @UserName

			UPDATE sadsignups 
			SET
				UserName = @UserName,
				FullNames = @FullNames,
				EmpNo = @EmpNo,
				Password = @Password,
				ConfirmPassword = @ConfirmPassword,
				Question = @Question,
				Answer = @Answer,
				Email = @Email,
				Telephone = @Telephone,
				ExpiryDate = @ExpiryDate,
				ExpiresAfter = @ExpiresAfter,
				LockAfter = @LockAfter,
				ImmediateChange = @ImmediateChange,
				IsActive = @IsActive,
				SuperUser = @SuperUser,
				BioUserID = @BioUserID,
				CCCode = @CCCode,
				RegSource = @RegSource,
				Narration = @Narration,
				DOB = @DOB,
				IDType = @IDType,
				Profession = @Profession,
				Company = @Company,
				Employees = @Employees,
				Country = @Country,
				AcceptTerms = @AcceptTerms,
				Notifications = @Notifications,
				DBIdentity = @DBIdentity,
				InitReady = @InitReady,
				ExternalDB = @ExternalDB,
				PrincipalUser = @PrincipalUser,
				TimeZone = @TimeZone,
				CreatedUser = @CreatedUser,
				CreatedDate = @CreatedDate,
				UpdatedUser = @UpdatedUser,
				UpdatedDate = GETDATE(),
				Terminus = @Terminus

			WHERE UserName = @UserName 				
		END
	ELSE
		BEGIN
			PRINT 'User does not exist!'
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
