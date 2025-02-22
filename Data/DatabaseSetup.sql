-- Om du vill köra detta skript flera gånger, kan du först ta bort databasen om den finns.
IF DB_ID(N'PopulationGame') IS NOT NULL
BEGIN
    ALTER DATABASE [PopulationGame] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [PopulationGame];
END
GO

-- Skapa databasen PopulationGame
CREATE DATABASE [PopulationGame];
GO

USE [PopulationGame];
GO

--------------------------------------------------
-- Tabell: Users
--------------------------------------------------
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE
);
GO

--------------------------------------------------
-- Tabell: Countries
--------------------------------------------------
CREATE TABLE Countries (
    CountryId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Population BIGINT NOT NULL
);
GO

--------------------------------------------------
-- Tabell: UserGameLog
--------------------------------------------------
CREATE TABLE UserGameLog (
    LogId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    StartTime DATETIME NOT NULL,
    EndTime DATETIME NOT NULL,
    Streak INT NOT NULL,
    Guesses INT NOT NULL,
    Difficulty NVARCHAR(50) NOT NULL,
    CONSTRAINT FK_UserGameLog_Users FOREIGN KEY (UserId)
        REFERENCES Users(UserId)
);
GO


