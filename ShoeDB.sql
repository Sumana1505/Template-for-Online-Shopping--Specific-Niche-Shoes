--Create database
CREATE DATABASE ShoeaDB
GO

USE ShoeaDB
GO
--Create the tables

CREATE TABLE tblUser (
  ID int IDENTITY (1, 1) NOT NULL,
  FirstName varchar(100) NOT NULL,
  MiddleName varchar(100),
  LastName varchar(100) NOT NULL,
  Email varchar(255) NOT NULL,
  Passwd varchar(100) NOT NULL,
  WalletAmount decimal(10, 2) NOT NULL
)

CREATE TABLE tblUserAddress (
  ID int IDENTITY (1, 1) NOT NULL,
  UserId int NOT NULL,
  AddresstypeId int NOT NULL,
  AddressLine1 varchar(255) NOT NULL,
  AddressLine2 varchar(255),
  Country varchar(255) NOT NULL,
  [State] varchar(255) NOT NULL,
  City varchar(255) NOT NULL,
  Pin varchar(30) NOT NULL
)

CREATE TABLE tblWalletHistory (
  ID int IDENTITY (1, 1) NOT NULL,
  UserId int NOT NULL,
  WalletAmount decimal(10, 2) NOT NULL,
  DateStamp datetime
)

CREATE TABLE dbo.tblAddressType (
  ID int IDENTITY (1, 1) NOT NULL,
  AddressType varchar(100) NOT NULL
)



ALTER TABLE tblUser ADD CONSTRAINT pk_UserId PRIMARY KEY (ID)
--Alter Table tblAddressID Add Constraint fk_AddressId Foreign Key(ID)
GO
--Data
INSERT INTO dbo.tblAddressType (AddressType)
  VALUES ('Home')
INSERT INTO dbo.tblAddressType (AddressType)
  VALUES ('Office')
INSERT INTO dbo.tblAddressType (AddressType)
  VALUES ('Apartment')
GO

--Procedures

CREATE PROCEDURE usp_changePassword @Email varchar(255), @OldPassword varchar(255), @NewPassword varchar(255), @result varchar(100) OUTPUT
AS
BEGIN
  IF EXISTS (SELECT
      1
    FROM dbo.tblUser
    WHERE Email = @Email
    AND Passwd = @OldPassword)
  BEGIN
    UPDATE dbo.tblUser
    SET Passwd = @NewPassword
    WHERE Email = @Email
    SET @result = 'Password updated successfully'
  END
  ELSE
  BEGIN
    SET @result = 'Failed to update password Email or OldPassword Mismatch'
  END
END
GO

CREATE PROCEDURE usp_UpdateWallet @Email varchar(255), @amount int, @result varchar(100) OUTPUT
AS
BEGIN
  DECLARE @userId int
  IF EXISTS (SELECT
      1
    FROM dbo.tblUser
    WHERE Email = @Email)
  BEGIN
    SELECT
      @userId = MIN(ID)
    FROM tblUser
    WHERE Email = @Email
    UPDATE dbo.tblUser
    SET WalletAmount = ISNULL(WalletAmount, 0) + CAST(@amount AS decimal)
    WHERE Email = @Email
    INSERT INTO dbo.tblWalletHistory (UserId, WalletAmount, DateStamp)
      VALUES (@userId, @amount, GETDATE())
    SET @result = 'Updated Wallet successfully'
  END
  ELSE
  BEGIN
    SET @result = 'Failed to update Wallet Amount, Email does not exist'
  END
END
GO

CREATE PROCEDURE dbo.usp_GetCustomerAddress @Email varchar(255)
AS
BEGIN
  SELECT
    A.AddressType,
    B.AddressLine1 + ' ' + ISNULL(B.AddressLine2, '') + ' ' + b.Country + ' ' + b.State + ' ' + b.City + ' ' + b.Pin AS Address
  FROM dbo.tblAddresstype A
  JOIN dbo.tblUserAddress b
    ON a.ID = b.AddresstypeId
  JOIN dbo.tblUser c
    ON b.UserId = c.id
  WHERE c.Email = @Email
END
GO