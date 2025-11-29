-- =============================================
-- Script: Sửa lại bảng PhanCongCongViec
-- Mục đích: Đơn giản hóa trigger để tương thích với Entity Framework
-- =============================================

USE [DB_QLKhachSan]
GO

PRINT '========================================';
PRINT 'SỬA LẠI BẢNG PhanCongCongViec';
PRINT '========================================';
GO

-- ===== BƯỚC 1: XÓA TRIGGER CŨ =====
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_PhanCong_AutoMaPhanCong')
BEGIN
    DROP TRIGGER [dbo].[TR_PhanCong_AutoMaPhanCong];
    PRINT 'Đã xóa trigger TR_PhanCong_AutoMaPhanCong';
END
GO

-- ===== BƯỚC 2: XÓA FUNCTION CŨ =====
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[fn_TaoMaPhanCong]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
BEGIN
    DROP FUNCTION [dbo].[fn_TaoMaPhanCong];
    PRINT 'Đã xóa function fn_TaoMaPhanCong';
END
GO

-- ===== BƯỚC 3: TẠO LẠI FUNCTION =====
CREATE FUNCTION [dbo].[fn_TaoMaPhanCong]()
RETURNS NVARCHAR(50)
AS
BEGIN
    DECLARE @MaPhanCong NVARCHAR(50);
    DECLARE @SoThuTu INT;
    DECLARE @NgayHienTai NVARCHAR(8);
    
    SET @NgayHienTai = CONVERT(NVARCHAR(8), GETDATE(), 112);
    
    SELECT @SoThuTu = ISNULL(MAX(CAST(SUBSTRING(MaPhanCong, 12, LEN(MaPhanCong)) AS INT)), 0) + 1
    FROM [dbo].[PhanCongCongViec]
    WHERE MaPhanCong LIKE 'PC-' + @NgayHienTai + '-%';
    
    SET @MaPhanCong = 'PC-' + @NgayHienTai + '-' + RIGHT('000' + CAST(@SoThuTu AS NVARCHAR(3)), 3);
    
    RETURN @MaPhanCong;
END
GO

PRINT 'Đã tạo function fn_TaoMaPhanCong';
GO

-- ===== BƯỚC 4: TẠO TRIGGER AFTER INSERT =====
CREATE TRIGGER [dbo].[TR_PhanCong_AutoMaPhanCong]
ON [dbo].[PhanCongCongViec]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @PhanCongId INT;
    DECLARE @MaPhanCong NVARCHAR(50);
    
    DECLARE cur CURSOR FOR
    SELECT PhanCongId, MaPhanCong
    FROM inserted
    WHERE MaPhanCong IS NULL OR MaPhanCong = '';
    
    OPEN cur;
    FETCH NEXT FROM cur INTO @PhanCongId, @MaPhanCong;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @MaPhanCong = [dbo].[fn_TaoMaPhanCong]();
        
        UPDATE [dbo].[PhanCongCongViec]
        SET [MaPhanCong] = @MaPhanCong,
            [NgayCapNhat] = GETDATE()
        WHERE [PhanCongId] = @PhanCongId;
        
        FETCH NEXT FROM cur INTO @PhanCongId, @MaPhanCong;
    END;
    
    CLOSE cur;
    DEALLOCATE cur;
END
GO

PRINT 'Đã tạo trigger TR_PhanCong_AutoMaPhanCong (AFTER INSERT)';
GO

-- ===== BƯỚC 5: XÓA TRIGGER UPDATE CŨ =====
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_PhanCong_UpdateNgayCapNhat')
BEGIN
    DROP TRIGGER [dbo].[TR_PhanCong_UpdateNgayCapNhat];
    PRINT 'Đã xóa trigger TR_PhanCong_UpdateNgayCapNhat';
END
GO

-- ===== BƯỚC 6: TẠO TRIGGER UPDATE NgayCapNhat =====
CREATE TRIGGER [dbo].[TR_PhanCong_UpdateNgayCapNhat]
ON [dbo].[PhanCongCongViec]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE [dbo].[PhanCongCongViec]
    SET [NgayCapNhat] = GETDATE()
    WHERE [PhanCongId] IN (SELECT [PhanCongId] FROM inserted);
END
GO

PRINT 'Đã tạo trigger TR_PhanCong_UpdateNgayCapNhat';
GO

-- ===== BƯỚC 7: CẬP NHẬT DỮ LIỆU CŨ =====
DECLARE @Count INT;
SELECT @Count = COUNT(*) 
FROM [dbo].[PhanCongCongViec] 
WHERE MaPhanCong IS NULL OR MaPhanCong = '';

IF @Count > 0
BEGIN
    DECLARE @PhanCongId INT;
    DECLARE @MaPhanCong NVARCHAR(50);
    
    DECLARE cur2 CURSOR FOR
    SELECT PhanCongId
    FROM [dbo].[PhanCongCongViec]
    WHERE MaPhanCong IS NULL OR MaPhanCong = '';
    
    OPEN cur2;
    FETCH NEXT FROM cur2 INTO @PhanCongId;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @MaPhanCong = [dbo].[fn_TaoMaPhanCong]();
        
        UPDATE [dbo].[PhanCongCongViec]
        SET [MaPhanCong] = @MaPhanCong,
            [NgayCapNhat] = GETDATE()
        WHERE [PhanCongId] = @PhanCongId;
        
        FETCH NEXT FROM cur2 INTO @PhanCongId;
    END;
    
    CLOSE cur2;
    DEALLOCATE cur2;
    
    PRINT 'Đã cập nhật ' + CAST(@Count AS NVARCHAR(10)) + ' bản ghi có MaPhanCong NULL';
END
ELSE
BEGIN
    PRINT 'Không có bản ghi nào cần cập nhật';
END
GO

PRINT '';
PRINT '========================================';
PRINT 'HOÀN TẤT!';
PRINT '========================================';
PRINT 'Thay đổi:';
PRINT '  - Xóa INSTEAD OF trigger';
PRINT '  - Tạo AFTER INSERT trigger';
PRINT '  - Trigger tự động tạo MaPhanCong sau khi insert';
PRINT '  - Entity Framework có thể lấy PhanCongId bình thường';
GO
