-- OPEN IZ DATABASE FUNCTIONS
-- COPYRIGHT (c) 2016, OPEN IZ
-- PORTIONS COPYRIGHT (C) 2015, FYFE SOFTWARE INC.
-- LICENSED UNDER THE APACHE 2.0 LICENSE

CREATE FUNCTION fn_IsAccountLocked (@UserName NVARCHAR(128)) 
RETURNS BIT BEGIN
	RETURN (SELECT TOP 1 CASE WHEN Lockout > CURRENT_TIMESTAMP THEN 1 ELSE 0 END AS Lockout FROM SecurityUser WHERE UserName = @UserName)
END;

GO


CREATE PROCEDURE sp_Authenticate (@UserName NVARCHAR(128), @PasswordHash NVARCHAR(128), @MaxInvalidLoginAttempts INT, @SecurityUserId UNIQUEIDENTIFIER OUTPUT)
AS
DECLARE
	@UserPasswordHash NVARCHAR(128),
	@FailedLoginAttempts INT,
	@TwoFactorEnabled BIT;
BEGIN
	-- FIRST CHECK THE USERNAME EXISTS
	IF dbo.fn_IsAccountLocked(@UserName) = 1 BEGIN
		UPDATE SecurityUser SET FailedLoginAttempts = FailedLoginAttempts + 1, Lockout = dateadd(SECOND, 30 * (FailedLoginAttempts - @MaxInvalidLoginAttempts), CURRENT_TIMESTAMP) WHERE UserName = @UserName;
		THROW 51900, 'Account is locked', 1;
	END
	ELSE BEGIN
		SELECT @UserPasswordHash = UserPassword, @FailedLoginAttempts = FailedLoginAttempts, 
			@TwoFactorEnabled = TwoFactorEnabled, @SecurityUserId = UserId FROM SecurityUser WHERE 
			UserName = @UserName;

		-- ACCOUNT LOCKOUT ?
		IF @FailedLoginAttempts >= @MaxInvalidLoginAttempts BEGIN
			UPDATE SecurityUser SET FailedLoginAttempts = FailedLoginAttempts + 1, Lockout = dateadd(SECOND, 30 * (FailedLoginAttempts - @MaxInvalidLoginAttempts), CURRENT_TIMESTAMP) WHERE UserName = @UserName;
		END

		IF @TwoFactorEnabled = 1 BEGIN
			UPDATE SecurityUser SET FailedLoginAttempts = FailedLoginAttempts + 1 WHERE UserName = @UserName;
			THROW 51902, 'User must use two factor authentication', 1;
		END 		
		ELSE IF @UserPasswordHash = @PasswordHash BEGIN
			UPDATE SecurityUser SET 
				FailedLoginAttempts = 0, 
				LastSuccessfulLogin = CURRENT_TIMESTAMP, 
				UpdatedBy = 'fadca076-3690-4a6e-af9e-f1cd68e8c7e8',
				UpdatedTime = CURRENT_TIMESTAMP
			WHERE UserName = @UserName;

			RETURN
		END
		ELSE BEGIN
			UPDATE SecurityUser SET FailedLoginAttempts = FailedLoginAttempts + 1 WHERE UserName = @UserName;
			THROW 51901, 'Invalid Username/Password', 1;
		END		

	END;
	
END;
GO

CREATE FUNCTION fn_OpenIzSchemaVersion() RETURNS VARCHAR(10) AS
BEGIN
	RETURN '1.0.0.0';
END;