-- =============================================
-- Script: Tạo bảng PhanCongCongViec
-- Database: DB_QLKhachSan
-- Author: System
-- Date: 2024
-- Description: Bảng phân công công việc dọn phòng cho nhân viên buồng phòng
-- =============================================

USE [DB_QLKhachSan]
GO

SET NOCOUNT ON;
GO

PRINT '========================================';
PRINT 'BẮT ĐẦU TẠO BẢNG PHÂN CÔNG CÔNG VIỆC';
PRINT 'Thời gian: ' + CONVERT(VARCHAR, GETDATE(), 120);
PRINT '========================================';
PRINT '';

-- =============================================
-- BƯỚC 1: KIỂM TRA BẢNG ĐÃ TỒN TẠI CHƯA
-- =============================================
PRINT 'BƯỚC 1: Kiểm tra bảng PhanCongCongViec...';
PRINT '----------------------------------------';

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PhanCongCongViec]') AND type in (N'U'))
BEGIN
    PRINT '  ⚠️  Bảng PhanCongCongViec đã tồn tại!';
    PRINT '  → Bỏ qua việc tạo bảng mới.';
    PRINT '';
END
ELSE
BEGIN
    PRINT '  ✓ Bảng PhanCongCongViec chưa tồn tại.';
    PRINT '  → Tiến hành tạo bảng mới...';
    PRINT '';

    -- =============================================
    -- BƯỚC 2: TẠO BẢNG PHANCONGCONGVIEC
    -- =============================================
    BEGIN TRANSACTION CreatePhanCongTable;
    
    BEGIN TRY
        CREATE TABLE [dbo].[PhanCongCongViec] (
            [PhanCongId] INT IDENTITY(1,1) NOT NULL,
            [MaPhanCong] NVARCHAR(50) NOT NULL, -- Mã phân công (VD: PC-20241107-001)
            [PhongId] INT NOT NULL, -- Phòng cần dọn
            [NhanVienBuongPhongId] INT NOT NULL, -- Nhân viên buồng phòng được phân công
            [NhanVienLeTanId] INT NULL, -- Nhân viên lễ tân phân công (có thể NULL nếu tự động)
            [TrangThaiCongViec] TINYINT NOT NULL DEFAULT 0, -- 0=Chờ xử lý, 1=Đang làm, 2=Hoàn thành, 3=Hủy
            [MucDoUuTien] TINYINT NOT NULL DEFAULT 1, -- 1=Thường, 2=Cao, 3=Khẩn cấp
            [GhiChu] NVARCHAR(500) NULL, -- Ghi chú từ lễ tân
            [GhiChuNhanVien] NVARCHAR(500) NULL, -- Ghi chú từ nhân viên buồng phòng
            [ThoiGianPhanCong] DATETIME NOT NULL DEFAULT GETDATE(), -- Thời gian phân công
            [ThoiGianBatDau] DATETIME NULL, -- Thời gian bắt đầu làm
            [ThoiGianHoanThanh] DATETIME NULL, -- Thời gian hoàn thành
            [ThoiGianDuKien] DATETIME NULL, -- Thời gian dự kiến hoàn thành
            [NgayTao] DATETIME NOT NULL DEFAULT GETDATE(),
            [NgayCapNhat] DATETIME NULL,
            CONSTRAINT [PK_PhanCongCongViec] PRIMARY KEY CLUSTERED ([PhanCongId] ASC),
            CONSTRAINT [FK_PhanCong_Phong] FOREIGN KEY ([PhongId]) 
                REFERENCES [dbo].[Phong] ([PhongId]) ON DELETE NO ACTION ON UPDATE NO ACTION,
            CONSTRAINT [FK_PhanCong_NhanVienBuongPhong] FOREIGN KEY ([NhanVienBuongPhongId]) 
                REFERENCES [dbo].[NhanVien] ([NhanVienId]) ON DELETE NO ACTION ON UPDATE NO ACTION,
            CONSTRAINT [FK_PhanCong_NhanVienLeTan] FOREIGN KEY ([NhanVienLeTanId]) 
                REFERENCES [dbo].[NhanVien] ([NhanVienId]) ON DELETE NO ACTION ON UPDATE NO ACTION,
            CONSTRAINT [CK_PhanCong_TrangThai] CHECK ([TrangThaiCongViec] IN (0, 1, 2, 3)),
            CONSTRAINT [CK_PhanCong_MucDoUuTien] CHECK ([MucDoUuTien] IN (1, 2, 3))
        );
        
        -- Tạo Index để tối ưu truy vấn
        CREATE NONCLUSTERED INDEX [IX_PhanCong_PhongId] 
            ON [dbo].[PhanCongCongViec] ([PhongId]);
        
        CREATE NONCLUSTERED INDEX [IX_PhanCong_NhanVienBuongPhongId] 
            ON [dbo].[PhanCongCongViec] ([NhanVienBuongPhongId]);
        
        CREATE NONCLUSTERED INDEX [IX_PhanCong_TrangThai] 
            ON [dbo].[PhanCongCongViec] ([TrangThaiCongViec]);
        
        CREATE NONCLUSTERED INDEX [IX_PhanCong_ThoiGianPhanCong] 
            ON [dbo].[PhanCongCongViec] ([ThoiGianPhanCong]);
        
        -- Tạo Unique Index cho MaPhanCong
        CREATE UNIQUE NONCLUSTERED INDEX [IX_PhanCong_MaPhanCong] 
            ON [dbo].[PhanCongCongViec] ([MaPhanCong]);
        
        COMMIT TRANSACTION CreatePhanCongTable;
        
        PRINT '  ✓ Tạo bảng PhanCongCongViec thành công!';
        PRINT '  ✓ Tạo các Index thành công!';
        PRINT '';
        
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION CreatePhanCongTable;
        
        PRINT '  ✗ LỖI khi tạo bảng PhanCongCongViec:';
        PRINT '    ' + ERROR_MESSAGE();
        PRINT '';
        THROW;
    END CATCH
