IF DB_ID(N'MyStore') IS NULL CREATE DATABASE [MyStore];
GO
USE [MyStore];
GO
CREATE TABLE [dbo].[User] (
  [Username] NVARCHAR(255) NOT NULL PRIMARY KEY,
  [Password] NCHAR(50) NOT NULL,
  [UserRole] NCHAR(1) NOT NULL
);
GO
CREATE TABLE [dbo].[Customer] (
  [CustomerID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  [CustomerName] NVARCHAR(MAX) NOT NULL,
  [CustomerPhone] NVARCHAR(15) NOT NULL,
  [CustomerEmail] NVARCHAR(MAX) NULL,
  [CustomerAddress] NVARCHAR(MAX) NULL,
  [Username] NVARCHAR(255) NOT NULL,
  CONSTRAINT [FK_User_Customer] FOREIGN KEY([Username]) REFERENCES [dbo].[User]([Username])
);
GO
CREATE TABLE [dbo].[Category] (
  [CategoryID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  [CategoryName] NVARCHAR(MAX) NOT NULL
);
GO
CREATE TABLE [dbo].[Product] (
  [ProductID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  [CategoryID] INT NOT NULL,
  [ProductName] NVARCHAR(MAX) NOT NULL,
  [ProductDescription] NVARCHAR(MAX) NOT NULL,
  [ProductPrice] DECIMAL(18,2) NOT NULL,
  [ProductImage] NVARCHAR(MAX) NULL,
  CONSTRAINT [FK_Pro_Category] FOREIGN KEY([CategoryID]) REFERENCES [dbo].[Category]([CategoryID]),
  CONSTRAINT [CK_Product_Price] CHECK ([ProductPrice] > 0)
);
GO
CREATE TABLE [dbo].[Order] (
  [OrderID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  [CustomerID] INT NOT NULL,
  [OrderDate] DATE NOT NULL,
  [TotalAmount] DECIMAL(18,2) NOT NULL,
  [PaymentStatus] NVARCHAR(MAX) NULL,
  [AddressDelivery] NVARCHAR(MAX) NOT NULL,
  CONSTRAINT [FK_Order_Customer] FOREIGN KEY([CustomerID]) REFERENCES [dbo].[Customer]([CustomerID])
);
GO
CREATE TABLE [dbo].[OrderDetail] (
  [ID] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
  [ProductID] INT NOT NULL,
  [OrderID] INT NOT NULL,
  [Quantity] INT NOT NULL,
  [UnitPrice] DECIMAL(18,2) NOT NULL,
  CONSTRAINT [FK_OrderDetail_Product] FOREIGN KEY([ProductID]) REFERENCES [dbo].[Product]([ProductID]),
  CONSTRAINT [FK_OrderDetail_Order] FOREIGN KEY([OrderID]) REFERENCES [dbo].[Order]([OrderID])
);
GO
-- Demo data; add further items through the Admin area.
INSERT INTO [dbo].[Category]([CategoryName]) VALUES (N'Điện thoại'), (N'Phụ kiện'), (N'Thiết bị khác');
INSERT INTO [dbo].[Product]([CategoryID],[ProductName],[ProductDescription],[ProductPrice],[ProductImage])
VALUES (1,N'Điện thoại mẫu',N'Sản phẩm minh họa - thay bằng thông tin thực tế.',3500000,NULL),
       (2,N'Tai nghe mẫu',N'Phụ kiện mẫu dùng để kiểm tra chức năng giỏ hàng.',250000,NULL),
       (3,N'Bàn phím mẫu',N'Thiết bị mẫu dùng để kiểm tra tìm kiếm và phân trang.',520000,NULL);
GO
