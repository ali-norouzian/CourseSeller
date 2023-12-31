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
	(N'حذف کاربر', 2),
	(N'مدیریت نقش ها', 1),
	(N'افزودن نقش جدید', 6),
	(N'ویرایش نقش', 6),
	(N'حذف نقش', 6),

    (N'مدیریت تخفیف ها', 1),
	(N'مدیریت دوره ها', 1),
	(N'مدیریت گروه ها', 1)

INSERT INTO [dbo].[CourseGroups]
           ([GroupTitle]
           ,[IsDelete]
           ,[ParentId])
VALUES
    (N'برنامه نویسی موبایل',0,null),
    (N'Xamarin',0,1),
    (N'React Native',0,1),
    (N'برنامه نویسی وب',0,null),
    (N'ASP.Net Core',0,4),
    (N'Laravel',0,4),
    (N'Django',0,4),
    (N'برنامه نویسی سخت افزار',0,null)

INSERT INTO [dbo].[CourseLevels]
           ([LevelTitle])
VALUES
    (N'مقدماتی'),
    (N'متوسط'),
    (N'پیشرفته'),
    (N'خیلی پیشرفته')

INSERT INTO [dbo].[CourseStatus]
           ([StatusTitle])
VALUES
    (N'درحال برگزاری'),
    (N'کامل شده')

GO
