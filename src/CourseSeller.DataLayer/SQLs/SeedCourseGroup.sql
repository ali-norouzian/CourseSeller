USE [CourseSeller_DB]
GO

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
    (N'برنامه نویسی سخت افزار',0,null),

GO

