USE [master]
GO
/****** Object:  Database [ToDo]    Script Date: 01.07.2015 16:13:05 ******/
CREATE DATABASE [ToDo] ON  PRIMARY 
( NAME = N'ToDo', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.PATRICK\MSSQL\DATA\ToDo.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'ToDo_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.PATRICK\MSSQL\DATA\ToDo_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [ToDo].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [ToDo] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [ToDo] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [ToDo] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [ToDo] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [ToDo] SET ARITHABORT OFF 
GO
ALTER DATABASE [ToDo] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [ToDo] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [ToDo] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [ToDo] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [ToDo] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [ToDo] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [ToDo] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [ToDo] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [ToDo] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [ToDo] SET  DISABLE_BROKER 
GO
ALTER DATABASE [ToDo] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [ToDo] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [ToDo] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [ToDo] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [ToDo] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [ToDo] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [ToDo] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [ToDo] SET RECOVERY FULL 
GO
ALTER DATABASE [ToDo] SET  MULTI_USER 
GO
ALTER DATABASE [ToDo] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [ToDo] SET DB_CHAINING OFF 
GO
EXEC sys.sp_db_vardecimal_storage_format N'ToDo', N'ON'
GO
USE [ToDo]
GO
/****** Object:  User [admin]    Script Date: 01.07.2015 16:13:05 ******/
CREATE USER [admin] FOR LOGIN [admin] WITH DEFAULT_SCHEMA=[dbo]
GO
sys.sp_addrolemember @rolename = N'db_owner', @membername = N'admin'
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 01.07.2015 16:13:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Roles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tasks]    Script Date: 01.07.2015 16:13:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tasks](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Done] [bit] NULL,
	[Created] [datetime] NULL,
	[Description] [nvarchar](max) NULL,
	[Importance] [int] NULL,
	[Name] [nvarchar](255) NULL,
	[UserId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Tasks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserProfiles]    Script Date: 01.07.2015 16:13:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserProfiles](
	[Id] [uniqueidentifier] NOT NULL,
	[FirstName] [nvarchar](max) NULL,
	[LastName] [nvarchar](max) NULL,
	[Phone] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.UserProfiles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Users]    Script Date: 01.07.2015 16:13:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [uniqueidentifier] NOT NULL,
	[UserName] [nvarchar](max) NOT NULL,
	[Email] [nvarchar](max) NOT NULL,
	[Password] [nvarchar](max) NOT NULL,
	[Comment] [nvarchar](max) NULL,
	[IsApproved] [bit] NOT NULL,
	[IsConfirmed] [bit] NOT NULL,
	[PasswordFailuresSinceLastSuccess] [int] NOT NULL,
	[LastPasswordFailureDate] [datetime] NULL,
	[LastActivityDate] [datetime] NULL,
	[LastLockoutDate] [datetime] NULL,
	[LastLoginDate] [datetime] NULL,
	[ConfirmationToken] [nvarchar](max) NULL,
	[CreateDate] [datetime] NULL,
	[IsLockedOut] [bit] NOT NULL,
	[LastPasswordChangedDate] [datetime] NULL,
	[PasswordVerificationToken] [nvarchar](max) NULL,
	[PasswordVerificationTokenExpirationDate] [datetime] NULL,
 CONSTRAINT [PK_dbo.Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UsersInRoles]    Script Date: 01.07.2015 16:13:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UsersInRoles](
	[User_Id] [uniqueidentifier] NOT NULL,
	[Role_Id] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_dbo.UsersInRoles] PRIMARY KEY CLUSTERED 
