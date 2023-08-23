USE [CourseSeller_DB]
GO

INSERT INTO [dbo].[Users]
           ([UserId]
           ,[UserName]
           ,[Email]
           ,[Password]
           ,[ActiveCode]
           ,[ActiveCodeGenerateDateTime]
           ,[IsActive]
           ,[UserAvatar]
           ,[RegisterDateTime])
     VALUES
           (N'b0e7dc8c305d4f6a820d875aef7e1767'
           ,N'TestUserName'
           ,N'TestEmail@TestMail.com'
           ,N'$2a$11$8pesfV.tiyV2jwVNXqT55.OQGe03511V5n9fJPX2FjA/9TI8mjP4e'
           ,N'04111f9fc04544a2b989d3c6b7964f9f9bfe8924f75244e8ab1f90dbe9238282'
           ,GETDATE()
           ,1
           ,N'Default.png'
           ,GETDATE()
		   )

INSERT INTO WalletTypes (TypeId, TypeTitle)
VALUES (1, N'واریز'), (2, N'برداشت')

DECLARE @UserId NVARCHAR(32)
SET @UserId = N'b0e7dc8c305d4f6a820d875aef7e1767'
INSERT INTO Wallets (TypeId, UserId, Amount, Description, IsPaid, CreatedDateTime)
VALUES (1, @UserId, 25000, N'شارژ حساب', 1, GETDATE()),
		(1, @UserId, 6000, N'شارژ حساب', 1, GETDATE()),
		(2, @UserId, 8000, N'خرید آموزش', 1, GETDATE())

INSERT INTO [dbo].[Roles] ([RoleTitle])
VALUES
    (N'مدیر سایت'),
	(N'استاد'),
	(N'کاربر عادی')

INSERT INTO [dbo].[Permission]
           ([PermissionTitle]
           ,[ParentId])
VALUES
    (N'پنل مدیریت', null),
	(N'مدیریت کاربران', 1),
	(N'افزودن کاربر', 2),
	(N'ویرایش کاربر', 2),
	(N'حذف کاربر', 2)

GO

