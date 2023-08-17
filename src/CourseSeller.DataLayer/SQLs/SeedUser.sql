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
GO

