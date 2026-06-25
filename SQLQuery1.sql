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

-- 2. Reset hitungan ID Auto-Increment (IDENTITY) kembali ke angka 0
DBCC CHECKIDENT ('Penggilingan', RESEED, 0);
GO
-- 3. Menambahkan Data
INSERT INTO Penggilingan (NamaTempat, Alamat, Wilayah, StatusOperasional)
VALUES ('Sari Tani', 'Jl. Sawah No. 5', 'Kasihan', 'Buka'),
       ('Maju Jaya', 'Depan Balai Desa', 'Bantul', 'Tutup');
GO


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

SELECT * FROM vw_Penggilingan


-- =========================================
-- STORED PROCEDURE INSERT
-- =========================================
ALTER PROCEDURE sp_InsertPenggilingan
    @nama VARCHAR(100),
    @alamat VARCHAR(255),
    @wilayah VARCHAR(100),
    @status VARCHAR(10),
    @foto VARBINARY(MAX)
AS
BEGIN
    INSERT INTO Penggilingan
    (
        NamaTempat,
        Alamat,
        Wilayah,
        StatusOperasional,
        Foto
    )
    VALUES
    (
        @nama,
        @alamat,
        @wilayah,
        @status,
        @foto
    )
END
GO

-- =========================================
-- STORED PROCEDURE UPDATE
-- =========================================

USE DBGilingPadi;
GO

ALTER PROCEDURE sp_UpdatePenggilingan
    @id INT,
    @nama VARCHAR(100),
    @alamat VARCHAR(255),
    @wilayah VARCHAR(100),
    @status VARCHAR(10),
    @foto VARBINARY(MAX)
AS
BEGIN
    UPDATE Penggilingan
    SET
        NamaTempat = @nama,
        Alamat = @alamat,
        Wilayah = @wilayah,
        StatusOperasional = @status,
        Foto = @foto
    WHERE ID_Giling = @id;
END;
GO


-- =========================================
-- STORED PROCEDURE DELETE
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
-- STORED PROCEDURE SEARCH
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



-- =========================================================================
-- 1. MEMBUAT TABEL ERROR LOGGING SYSTEM
-- =========================================================================
CREATE TABLE LogError
(
    id_log INT IDENTITY(1,1) PRIMARY KEY,
    waktu DATETIME DEFAULT GETDATE(),
    pesan_error VARCHAR(MAX)
);
GO



-- =========================================================================
-- 2. MEMBUAT TABEL LOG AKTIVITAS
-- =========================================================================
CREATE TABLE LogAktivitas
(
    id_log INT IDENTITY(1,1),
    aktivitas VARCHAR(100),
    waktu DATETIME DEFAULT GETDATE()
);
GO



-- =========================================================================
-- 3. MEMBUAT TRIGGER INSERT
-- =========================================================================
CREATE TRIGGER trg_InsertPenggilingan
ON Penggilingan
AFTER INSERT
AS
BEGIN
    INSERT INTO LogAktivitas (aktivitas, waktu)
    VALUES ('Tambah data tempat penggilingan baru', GETDATE());
END;
GO



-- =========================================================================
-- 4. MEMBUAT TRIGGER DELETE
-- =========================================================================
CREATE TRIGGER trg_DeletePenggilingan
ON Penggilingan
AFTER DELETE
AS
BEGIN
    INSERT INTO LogAktivitas (aktivitas, waktu)
    VALUES ('Hapus data tempat penggilingan', GETDATE());
END;
GO


-- =========================================================================
-- 5. MEMBUAT TABEL SECURITY LOG
-- =========================================================================
CREATE TABLE LogKeamanan
(
    id_log INT IDENTITY(1,1),
    aktivitas VARCHAR(200),
    jumlah_data INT,
    waktu DATETIME DEFAULT GETDATE()
);
GO



-- =========================================================================
-- 3. MEMBUAT TRIGGER MonitoringUPDATE
-- =========================================================================
-- Jika aplikasi mencoba mengubah lebih dari 5 baris sekaligus, transaksi akan dibatalkan otomatis
CREATE TRIGGER trg_PreventMassUpdatePenggilingan
ON Penggilingan
AFTER UPDATE
AS
BEGIN
    
    DECLARE @jumlah INT;
    -- Menghitung total baris yang terpengaruh dalam tabel maya 'inserted'
    SELECT @jumlah = COUNT(*) FROM inserted;

    -- Proteksi: Jika update mempengaruhi lebih dari 5 data sekaligus
    IF @jumlah > 5
    BEGIN
        -- 1. Catat ke tabel log keamanan
        INSERT INTO LogKeamanan (aktivitas, jumlah_data, waktu)
        VALUES ('WARNING: Terdeteksi upaya update massal pada data penggilingan!', @jumlah, GETDATE());

        -- 2. Batalkan seluruh rangkaian transaksi query tersebut
        ROLLBACK TRANSACTION;

        -- 3. Lempar pesan error kembali ke aplikasi ADO.NET
        RAISERROR('Update dibatalkan! Terlalu banyak data yang mencoba diubah sekaligus demi keamanan.', 16, 1);
    END
END;
GO

-- Melihat Log Aktivitas
select * from LogAktivitas

-- Melihat Log Error
select * from LogError

-- Melihat Log Keamanan
select * from LogKeamanan




-- =========================================================================
-- 6. MEMBUAT TABEL LOG AKTIVITAS SALAH
-- =========================================================================
CREATE TABLE LogAktivitasSalah
(
    id_log INT IDENTITY(1,1) PRIMARY KEY,
    aktivitas VARCHAR(100),
    waktu DATETIME
);
GO



-- =========================================
-- STORED PROCEDURE CETAK LAPORAN
-- =========================================
ALTER PROCEDURE sp_ReportPenggilingan
    @inWilayah VARCHAR(50),
    @inStatus VARCHAR(20)
AS
BEGIN
    SELECT 
        ID_Giling,
        NamaTempat,
        Alamat,
        Wilayah,
        StatusOperasional,
        Foto 
    FROM 
        Penggilingan
    WHERE 
        Wilayah = @inWilayah AND StatusOperasional = @inStatus;
END;
GO



-- =========================================
--	MENAMBAHKAN FOTO
-- =========================================
ALTER TABLE Penggilingan
ADD Foto VARBINARY(MAX);