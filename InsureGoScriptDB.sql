CREATE DATABASE InsureGoDB;
USE InsureGoDB;

--creating user table----
CREATE TABLE Users (
   UserId INT IDENTITY(1,1) PRIMARY KEY,
   FullName VARCHAR(100) NOT NULL,
   EmailId VARCHAR(100) NOT NULL UNIQUE,
   DateOfBirth DATE NOT NULL,
   ContactNumber VARCHAR(15) NOT NULL,
   Address VARCHAR(250) NOT NULL,
   PasswordHash VARCHAR(200) NOT NULL,
   CreatedDate DATETIME DEFAULT GETDATE()
);


--Insurance Types
--(2 wheeler, 4 wheeler)
CREATE TABLE InsuranceTypes (
   InsuranceTypeId INT IDENTITY PRIMARY KEY,
   InsuranceTypeName VARCHAR(50) NOT NULL
); 

--Sample data for insurance types
INSERT INTO InsuranceTypes VALUES ('Motor Insurance');
INSERT INTO InsuranceTypes VALUES ('Travel Insurance');


--Vehicles table
CREATE TABLE Vehicles (
   VehicleId INT IDENTITY(1,1) PRIMARY KEY,
   Manufacturer VARCHAR(50) NOT NULL,
   Model VARCHAR(50) NOT NULL,
   DrivingLicenseNumber VARCHAR(30) NOT NULL,
   PurchaseDate DATE NOT NULL,
   RegistrationNumber VARCHAR(20) NOT NULL UNIQUE,
   EngineNumber VARCHAR(30) NOT NULL UNIQUE,
   ChassisNumber VARCHAR(30) NOT NULL UNIQUE,
   CreatedDate DATETIME DEFAULT GETDATE()
);

--Vehicle types
CREATE TABLE VehicleTypes (
   VehicleTypeId INT IDENTITY PRIMARY KEY,
   VehicleTypeName VARCHAR(20) NOT NULL
);
--Sample data
INSERT INTO VehicleTypes VALUES ('2 Wheeler');
INSERT INTO VehicleTypes VALUES ('4 Wheeler');

--Insurance Plans
--(Third Party / Comprehensive)
CREATE TABLE InsurancePlans (
   PlanId INT IDENTITY PRIMARY KEY,
   PlanName VARCHAR(50),
   Description VARCHAR(200)
);
--Sample data for Insurance Plans 
INSERT INTO InsurancePlans VALUES
('Third Party Liability', 'Covers third party damage'),
('Comprehensive', 'Covers own and third party damage');

--Policy Duration
CREATE TABLE PolicyDurations (
   DurationId INT IDENTITY PRIMARY KEY,
   DurationYears INT
);
--Sample data for Policy Duration
INSERT INTO PolicyDurations VALUES (1);
INSERT INTO PolicyDurations VALUES (3);

--Policies Table
--(Buy & Renew Insurance)
CREATE TABLE Policies (
   PolicyId INT IDENTITY PRIMARY KEY,
   PolicyNumber VARCHAR(30) UNIQUE,
   UserId INT,
   InsuranceTypeId INT,
   VehicleId INT NULL,
   PlanId INT,
   DurationId INT,
   PremiumAmount DECIMAL(10,2),
   StartDate DATE,
   EndDate DATE,
   PolicyStatus VARCHAR(20), -- Active / Expired / Renewed
   FOREIGN KEY (UserId) REFERENCES Users(UserId),
   FOREIGN KEY (InsuranceTypeId) REFERENCES InsuranceTypes(InsuranceTypeId),
   FOREIGN KEY (VehicleId) REFERENCES Vehicles(VehicleId),
   FOREIGN KEY (PlanId) REFERENCES InsurancePlans(PlanId),
   FOREIGN KEY (DurationId) REFERENCES PolicyDurations(DurationId)
);


--Premium Calculation Table
--(Estimate Insurance)
CREATE TABLE PremiumRates (
   RateId INT IDENTITY PRIMARY KEY,
   VehicleTypeId INT,
   BasePremium DECIMAL(10,2),
   PerYearCharge DECIMAL(10,2),
   FOREIGN KEY (VehicleTypeId) REFERENCES VehicleTypes(VehicleTypeId)
);
--Sample data
INSERT INTO PremiumRates VALUES (1, 2000, 300); -- 2W
INSERT INTO PremiumRates VALUES (2, 5000, 500); -- 4W

--Payments table
CREATE TABLE Payments (
   PaymentId INT IDENTITY PRIMARY KEY,
   PolicyId INT,
   PaymentDate DATETIME DEFAULT GETDATE(),
   Amount DECIMAL(10,2),
   PaymentStatus VARCHAR(20),
   FOREIGN KEY (PolicyId) REFERENCES Policies(PolicyId)
);

--Claims table
CREATE TABLE Claims (
   ClaimId INT IDENTITY PRIMARY KEY,
   PolicyId INT,
   ClaimReason VARCHAR(300),
   ClaimDate DATETIME DEFAULT GETDATE(),
   ClaimStatus VARCHAR(30), -- Pending / Approved / Rejected
   ClaimAmount DECIMAL(10,2),
   FOREIGN KEY (PolicyId) REFERENCES Policies(PolicyId)
);

--Claim History View
Go
CREATE VIEW ClaimHistory AS
SELECT
   c.ClaimId,
   p.PolicyNumber,
   c.ClaimDate,
   c.ClaimStatus,
   c.ClaimAmount
FROM Claims c
JOIN Policies p ON c.PolicyId = p.PolicyId;


--Stored Procedure – Calculate Premium
go
CREATE PROCEDURE CalculatePremium 
   @VehicleTypeId INT,
   @VehicleAge INT
AS
BEGIN
   DECLARE @Base DECIMAL(10,2);
   DECLARE @PerYear DECIMAL(10,2);
   SELECT
       @Base = BasePremium,
       @PerYear = PerYearCharge
   FROM PremiumRates
   WHERE VehicleTypeId = @VehicleTypeId;
   SELECT (@Base + (@VehicleAge * @PerYear)) AS PremiumAmount;
END;

--Stored Procedure – Renew Policy
go
CREATE PROCEDURE RenewPolicy
   @OldPolicyNumber VARCHAR(30)
AS
BEGIN
   UPDATE Policies
   SET PolicyStatus = 'Renewed',
       StartDate = GETDATE(),
       EndDate = DATEADD(YEAR, 1, GETDATE())
   WHERE PolicyNumber = @OldPolicyNumber;
END;