END

-- =============================================
-- BƯỚC 3: TẠO FUNCTION TẠO MÃ PHÂN CÔNG TỰ ĐỘNG
-- =============================================
PRINT 'BƯỚC 3: Tạo Function tạo mã phân công tự động...';
PRINT '----------------------------------------';

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[fn_TaoMaPhanCong]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
    DROP FUNCTION [dbo].[fn_TaoMaPhanCong];
    PRINT '  → Xóa function cũ...';
END

BEGIN TRY
    EXEC('
    CREATE FUNCTION [dbo].[fn_TaoMaPhanCong]()
    RETURNS NVARCHAR(50)
    AS
    BEGIN
        DECLARE @MaPhanCong NVARCHAR(50);
        DECLARE @SoThuTu INT;
        DECLARE @NgayHienTai NVARCHAR(8) = CONVERT(NVARCHAR(8), GETDATE(), 112); -- YYYYMMDD
        
        -- Lấy số thứ tự lớn nhất trong ngày
        SELECT @SoThuTu = ISNULL(MAX(CAST(SUBSTRING(MaPhanCong, 12, LEN(MaPhanCong)) AS INT)), 0) + 1
        FROM [dbo].[PhanCongCongViec]
        WHERE MaPhanCong LIKE ''PC-'' + @NgayHienTai + ''-%'';
        
        -- Tạo mã: PC-YYYYMMDD-XXX
        SET @MaPhanCong = ''PC-'' + @NgayHienTai + ''-'' + RIGHT(''000'' + CAST(@SoThuTu AS NVARCHAR(3)), 3);
        
        RETURN @MaPhanCong;
    END
    ');
    
    PRINT '  ✓ Tạo Function fn_TaoMaPhanCong thành công!';
    PRINT '';
END TRY
BEGIN CATCH
    PRINT '  ✗ LỖI khi tạo Function:';
    PRINT '    ' + ERROR_MESSAGE();
    PRINT '';
END CATCH

-- =============================================
-- BƯỚC 4: TẠO TRIGGER TỰ ĐỘNG TẠO MÃ PHÂN CÔNG
-- =============================================
PRINT 'BƯỚC 4: Tạo Trigger tự động tạo mã phân công...';
PRINT '----------------------------------------';

IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_PhanCong_AutoMaPhanCong')
BEGIN
    DROP TRIGGER [dbo].[TR_PhanCong_AutoMaPhanCong];
    PRINT '  → Xóa trigger cũ...';
END

BEGIN TRY
    EXEC('
    CREATE TRIGGER [dbo].[TR_PhanCong_AutoMaPhanCong]
    ON [dbo].[PhanCongCongViec]
    INSTEAD OF INSERT
    AS
    BEGIN
        SET NOCOUNT ON;
        
        INSERT INTO [dbo].[PhanCongCongViec] (
            [MaPhanCong],
            [PhongId],
            [NhanVienBuongPhongId],
            [NhanVienLeTanId],
            [TrangThaiCongViec],
            [MucDoUuTien],
            [GhiChu],
            [GhiChuNhanVien],
            [ThoiGianPhanCong],
            [ThoiGianBatDau],
            [ThoiGianHoanThanh],
            [ThoiGianDuKien],
            [NgayTao],
            [NgayCapNhat]
        )
        SELECT 
            CASE 
                WHEN i.MaPhanCong IS NULL OR i.MaPhanCong = '''' THEN [dbo].[fn_TaoMaPhanCong]()
                ELSE i.MaPhanCong
            END,
            i.PhongId,
            i.NhanVienBuongPhongId,
            i.NhanVienLeTanId,
            ISNULL(i.TrangThaiCongViec, 0),
            ISNULL(i.MucDoUuTien, 1),
            i.GhiChu,
            i.GhiChuNhanVien,
            ISNULL(i.ThoiGianPhanCong, GETDATE()),
            i.ThoiGianBatDau,
            i.ThoiGianHoanThanh,
            i.ThoiGianDuKien,
            ISNULL(i.NgayTao, GETDATE()),
            i.NgayCapNhat
        FROM inserted i;
    END
    ');
    
    PRINT '  ✓ Tạo Trigger TR_PhanCong_AutoMaPhanCong thành công!';
    PRINT '';
END TRY
BEGIN CATCH
    PRINT '  ✗ LỖI khi tạo Trigger:';
    PRINT '    ' + ERROR_MESSAGE();
    PRINT '';
END CATCH

-- =============================================
-- BƯỚC 5: TẠO TRIGGER CẬP NHẬT NgayCapNhat
-- =============================================
PRINT 'BƯỚC 5: Tạo Trigger cập nhật NgayCapNhat...';
PRINT '----------------------------------------';

IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_PhanCong_UpdateNgayCapNhat')
BEGIN
    DROP TRIGGER [dbo].[TR_PhanCong_UpdateNgayCapNhat];
    PRINT '  → Xóa trigger cũ...';
END

BEGIN TRY
    EXEC('
    CREATE TRIGGER [dbo].[TR_PhanCong_UpdateNgayCapNhat]
    ON [dbo].[PhanCongCongViec]
    AFTER UPDATE
    AS
    BEGIN
        SET NOCOUNT ON;
        
        UPDATE [dbo].[PhanCongCongViec]
        SET [NgayCapNhat] = GETDATE()
        FROM [dbo].[PhanCongCongViec] p
        INNER JOIN inserted i ON p.PhanCongId = i.PhanCongId;
    END
    ');
    
    PRINT '  ✓ Tạo Trigger TR_PhanCong_UpdateNgayCapNhat thành công!';
    PRINT '';
END TRY
BEGIN CATCH
    PRINT '  ✗ LỖI khi tạo Trigger:';
    PRINT '    ' + ERROR_MESSAGE();
    PRINT '';
END CATCH

-- =============================================
-- HOÀN TẤT
-- =============================================
PRINT '========================================';
PRINT 'HOÀN TẤT TẠO BẢNG PHÂN CÔNG CÔNG VIỆC';
PRINT 'Thời gian: ' + CONVERT(VARCHAR, GETDATE(), 120);
PRINT '========================================';
PRINT '';
PRINT 'Cấu trúc bảng PhanCongCongViec:';
PRINT '  - PhanCongId: ID tự tăng (PK)';
PRINT '  - MaPhanCong: Mã phân công tự động (VD: PC-20241107-001)';
PRINT '  - PhongId: Phòng cần dọn (FK → Phong)';
PRINT '  - NhanVienBuongPhongId: Nhân viên buồng phòng (FK → NhanVien)';
PRINT '  - NhanVienLeTanId: Nhân viên lễ tân phân công (FK → NhanVien, nullable)';
PRINT '  - TrangThaiCongViec: 0=Chờ xử lý, 1=Đang làm, 2=Hoàn thành, 3=Hủy';
PRINT '  - MucDoUuTien: 1=Thường, 2=Cao, 3=Khẩn cấp';
PRINT '  - GhiChu: Ghi chú từ lễ tân';
PRINT '  - GhiChuNhanVien: Ghi chú từ nhân viên buồng phòng';
PRINT '  - ThoiGianPhanCong: Thời gian phân công';
PRINT '  - ThoiGianBatDau: Thời gian bắt đầu làm';
PRINT '  - ThoiGianHoanThanh: Thời gian hoàn thành';
PRINT '  - ThoiGianDuKien: Thời gian dự kiến hoàn thành';
PRINT '';
GO

