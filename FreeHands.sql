
CREATE proc [dbo].[RateProduct]
@Rate float,
@Product_ID int,
@Comment nvarchar(max),
@User_ID int,
@ReturnValue int out
as
begin
if(select count(*) from Product_Rates where User_ID=@User_ID and Product_ID=@Product_ID)>=1
select @ReturnValue=0
else
insert into Product_Rates values (@Rate,@Comment,@User_ID,@Product_ID,'true')
end

GO
/****** Object:  StoredProcedure [dbo].[RateProvider]    Script Date: 11/19/2019 10:01:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE proc [dbo].[RateProvider]
@Rate float,
@User_ID int,
@Comment nvarchar(max),
@Provider_ID int,
@ReturnValue int out
as
begin
if(select count(*) from Provider_Rates where User_ID=@User_ID and Provider_ID=@Provider_ID)>=1
select @ReturnValue=0
else
insert into Provider_Rates values (@Rate,@Comment,@User_ID,@Provider_ID,'true')
end
GO
/****** Object:  Table [dbo].[Admins]    Script Date: 11/19/2019 10:01:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Admins](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](50) NOT NULL,
	[IsManager] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Categories]    Script Date: 11/19/2019 10:01:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[City]    Script Date: 11/19/2019 10:01:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[City](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Country_ID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Companies]    Script Date: 11/19/2019 10:01:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Companies](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](max) NOT NULL,
	[Phone] [nvarchar](20) NOT NULL,
	[Address] [nvarchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Contacts]    Script Date: 11/19/2019 10:01:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Contacts](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[Subject] [nvarchar](50) NOT NULL,
	[Message] [nvarchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Country]    Script Date: 11/19/2019 10:01:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Country](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Data]    Script Date: 11/19/2019 10:01:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Data](
	[ID] [int] NOT NULL,
	[TaxForVisa] [float] NOT NULL,
	[Tax] [float] NOT NULL,
	[Delivery] [int] NOT NULL,
	[AllowKnownNumber] [bit] NOT NULL,
	[AllowVisa] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Drivers]    Script Date: 11/19/2019 10:01:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Drivers](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[Token] [nvarchar](max) NOT NULL,
	[Password] [nvarchar](100) NOT NULL,
	[Phone] [nvarchar](20) NOT NULL,
	[Location] [nvarchar](max) NOT NULL,
	[Company_ID] [int] NULL,
	[Visible] [bit] NOT NULL,
	[Country_ID] [int] NOT NULL,
	[City_ID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Offers]    Script Date: 11/19/2019 10:01:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Offers](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Price] [float] NOT NULL,
	[Product_ID] [int] NOT NULL,
	[Provider_ID] [int] NOT NULL,
	[Active] [bit] NOT NULL,
	[Description] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Options]    Script Date: 11/19/2019 10:01:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Options](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[OptionName] [nvarchar](max) NOT NULL,
	[Product_ID] [int] NOT NULL,
	[Price] [float] NOT NULL,
	[Description] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OrderOptions]    Script Date: 11/19/2019 10:01:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderOptions](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Order_ID] [int] NOT NULL,
	[Option_ID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Orders]    Script Date: 11/19/2019 10:01:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Date] [datetime] NOT NULL,
	[Number] [int] NOT NULL,
	[IsOrderd] [bit] NOT NULL,
	[IsUseVisa] [bit] NOT NULL,
	[Status] [nvarchar](50) NULL,
	[Driver_ID] [int] NULL,
	[Offer_ID] [int] NULL,
	[User_ID] [int] NOT NULL,
	[Product_ID] [int] NOT NULL,
	[Accepted] [bit] NOT NULL,
	[Ready] [bit] NOT NULL,
	[ISArrivedToUser] [bit] NOT NULL,
	[UserLat] [float] NOT NULL,
	[TaxForVisa] [float] NULL,
	[Total] [float] NULL,
	[Discount] [float] NULL,
	[Delivery] [int] NULL,
	[DateTimeNeeded] [datetime] NULL,
	[Reason] [nvarchar](max) NULL,
	[UserLog] [float] NOT NULL,
	[ProblemInArriveToUser] [bit] NOT NULL,
	[Price] [float] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Product_Rates]    Script Date: 11/19/2019 10:01:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product_Rates](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Rate] [float] NULL,
	[Comment] [nvarchar](max) NULL,
	[User_ID] [int] NOT NULL,
	[Product_ID] [int] NOT NULL,
	[Avaiable] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Products]    Script Date: 11/19/2019 10:01:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[Price] [float] NOT NULL,
	[Cat_ID] [int] NOT NULL,
	[Provider_ID] [int] NOT NULL,
	[TimeToFinish] [int] NOT NULL,
	[Visible] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Provider_Rates]    Script Date: 11/19/2019 10:01:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Provider_Rates](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Rate] [float] NULL,
	[Comment] [nvarchar](max) NULL,
	[User_ID] [int] NOT NULL,
	[Provider_ID] [int] NOT NULL,
	[Avaiable] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Providers]    Script Date: 11/19/2019 10:01:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Providers](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](100) NOT NULL,
	[LastName] [nvarchar](100) NOT NULL,
	[TradeName] [nvarchar](100) NOT NULL,
	[Token] [nvarchar](max) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[Password] [nvarchar](100) NOT NULL,
	[Phone] [nvarchar](20) NOT NULL,
	[Lat] [float] NOT NULL,
	[Log] [float] NOT NULL,
	[Location] [nvarchar](max) NOT NULL,
	[IsAvaiable] [bit] NOT NULL,
	[AllowVisa] [bit] NOT NULL,
	[IsExcelent] [bit] NOT NULL,
	[KnownNumber] [nvarchar](max) NULL,
	[Discount] [float] NULL,
	[Country_ID] [int] NOT NULL,
	[City_ID] [int] NOT NULL,
	[Currency] [nvarchar](3) NOT NULL,
	[Admin_Accept] [bit] NOT NULL,
	[AccountNumber] [nvarchar](max) NULL,
	[BankName] [nvarchar](max) NULL,
	[IPan] [nvarchar](max) NULL,
	[SwiftCode] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Subscribers]    Script Date: 11/19/2019 10:01:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Subscribers](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tasks]    Script Date: 11/19/2019 10:01:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tasks](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Task] [nvarchar](max) NOT NULL,
	[FromManager] [int] NOT NULL,
	[ToAdmin] [int] NOT NULL,
	[Finished] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Users]    Script Date: 11/19/2019 10:01:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Phone] [nvarchar](20) NOT NULL,
	[Token] [nvarchar](max) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[City_ID] [int] NOT NULL,
	[Country_ID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[Admins] ON 

GO
INSERT [dbo].[Admins] ([ID], [UserName], [Password], [IsManager]) VALUES (4, N'Ata Sabri', N'01142229025', 1)
GO
INSERT [dbo].[Admins] ([ID], [UserName], [Password], [IsManager]) VALUES (7, N'Ahmed', N'12345', 0)
GO
SET IDENTITY_INSERT [dbo].[Admins] OFF
GO
SET IDENTITY_INSERT [dbo].[Categories] ON 

GO
INSERT [dbo].[Categories] ([ID], [Name]) VALUES (1, N'Category 1')
GO
INSERT [dbo].[Categories] ([ID], [Name]) VALUES (2, N'Category 2')
GO
INSERT [dbo].[Categories] ([ID], [Name]) VALUES (3, N'Category 3')
GO
INSERT [dbo].[Categories] ([ID], [Name]) VALUES (4, N'Category 4')
GO
SET IDENTITY_INSERT [dbo].[Categories] OFF
GO
SET IDENTITY_INSERT [dbo].[City] ON 

GO
INSERT [dbo].[City] ([ID], [Name], [Country_ID]) VALUES (1, N'Cairo', 1)
GO
INSERT [dbo].[City] ([ID], [Name], [Country_ID]) VALUES (2, N'6 October', 1)
GO
INSERT [dbo].[City] ([ID], [Name], [Country_ID]) VALUES (3, N'الرياض', 3)
GO
INSERT [dbo].[City] ([ID], [Name], [Country_ID]) VALUES (4, N'الرياضff', 3)
GO
SET IDENTITY_INSERT [dbo].[City] OFF
GO
SET IDENTITY_INSERT [dbo].[Companies] ON 

GO
INSERT [dbo].[Companies] ([ID], [Name], [UserName], [Password], [Phone], [Address]) VALUES (1, N'Company 1', N'company', N'123', N'01142229025', N'Cairo')
GO
SET IDENTITY_INSERT [dbo].[Companies] OFF
GO
SET IDENTITY_INSERT [dbo].[Contacts] ON 

GO
INSERT [dbo].[Contacts] ([ID], [Name], [Email], [Subject], [Message]) VALUES (24, N'Ata Sabri', N'ata.sabry@rooyadev.com', N'test', N'jhjhjhjhjhjhjhjhjhjhjhjh')
GO
SET IDENTITY_INSERT [dbo].[Contacts] OFF
GO
SET IDENTITY_INSERT [dbo].[Country] ON 

GO
INSERT [dbo].[Country] ([ID], [Name]) VALUES (1, N'Egypt')
GO
INSERT [dbo].[Country] ([ID], [Name]) VALUES (2, N'USA')
GO
INSERT [dbo].[Country] ([ID], [Name]) VALUES (3, N'KSA')
GO
SET IDENTITY_INSERT [dbo].[Country] OFF
GO
INSERT [dbo].[Data] ([ID], [TaxForVisa], [Tax], [Delivery], [AllowKnownNumber], [AllowVisa]) VALUES (1, 2.5, 10, 5, 1, 0)
GO
SET IDENTITY_INSERT [dbo].[Drivers] ON 

GO
INSERT [dbo].[Drivers] ([ID], [FirstName], [LastName], [Email], [Token], [Password], [Phone], [Location], [Company_ID], [Visible], [Country_ID], [City_ID]) VALUES (1, N'Sabri', N'Sabri', N'ataeldon@gmail.com', N't7fCbntMWD/40QY7jsXmT4a3jYMNJ7Fmazx5divD14xJZS6iZZk8sst+A+I7kOsi', N'123456', N'01142229025', N'asdasasa', NULL, 1, 1, 1)
GO
SET IDENTITY_INSERT [dbo].[Drivers] OFF
GO
SET IDENTITY_INSERT [dbo].[Options] ON 

GO
INSERT [dbo].[Options] ([ID], [OptionName], [Product_ID], [Price], [Description]) VALUES (15, N'option2', 17, 4, NULL)
GO
INSERT [dbo].[Options] ([ID], [OptionName], [Product_ID], [Price], [Description]) VALUES (16, N'option3', 17, 4.4, NULL)
GO
SET IDENTITY_INSERT [dbo].[Options] OFF
GO
SET IDENTITY_INSERT [dbo].[OrderOptions] ON 

GO
INSERT [dbo].[OrderOptions] ([ID], [Order_ID], [Option_ID]) VALUES (11, 1, 15)
GO
SET IDENTITY_INSERT [dbo].[OrderOptions] OFF
GO
SET IDENTITY_INSERT [dbo].[Orders] ON 

GO
INSERT [dbo].[Orders] ([ID], [Date], [Number], [IsOrderd], [IsUseVisa], [Status], [Driver_ID], [Offer_ID], [User_ID], [Product_ID], [Accepted], [Ready], [ISArrivedToUser], [UserLat], [TaxForVisa], [Total], [Discount], [Delivery], [DateTimeNeeded], [Reason], [UserLog], [ProblemInArriveToUser], [Price]) VALUES (1, CAST(0x0000A95200FC5362 AS DateTime), 2, 1, 0, N'Under Work', NULL, NULL, 4, 17, 1, 0, 0, 29.912278, NULL, 211.6, NULL, 5, CAST(0x0000A8DE00D21D10 AS DateTime), NULL, 30.920062, 0, NULL)
GO
SET IDENTITY_INSERT [dbo].[Orders] OFF
GO
SET IDENTITY_INSERT [dbo].[Product_Rates] ON 

GO
INSERT [dbo].[Product_Rates] ([ID], [Rate], [Comment], [User_ID], [Product_ID], [Avaiable]) VALUES (6, 5, N'test', 4, 11, 1)
GO
SET IDENTITY_INSERT [dbo].[Product_Rates] OFF
GO
SET IDENTITY_INSERT [dbo].[Products] ON 

GO
INSERT [dbo].[Products] ([ID], [Name], [Description], [Price], [Cat_ID], [Provider_ID], [TimeToFinish], [Visible]) VALUES (10, N'Product 6', N'This Is For Test', 34, 3, 4, 30, 0)
GO
INSERT [dbo].[Products] ([ID], [Name], [Description], [Price], [Cat_ID], [Provider_ID], [TimeToFinish], [Visible]) VALUES (11, N'Product 7', N'This Is For Test', 87, 4, 4, 15, 1)
GO
INSERT [dbo].[Products] ([ID], [Name], [Description], [Price], [Cat_ID], [Provider_ID], [TimeToFinish], [Visible]) VALUES (17, N'نص فرخة مشوية', N'This For Test final', 99.3, 1, 5, 45, 1)
GO
SET IDENTITY_INSERT [dbo].[Products] OFF
GO
SET IDENTITY_INSERT [dbo].[Providers] ON 

GO
INSERT [dbo].[Providers] ([ID], [FirstName], [LastName], [TradeName], [Token], [Email], [Password], [Phone], [Lat], [Log], [Location], [IsAvaiable], [AllowVisa], [IsExcelent], [KnownNumber], [Discount], [Country_ID], [City_ID], [Currency], [Admin_Accept], [AccountNumber], [BankName], [IPan], [SwiftCode]) VALUES (4, N'Ata', N'Sabri', N'AtaSabri', N'zwJNaTBdrDVfnsbVvpgRzdNnRam8pK6NPUos+DaBQFM=', N'ataeldon@gmail.com', N'01142229025', N'201142229025', 29.8853967, 30.9080815, N'Cairo', 0, 1, 1, NULL, NULL, 1, 1, N'EGP', 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Providers] ([ID], [FirstName], [LastName], [TradeName], [Token], [Email], [Password], [Phone], [Lat], [Log], [Location], [IsAvaiable], [AllowVisa], [IsExcelent], [KnownNumber], [Discount], [Country_ID], [City_ID], [Currency], [Admin_Accept], [AccountNumber], [BankName], [IPan], [SwiftCode]) VALUES (5, N'Ata', N'Sabri', N'AtaSabr', N'FdPzbt1fosPIfPmEsiAXGw==', N'ataeldon@gmail.com', N'01142229025', N'01142229025', 20.87766, 31.98776, N'Cairo', 1, 0, 0, NULL, NULL, 3, 3, N'SAR', 0, NULL, NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[Providers] OFF
GO
SET IDENTITY_INSERT [dbo].[Subscribers] ON 

GO
INSERT [dbo].[Subscribers] ([ID], [Email]) VALUES (24, N'ataeldon@gmail.com')
GO
SET IDENTITY_INSERT [dbo].[Subscribers] OFF
GO
SET IDENTITY_INSERT [dbo].[Tasks] ON 

GO
INSERT [dbo].[Tasks] ([ID], [Task], [FromManager], [ToAdmin], [Finished]) VALUES (2, N'This Is Task 1', 4, 7, 1)
GO
INSERT [dbo].[Tasks] ([ID], [Task], [FromManager], [ToAdmin], [Finished]) VALUES (3, N'This Is Task 2', 4, 7, 0)
GO
SET IDENTITY_INSERT [dbo].[Tasks] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

GO
INSERT [dbo].[Users] ([ID], [Phone], [Token], [IsActive], [City_ID], [Country_ID]) VALUES (4, N'201142229025', N'CtQ4FeEM4bP705wSV1cDgpgVLQpP0322gWyHDlIpG5s=', 1, 1, 1)
GO
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ__Companie__C9F28456098117DD]    Script Date: 11/19/2019 10:01:34 AM ******/
ALTER TABLE [dbo].[Companies] ADD UNIQUE NONCLUSTERED 
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ__tmp_ms_x__A9D105344E660E19]    Script Date: 11/19/2019 10:01:34 AM ******/
ALTER TABLE [dbo].[Drivers] ADD UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ__tmp_ms_x__6675EB9DC04D600E]    Script Date: 11/19/2019 10:01:34 AM ******/
ALTER TABLE [dbo].[Providers] ADD UNIQUE NONCLUSTERED 
(
	[TradeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ__Users__5C7E359E3A020707]    Script Date: 11/19/2019 10:01:34 AM ******/
ALTER TABLE [dbo].[Users] ADD UNIQUE NONCLUSTERED 
(
	[Phone] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UQ__Users__5C7E359EFE30BE40]    Script Date: 11/19/2019 10:01:34 AM ******/
ALTER TABLE [dbo].[Users] ADD UNIQUE NONCLUSTERED 
(
	[Phone] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT ((1)) FOR [Number]
GO
ALTER TABLE [dbo].[City]  WITH CHECK ADD FOREIGN KEY([Country_ID])
REFERENCES [dbo].[Country] ([ID])
GO
ALTER TABLE [dbo].[Drivers]  WITH CHECK ADD FOREIGN KEY([City_ID])
REFERENCES [dbo].[City] ([ID])
GO
ALTER TABLE [dbo].[Drivers]  WITH CHECK ADD  CONSTRAINT [FK__Drivers__Company__65F62111] FOREIGN KEY([Company_ID])
REFERENCES [dbo].[Companies] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Drivers] CHECK CONSTRAINT [FK__Drivers__Company__65F62111]
GO
ALTER TABLE [dbo].[Drivers]  WITH CHECK ADD FOREIGN KEY([Country_ID])
REFERENCES [dbo].[Country] ([ID])
GO
ALTER TABLE [dbo].[Offers]  WITH CHECK ADD  CONSTRAINT [FK__Offers__Product___1CF15040] FOREIGN KEY([Product_ID])
REFERENCES [dbo].[Products] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Offers] CHECK CONSTRAINT [FK__Offers__Product___1CF15040]
GO
ALTER TABLE [dbo].[Offers]  WITH CHECK ADD  CONSTRAINT [FK__Offers__Provider__72C60C4A] FOREIGN KEY([Provider_ID])
REFERENCES [dbo].[Providers] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Offers] CHECK CONSTRAINT [FK__Offers__Provider__72C60C4A]
GO
ALTER TABLE [dbo].[Options]  WITH CHECK ADD  CONSTRAINT [FK__Options__Product__2A4B4B5E] FOREIGN KEY([Product_ID])
REFERENCES [dbo].[Products] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Options] CHECK CONSTRAINT [FK__Options__Product__2A4B4B5E]
GO
ALTER TABLE [dbo].[OrderOptions]  WITH CHECK ADD  CONSTRAINT [FK__OrderOpti__Option__03F0984C] FOREIGN KEY([Option_ID])
REFERENCES [dbo].[Options] ([ID])
GO
ALTER TABLE [dbo].[OrderOptions] CHECK CONSTRAINT [FK__OrderOpti__Option__03F0984C]
GO
ALTER TABLE [dbo].[OrderOptions]  WITH CHECK ADD  CONSTRAINT [FK__OrderOpti__Order__03F0984C] FOREIGN KEY([Order_ID])
REFERENCES [dbo].[Orders] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderOptions] CHECK CONSTRAINT [FK__OrderOpti__Order__03F0984C]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK__Orders__Driver_I__0C50D423] FOREIGN KEY([Driver_ID])
REFERENCES [dbo].[Drivers] ([ID])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK__Orders__Driver_I__0C50D423]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK__Orders__offer__0C50D423] FOREIGN KEY([Offer_ID])
REFERENCES [dbo].[Offers] ([ID])
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK__Orders__offer__0C50D423]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK__Orders__Product___35BCFE0A] FOREIGN KEY([Product_ID])
REFERENCES [dbo].[Products] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK__Orders__Product___35BCFE0A]
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK__Orders__User_ID__693CA210] FOREIGN KEY([User_ID])
REFERENCES [dbo].[Users] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK__Orders__User_ID__693CA210]
GO
ALTER TABLE [dbo].[Product_Rates]  WITH CHECK ADD  CONSTRAINT [FK__Product_R__Produ__276EDEB3] FOREIGN KEY([Product_ID])
REFERENCES [dbo].[Products] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Product_Rates] CHECK CONSTRAINT [FK__Product_R__Produ__276EDEB3]
GO
ALTER TABLE [dbo].[Product_Rates]  WITH CHECK ADD  CONSTRAINT [FK__Product_R__User___68487DD7] FOREIGN KEY([User_ID])
REFERENCES [dbo].[Users] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Product_Rates] CHECK CONSTRAINT [FK__Product_R__User___68487DD7]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK__Products__Cat_ID__1920BF5C] FOREIGN KEY([Cat_ID])
REFERENCES [dbo].[Categories] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK__Products__Cat_ID__1920BF5C]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK__Products__Provider__1920BF5C] FOREIGN KEY([Provider_ID])
REFERENCES [dbo].[Providers] ([ID])
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK__Products__Provider__1920BF5C]
GO
ALTER TABLE [dbo].[Provider_Rates]  WITH CHECK ADD  CONSTRAINT [FK__Provider___Provi__73BA3083] FOREIGN KEY([Provider_ID])
REFERENCES [dbo].[Providers] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Provider_Rates] CHECK CONSTRAINT [FK__Provider___Provi__73BA3083]
GO
ALTER TABLE [dbo].[Provider_Rates]  WITH CHECK ADD  CONSTRAINT [FK__Provider___User___6754599E] FOREIGN KEY([User_ID])
REFERENCES [dbo].[Users] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Provider_Rates] CHECK CONSTRAINT [FK__Provider___User___6754599E]
GO
ALTER TABLE [dbo].[Providers]  WITH CHECK ADD FOREIGN KEY([City_ID])
REFERENCES [dbo].[City] ([ID])
GO
ALTER TABLE [dbo].[Providers]  WITH CHECK ADD FOREIGN KEY([Country_ID])
REFERENCES [dbo].[Country] ([ID])
GO
ALTER TABLE [dbo].[Tasks]  WITH CHECK ADD  CONSTRAINT [FK__Tasks__FromManager__3E52440B] FOREIGN KEY([FromManager])
REFERENCES [dbo].[Admins] ([ID])
GO
ALTER TABLE [dbo].[Tasks] CHECK CONSTRAINT [FK__Tasks__FromManager__3E52440B]
GO
ALTER TABLE [dbo].[Tasks]  WITH CHECK ADD  CONSTRAINT [FK__Tasks__ToAdmin__3E52440B] FOREIGN KEY([ToAdmin])
REFERENCES [dbo].[Admins] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Tasks] CHECK CONSTRAINT [FK__Tasks__ToAdmin__3E52440B]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD FOREIGN KEY([City_ID])
REFERENCES [dbo].[City] ([ID])
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD FOREIGN KEY([Country_ID])
REFERENCES [dbo].[Country] ([ID])
GO
USE [master]
GO
ALTER DATABASE [FreeHands] SET  READ_WRITE 
GO
