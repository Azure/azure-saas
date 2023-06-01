/****** Object:  StoredProcedure [dbo].[spSaveVerifiedUsers]    Script Date: 6/1/2023 12:06:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		JAMES KARANJA MAINA
-- Create date: MONDAY, 29TH MAY 2023 11:02HRS
-- Description:	This procedure saves verified users information to the catalogue on onboarding iBusiness.
-- =============================================
ALTER PROCEDURE [dbo].[spSaveVerifiedUsers]
	@UserName nvarchar(200),
	@FullNames nvarchar(200),
	@EmpNo nvarchar(200) = '001',
	@Password nvarchar(500),
	@ConfirmPassword nvarchar(500),
	@Question nvarchar(200),
	@Answer nvarchar(50),
	@Email nvarchar(200),
	@Telephone nvarchar(50),
	@ExpiryDate date,
	@ExpiresAfter int = 3,
	@LockAfter int = 3,
	@ImmediateChange bit = 0,
	@IsActive bit = 0,
	@SuperUser bit = 1,
	@BioUserID nvarchar(100) = '0',
	@CCCode nvarchar(50) = '001',
	@RegSource nvarchar(200),
	@Narration nvarchar(500),
	@DOB date = '1900-01-01',
	@IDType nvarchar(50) = 'National ID',
	@Profession nvarchar(300) = 'Other', 
	@Company nvarchar(500) = 'New company',
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

	IF EXISTS(SELECT UserName FROM sadsignups WHERE UserName = @UserName)
		BEGIN
			PRINT 'User exists!'
		END
	ELSE
		BEGIN
			INSERT INTO [dbo].[sadsignups]
					   ([UserName]
					   ,[FullNames]
					   ,[EmpNo]
					   ,[Password]
					   ,[ConfirmPassword]
					   ,[Question]
					   ,[Answer]
					   ,[Email]
					   ,[Telephone]
					   ,[ExpiryDate]
					   ,[ExpiresAfter]
					   ,[LockAfter]
					   ,[ImmediateChange]
					   ,[IsActive]
					   ,[SuperUser]
					   ,[BioUserID]
					   ,[CCCode]
					   ,[RegSource]
					   ,[Narration]
					   ,[DOB]
					   ,[IDType]
					   ,[Profession]
					   ,[Company]
					   ,[Employees]
					   ,[Country]
					   ,[AcceptTerms]
					   ,[Notifications]
					   ,[DBIdentity]
					   ,[InitReady]
					   ,[ExternalDB]
					   ,[PrincipalUser]
					   ,[TimeZone]
					   ,[CreatedUser]
					   ,[CreatedDate]
					   ,[UpdatedUser]
					   ,[UpdatedDate]
					   ,[Terminus])
				 VALUES
					   (
						@UserName,
						@FullNames,
						@EmpNo,
						@Password,
						@ConfirmPassword,
						@Question,
						@Answer,
						@Email,
						@Telephone,
						@ExpiryDate,
						@ExpiresAfter,
						@LockAfter,
						@ImmediateChange,
						@IsActive,
						@SuperUser,
						@BioUserID,
						@CCCode,
						@RegSource,
						@Narration,
						@DOB,
						@IDType,
						@Profession, 
						@Company,
						@Employees,
						@Country,
						@AcceptTerms,
						@Notifications,
						@DBIdentity,
						@InitReady,
						@ExternalDB,
						@PrincipalUser,
						@TimeZone,
						@CreatedUser,
						@CreatedDate,
						@UpdatedUser,
						@UpdatedDate,
						@Terminus

						)

			SELECT UserID from sadsignups WHERE UserName = @UserName
		END

COMMIT TRANSACTION
END TRY
BEGIN CATCH
	IF @@TRANCOUNT > 0
		ROLLBACK TRANSACTION
			declare @ErrorMessage nvarchar(500)
			declare @ErrorSeverity nvarchar(50)
			declare @ErrorState nvarchar(50)
			SET @ErrorMessage  = ERROR_MESSAGE()
			SET @ErrorSeverity = ERROR_SEVERITY()
			SET @ErrorState    = ERROR_STATE()
			RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState)
END CATCH
END
