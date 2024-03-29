/*  Title: Research database build script
	Author: Joshua Preece
	Description: Install script for the remote SQL database to be 
	used with the repository research application or DMA.
	
	NOTE: The FILENAME path MUST exist before running and does not need to be in the default folder
	NOTE 2: A SQL login for 'tester' MUST be created before running this script, although the login
	name does not need to be 'tester' all references to the login 'tester' in this script need to be
	changed to a login script of your choice.

USE [master]
GO


-- Create Database
CREATE DATABASE [Research]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'Research', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\Research.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'Research_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.SQLEXPRESS\MSSQL\DATA\Research_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [Research] SET COMPATIBILITY_LEVEL = 120
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [Research].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [Research] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [Research] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [Research] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [Research] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [Research] SET ARITHABORT OFF 
GO
ALTER DATABASE [Research] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [Research] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [Research] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [Research] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [Research] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [Research] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [Research] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [Research] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [Research] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [Research] SET  DISABLE_BROKER 
GO
ALTER DATABASE [Research] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [Research] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [Research] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [Research] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [Research] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [Research] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [Research] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [Research] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [Research] SET  MULTI_USER 
GO
ALTER DATABASE [Research] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [Research] SET DB_CHAINING OFF 
GO
ALTER DATABASE [Research] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [Research] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [Research] SET DELAYED_DURABILITY = DISABLED 
GO

-- Use the new Research DB
USE [Research]
GO

-- Create new user 'tester'
CREATE USER [tester] FOR LOGIN [tester] WITH DEFAULT_SCHEMA=[dbo]
GO

-- Add owner role to 'tester' user
ALTER ROLE [db_owner] ADD MEMBER [tester]
GO

-- Create new employee table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Employee](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](50) NOT NULL,
	[StoreId] [int] NOT NULL,
 CONSTRAINT [PK_Employee] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

-- Create new product table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Product](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Prod_Name] [varchar](50) NOT NULL,
	[Price] [float] NOT NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO

-- Create new sale table 
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sale](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[StoreId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[SaleTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Sale] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

-- Create new store table
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Store](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[StoreName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Store] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

-- Setup relations and constraints
ALTER TABLE [dbo].[Employee]  WITH CHECK ADD  CONSTRAINT [FK_Employee_Store] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Store] ([id])
GO
ALTER TABLE [dbo].[Employee] CHECK CONSTRAINT [FK_Employee_Store]
GO
ALTER TABLE [dbo].[Sale]  WITH CHECK ADD  CONSTRAINT [FK_Sale_Product] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Product] ([id])
GO
ALTER TABLE [dbo].[Sale] CHECK CONSTRAINT [FK_Sale_Product]
GO
ALTER TABLE [dbo].[Sale]  WITH CHECK ADD  CONSTRAINT [FK_Sale_Store] FOREIGN KEY([StoreId])
REFERENCES [dbo].[Store] ([id])
GO
ALTER TABLE [dbo].[Sale] CHECK CONSTRAINT [FK_Sale_Store]
GO
ALTER TABLE [dbo].[Store]  WITH CHECK ADD  CONSTRAINT [FK_Store_Store] FOREIGN KEY([id])
REFERENCES [dbo].[Store] ([id])
GO
ALTER TABLE [dbo].[Store] CHECK CONSTRAINT [FK_Store_Store]
GO
USE [master]
GO
ALTER DATABASE [Research] SET  READ_WRITE 
GO
