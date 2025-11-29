-- =============================================
-- Script: Sửa lại trigger để tránh duplicate key
-- =============================================

USE [DB_QLKhachSan]
GO

-- Xóa trigger cũ
IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_PhanCong_AutoMaPhanCong')
BEGIN
    DROP TRIGGER [dbo].[TR_PhanCong_AutoMaPhanCong];
    PRINT 'Đã xóa trigger cũ';
END
GO

-- Tạo lại trigger với logic cải thiện
CREATE TRIGGER [dbo].[TR_PhanCong_AutoMaPhanCong]
ON [dbo].[PhanCongCongViec]
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @PhanCongId INT;
    DECLARE @MaPhanCong NVARCHAR(50);
    DECLARE @NgayHienTai NVARCHAR(8);
    DECLARE @SoThuTu INT;
    DECLARE @RetryCount INT;
    DECLARE @MaxRetries INT = 10;
    
    DECLARE cur CURSOR FOR
    SELECT PhanCongId
    FROM inserted
    WHERE MaPhanCong IS NULL OR MaPhanCong = '';
    
    OPEN cur;
    FETCH NEXT FROM cur INTO @PhanCongId;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @RetryCount = 0;
        SET @NgayHienTai = CONVERT(NVARCHAR(8), GETDATE(), 112);
        
        -- Retry logic để tránh duplicate key
        WHILE @RetryCount < @MaxRetries
        BEGIN
            BEGIN TRY
                -- Lấy số thứ tự lớn nhất trong ngày
                SELECT @SoThuTu = ISNULL(MAX(CAST(SUBSTRING(MaPhanCong, 12, LEN(MaPhanCong)) AS INT)), 0) + 1
                FROM [dbo].[PhanCongCongViec] WITH (UPDLOCK, HOLDLOCK)
                WHERE MaPhanCong LIKE 'PC-' + @NgayHienTai + '-%';
                
                -- Tạo mã: PC-YYYYMMDD-XXX
                SET @MaPhanCong = 'PC-' + @NgayHienTai + '-' + RIGHT('000' + CAST(@SoThuTu AS NVARCHAR(3)), 3);
                
                -- Cập nhật MaPhanCong
                UPDATE [dbo].[PhanCongCongViec]
                SET [MaPhanCong] = @MaPhanCong,
                    [NgayCapNhat] = GETDATE()
                WHERE [PhanCongId] = @PhanCongId
                    AND ([MaPhanCong] IS NULL OR [MaPhanCong] = '');
                
                -- Nếu update thành công, break khỏi retry loop
                IF @@ROWCOUNT > 0
                    BREAK;
                    
            END TRY
            BEGIN CATCH
                -- Nếu lỗi duplicate key (2601), retry với số thứ tự tăng lên
                IF ERROR_NUMBER() = 2601
                BEGIN
                    SET @RetryCount = @RetryCount + 1;
                    SET @SoThuTu = @SoThuTu + 1;
                    -- Nếu đã retry quá nhiều, dùng timestamp để tạo unique
                    IF @RetryCount >= @MaxRetries
                    BEGIN
                        SET @MaPhanCong = 'PC-' + @NgayHienTai + '-' + RIGHT('000' + CAST(@SoThuTu AS NVARCHAR(3)), 3) + '-' + RIGHT('000' + CAST(DATEPART(MILLISECOND, GETDATE()) AS NVARCHAR(3)), 3);
                    END
                END
                ELSE
                BEGIN
                    -- Nếu lỗi khác, throw lại
                    THROW;
                END
            END CATCH
        END
        
        FETCH NEXT FROM cur INTO @PhanCongId;
    END;
    
    CLOSE cur;
    DEALLOCATE cur;
END
GO

PRINT 'Đã tạo trigger TR_PhanCong_AutoMaPhanCong với retry logic';
GO

