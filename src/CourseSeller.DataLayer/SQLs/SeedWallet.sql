USE [CourseSeller_DB]
GO

DECLARE @UserId NVARCHAR(32)
SET @UserId = N'b0e7dc8c305d4f6a820d875aef7e1767'

INSERT INTO Wallets (TypeId, UserId, Amount, Description, IsPaid, CreatedDateTime)
VALUES (1, @UserId, 25000, N'شارژ حساب', 1, GETDATE()),
		(1, @UserId, 6000, N'شارژ حساب', 1, GETDATE()),
		(2, @UserId, 8000, N'خرید آموزش', 1, GETDATE());

GO