(
	[User_Id] ASC,
	[Role_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
INSERT [dbo].[Roles] ([Id], [Name], [Description]) VALUES (N'2b155035-97db-4b07-9911-5e83c6ccc20a', N'User', N'Пользователь')
INSERT [dbo].[Roles] ([Id], [Name], [Description]) VALUES (N'2f155035-97db-4b07-9911-5e83c6ccc20a', N'Administrator', N'Администратор')
SET IDENTITY_INSERT [dbo].[Tasks] ON 

INSERT [dbo].[Tasks] ([Id], [Done], [Created], [Description], [Importance], [Name], [UserId]) VALUES (3, 0, CAST(N'2015-07-01 13:02:47.000' AS DateTime), N'4
4', 2, N'4', N'ea7c282b-a065-4895-94f8-d916d1840f7d')
INSERT [dbo].[Tasks] ([Id], [Done], [Created], [Description], [Importance], [Name], [UserId]) VALUES (5, 1, CAST(N'2015-07-01 13:12:35.000' AS DateTime), N'5
5
5', 1, N'5', N'eb7c282b-a065-4895-94f8-d916d1840f7d')
INSERT [dbo].[Tasks] ([Id], [Done], [Created], [Description], [Importance], [Name], [UserId]) VALUES (6, NULL, CAST(N'2015-07-01 13:12:50.013' AS DateTime), N'6
6
6', 2, N'6', N'eb7c282b-a065-4895-94f8-d916d1840f7d')
INSERT [dbo].[Tasks] ([Id], [Done], [Created], [Description], [Importance], [Name], [UserId]) VALUES (9, 1, CAST(N'2015-07-01 16:10:00.000' AS DateTime), N'6

6
6', 1, N'6', N'ea7c282b-a065-4895-94f8-d916d1840f7d')
SET IDENTITY_INSERT [dbo].[Tasks] OFF
INSERT [dbo].[UserProfiles] ([Id], [FirstName], [LastName], [Phone]) VALUES (N'ea7c282b-a065-4895-94f8-d916d1840f7d', NULL, NULL, NULL)
INSERT [dbo].[UserProfiles] ([Id], [FirstName], [LastName], [Phone]) VALUES (N'eb7c282b-a065-4895-94f8-d916d1840f7d', NULL, NULL, NULL)
INSERT [dbo].[Users] ([Id], [UserName], [Email], [Password], [Comment], [IsApproved], [IsConfirmed], [PasswordFailuresSinceLastSuccess], [LastPasswordFailureDate], [LastActivityDate], [LastLockoutDate], [LastLoginDate], [ConfirmationToken], [CreateDate], [IsLockedOut], [LastPasswordChangedDate], [PasswordVerificationToken], [PasswordVerificationTokenExpirationDate]) VALUES (N'ea7c282b-a065-4895-94f8-d916d1840f7d', N'admin', N'admin@todo.ru', N'AP5+xdbPlQGcDn8Ja+pVC1VwD8cuVNvWzk52tIf79cBr+L/c7ckoaoSd9ZRVg7YvXQ==', NULL, 1, 1, 0, CAST(N'2015-07-01 09:35:11.373' AS DateTime), CAST(N'2015-07-01 13:09:16.893' AS DateTime), CAST(N'2013-09-19 23:15:05.790' AS DateTime), CAST(N'2015-07-01 13:09:16.893' AS DateTime), NULL, CAST(N'2013-09-19 23:15:05.787' AS DateTime), 0, CAST(N'2015-07-01 10:07:58.467' AS DateTime), NULL, NULL)
INSERT [dbo].[Users] ([Id], [UserName], [Email], [Password], [Comment], [IsApproved], [IsConfirmed], [PasswordFailuresSinceLastSuccess], [LastPasswordFailureDate], [LastActivityDate], [LastLockoutDate], [LastLoginDate], [ConfirmationToken], [CreateDate], [IsLockedOut], [LastPasswordChangedDate], [PasswordVerificationToken], [PasswordVerificationTokenExpirationDate]) VALUES (N'eb7c282b-a065-4895-94f8-d916d1840f7d', N'user', N'user@todo.ru', N'AITlqoU6m7/UVEnxi10HiqVinDtuuO/w1IVk8X7a/H2S5aSFdoAUGl+k+q+FeGaJ9Q==', NULL, 1, 1, 0, CAST(N'2015-07-01 09:35:11.373' AS DateTime), CAST(N'2015-07-01 12:43:03.570' AS DateTime), CAST(N'2013-09-19 23:15:05.790' AS DateTime), CAST(N'2015-07-01 12:43:03.570' AS DateTime), NULL, CAST(N'2013-09-19 23:15:05.787' AS DateTime), 0, CAST(N'2015-07-01 10:07:58.467' AS DateTime), NULL, NULL)
INSERT [dbo].[UsersInRoles] ([User_Id], [Role_Id]) VALUES (N'eb7c282b-a065-4895-94f8-d916d1840f7d', N'2b155035-97db-4b07-9911-5e83c6ccc20a')
INSERT [dbo].[UsersInRoles] ([User_Id], [Role_Id]) VALUES (N'ea7c282b-a065-4895-94f8-d916d1840f7d', N'2f155035-97db-4b07-9911-5e83c6ccc20a')
/****** Object:  Index [IX_Id]    Script Date: 01.07.2015 16:13:05 ******/
CREATE NONCLUSTERED INDEX [IX_Id] ON [dbo].[UserProfiles]
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Role_Id]    Script Date: 01.07.2015 16:13:05 ******/
CREATE NONCLUSTERED INDEX [IX_Role_Id] ON [dbo].[UsersInRoles]
(
	[Role_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Tasks]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Tasks_dbo.Tasks_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[Tasks] CHECK CONSTRAINT [FK_dbo.Tasks_dbo.Tasks_UserId]
GO
ALTER TABLE [dbo].[UserProfiles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserProfiles_dbo.ProfileOf] FOREIGN KEY([Id])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[UserProfiles] CHECK CONSTRAINT [FK_dbo.UserProfiles_dbo.ProfileOf]
GO
ALTER TABLE [dbo].[UsersInRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UsersInRoles_dbo.Roles_Role_Id] FOREIGN KEY([Role_Id])
REFERENCES [dbo].[Roles] ([Id])
GO
ALTER TABLE [dbo].[UsersInRoles] CHECK CONSTRAINT [FK_dbo.UsersInRoles_dbo.Roles_Role_Id]
GO
ALTER TABLE [dbo].[UsersInRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UsersInRoles_dbo.Users_User_Id] FOREIGN KEY([User_Id])
REFERENCES [dbo].[Users] ([Id])
GO
ALTER TABLE [dbo].[UsersInRoles] CHECK CONSTRAINT [FK_dbo.UsersInRoles_dbo.Users_User_Id]
GO
USE [master]
GO
ALTER DATABASE [ToDo] SET  READ_WRITE 
GO
