-- 1. Membuat Database
CREATE DATABASE DBGilingPadi;
GO

USE DBGilingPadi;
GO

-- 2. Membuat Tabel Penggilingan
CREATE TABLE Penggilingan (
    ID_Giling INT PRIMARY KEY IDENTITY(1,1),
    NamaTempat VARCHAR(100) NOT NULL,
    Alamat VARCHAR(255) NOT NULL,
    Wilayah VARCHAR(100) NOT NULL, -- Desa atau Kecamatan
    StatusOperasional VARCHAR(10)
    CHECK (StatusOperasional IN ('Buka','Tutup'))
    DEFAULT 'Buka'
);

-- 3. Data Awal (Opsional)
INSERT INTO Penggilingan (NamaTempat, Alamat, Wilayah, StatusOperasional)
VALUES ('Sari Tani', 'Jl. Sawah No. 5', 'Kasihan', 'Buka'),
       ('Maju Jaya', 'Depan Balai Desa', 'Bantul', 'Tutup');


-- 4. Login
USE DBGilingPadi;
GO

CREATE TABLE Users (
    ID_User INT PRIMARY KEY IDENTITY(1,1),
    Username VARCHAR(50) UNIQUE NOT NULL,
    Password VARCHAR(50) NOT NULL,
    RoleUser VARCHAR(20) NOT NULL
);

INSERT INTO Users (Username, Password, RoleUser)
VALUES
('admin', 'admin123', 'Admin'),
('petani', 'user123', 'User');

SELECT * FROM Penggilingan
SELECT * FROM Users


-- =========================================
-- VIEW
-- =========================================
CREATE VIEW vw_Penggilingan
AS
SELECT
    ID_Giling,
    NamaTempat,
    Alamat,
    Wilayah,
    StatusOperasional
FROM Penggilingan;
GO


-- =========================================
-- INSERT
-- =========================================
CREATE PROCEDURE sp_InsertPenggilingan
    @nama VARCHAR(100),
    @alamat VARCHAR(255),
    @wilayah VARCHAR(100),
    @status VARCHAR(10)
AS
BEGIN
    INSERT INTO Penggilingan
    (
        NamaTempat,
        Alamat,
        Wilayah,
        StatusOperasional
    )
    VALUES
    (
        @nama,
        @alamat,
        @wilayah,
        @status
    )
END
GO


-- =========================================
-- UPDATE
-- =========================================

CREATE PROCEDURE sp_UpdatePenggilingan
    @id INT,
    @nama VARCHAR(100),
    @alamat VARCHAR(255),
    @wilayah VARCHAR(100),
    @status VARCHAR(10)
AS
BEGIN
    UPDATE Penggilingan
    SET
        NamaTempat = @nama,
        Alamat = @alamat,
        Wilayah = @wilayah,
        StatusOperasional = @status
    WHERE ID_Giling = @id
END
GO


-- =========================================
-- DELETE
-- =========================================

CREATE PROCEDURE sp_DeletePenggilingan
    @id INT
AS
BEGIN
    DELETE FROM Penggilingan
    WHERE ID_Giling = @id
END
GO


-- =========================================
-- SEARCH
-- =========================================

CREATE PROCEDURE sp_SearchPenggilingan
    @wilayah VARCHAR(100),
    @status VARCHAR(10)
AS
BEGIN

    IF @status = 'Semua'
    BEGIN
        SELECT *
        FROM vw_Penggilingan
        WHERE Wilayah LIKE '%' + @wilayah + '%'
    END

    ELSE
    BEGIN
        SELECT *
        FROM vw_Penggilingan
        WHERE Wilayah LIKE '%' + @wilayah + '%'
        AND StatusOperasional = @status
    END

END
GO



-- =========================================
-- SQL INJECTION
-- =========================================
SELECT RoleUser FROM Users
WHERE Username='' OR '1'='1' 
AND Password='abc'


