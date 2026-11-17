CREATE DATABASE MyStoreCustomer;
GO
USE MyStoreCustomer;
GO
CREATE TABLE [User](Username NVARCHAR(100) PRIMARY KEY, PasswordHash NVARCHAR(255) NOT NULL, Role NVARCHAR(20) NOT NULL DEFAULT 'Customer');
CREATE TABLE Customer(CustomerID INT IDENTITY PRIMARY KEY, Username NVARCHAR(100) NOT NULL UNIQUE, CustomerName NVARCHAR(150) NOT NULL, CustomerPhone NVARCHAR(20), CustomerEmail NVARCHAR(150), CustomerAddress NVARCHAR(300), CONSTRAINT FK_Customer_User FOREIGN KEY(Username) REFERENCES [User](Username));
CREATE TABLE Product(ProductID INT IDENTITY PRIMARY KEY, ProductName NVARCHAR(200) NOT NULL, ProductPrice DECIMAL(18,2) NOT NULL, ProductImage NVARCHAR(500), ProductDescription NVARCHAR(MAX));
CREATE TABLE [Order](OrderID INT IDENTITY PRIMARY KEY, CustomerID INT NOT NULL, OrderDate DATETIME NOT NULL DEFAULT GETDATE(), TotalAmount DECIMAL(18,2) NOT NULL, Status NVARCHAR(30) NOT NULL, AddressDelivery NVARCHAR(300), CONSTRAINT FK_Order_Customer FOREIGN KEY(CustomerID) REFERENCES Customer(CustomerID), CONSTRAINT CK_Order_Status CHECK(Status IN ('PendingPayment','Processing','Shipping','Delivered','Cancelled')));
CREATE TABLE OrderDetail(OrderDetailID INT IDENTITY PRIMARY KEY, OrderID INT NOT NULL, ProductID INT NOT NULL, Quantity INT NOT NULL CHECK(Quantity>0), UnitPrice DECIMAL(18,2) NOT NULL, CONSTRAINT FK_OrderDetail_Order FOREIGN KEY(OrderID) REFERENCES [Order](OrderID), CONSTRAINT FK_OrderDetail_Product FOREIGN KEY(ProductID) REFERENCES Product(ProductID));
CREATE INDEX IX_Order_Customer_Date ON [Order](CustomerID,OrderDate DESC);
INSERT INTO [User](Username,PasswordHash,Role) VALUES('customer01','123456','Customer');
INSERT INTO Customer(Username,CustomerName,CustomerPhone,CustomerEmail,CustomerAddress) VALUES('customer01',N'Khách hàng mẫu','0900000000','customer@example.com',N'TP. Hồ Chí Minh');
INSERT INTO Product(ProductName,ProductPrice,ProductImage,ProductDescription) VALUES(N'Tai nghe không dây',750000,'earbuds.svg',N'Tai nghe Bluetooth'),(N'Tai nghe gaming',1250000,'headset.svg',N'Tai nghe chụp tai'),(N'Đồng hồ thông minh',650000,'watch.svg',N'Đồng hồ thông minh'),(N'Loa Bluetooth',890000,'speaker.svg',N'Loa Bluetooth di động');
INSERT INTO [Order](CustomerID,OrderDate,TotalAmount,Status,AddressDelivery) VALUES(1,DATEADD(day,-2,GETDATE()),750000,'Delivered',N'TP. Hồ Chí Minh'),(1,DATEADD(day,-1,GETDATE()),1250000,'Shipping',N'TP. Hồ Chí Minh'),(1,GETDATE(),650000,'Processing',N'TP. Hồ Chí Minh');
INSERT INTO OrderDetail(OrderID,ProductID,Quantity,UnitPrice) VALUES(1,1,1,750000),(2,2,1,1250000),(3,3,1,650000);