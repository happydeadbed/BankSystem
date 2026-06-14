CREATE DATABASE BankDatabase;
GO

USE BankDatabase;
GO

CREATE TABLE Admins
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(500) NOT NULL
);

CREATE TABLE Customers
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CustomerNumber NVARCHAR(50) NOT NULL,
    FullName NVARCHAR(200) NOT NULL,
    PasswordHash NVARCHAR(500) NOT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE()
);

CREATE TABLE BankAccounts
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    AccountNumber NVARCHAR(50) NOT NULL,
    CustomerId INT NOT NULL,
    AccountType NVARCHAR(50) NOT NULL,
    Balance DECIMAL(18,2) NOT NULL DEFAULT 0,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_BankAccounts_Customers
        FOREIGN KEY(CustomerId) REFERENCES Customers(Id)
);

CREATE TABLE Transactions
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TransactionNumber NVARCHAR(50) NOT NULL,
    CustomerId INT NOT NULL,
    FromAccountId INT NULL,
    ToAccountId INT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    TransactionType NVARCHAR(50) NOT NULL,
    TransactionDate DATETIME NOT NULL DEFAULT GETDATE(),
    IsReversed BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_Transactions_Customers
        FOREIGN KEY(CustomerId) REFERENCES Customers(Id)
);

CREATE TABLE Loans
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    LoanNumber NVARCHAR(50) NOT NULL,
    CustomerId INT NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    InterestRate DECIMAL(5,2) NOT NULL,
    DurationMonths INT NOT NULL,
    StartDate DATETIME NOT NULL,
    Status NVARCHAR(50) NOT NULL,

    CONSTRAINT FK_Loans_Customers
        FOREIGN KEY(CustomerId) REFERENCES Customers(Id)
);

CREATE TABLE LoanPayments
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    LoanId INT NOT NULL,
    DueDate DATETIME NOT NULL,
    AmountDue DECIMAL(18,2) NOT NULL,
    AmountPaid DECIMAL(18,2) NOT NULL DEFAULT 0,
    IsPaid BIT NOT NULL DEFAULT 0,

    CONSTRAINT FK_LoanPayments_Loans
        FOREIGN KEY(LoanId) REFERENCES Loans(Id)
);

CREATE TABLE MarketplaceListings
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ListingNumber NVARCHAR(50) NOT NULL,
    LoanId INT NOT NULL,
    CustomerId INT NOT NULL,
    ListingDate DATETIME NOT NULL DEFAULT GETDATE(),
    Status NVARCHAR(50) NOT NULL,

    CONSTRAINT FK_MarketplaceListings_Loans
        FOREIGN KEY(LoanId) REFERENCES Loans(Id),

    CONSTRAINT FK_MarketplaceListings_Customers
        FOREIGN KEY(CustomerId) REFERENCES Customers(Id)
);
GO