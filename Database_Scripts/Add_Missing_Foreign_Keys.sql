-- =============================================
-- Script: Thêm các khóa ngo?i còn thi?u
-- Database: DB_QLKhachSan
-- Author: System
-- Date: 2024
-- Description: Thêm FK constraints cho DatPhong ? NhanVien và DatPhong ? KhuyenMai
-- =============================================

USE [DB_QLKhachSan]
GO

SET NOCOUNT ON;
GO

PRINT '========================================';
PRINT 'B?T ??U THÊM KHÓA NGO?I CÒN THI?U';
PRINT 'Th?i gian: ' + CONVERT(VARCHAR, GETDATE(), 120);
PRINT '========================================';
PRINT '';

-- =============================================
-- B??C 1: KI?M TRA VÀ BACKUP D? LI?U
-- =============================================
PRINT 'B??C 1: Ki?m tra d? li?u hi?n t?i...';
PRINT '----------------------------------------';

DECLARE @TongDonDatPhong INT;
DECLARE @DonCoNhanVien INT;
DECLARE @DonCoKhuyenMai INT;

SELECT @TongDonDatPhong = COUNT(*) FROM DatPhong;
SELECT @DonCoNhanVien = COUNT(*) FROM DatPhong WHERE NhanVienId IS NOT NULL;
SELECT @DonCoKhuyenMai = COUNT(*) FROM DatPhong WHERE MaKhuyenMai IS NOT NULL;

PRINT '  ?? T?ng s? ??n ??t phòng: ' + CAST(@TongDonDatPhong AS VARCHAR);
PRINT '  ?? S? ??n có NhanVienId: ' + CAST(@DonCoNhanVien AS VARCHAR);
PRINT '  ?? S? ??n có MaKhuyenMai: ' + CAST(@DonCoKhuyenMai AS VARCHAR);
PRINT '';

-- =============================================
-- B??C 2: X? LÝ D? LI?U ORPHAN (N?U CÓ)
-- =============================================
BEGIN TRANSACTION CleanupOrphanData;
GO

PRINT 'B??C 2: X? lý d? li?u không h?p l?...';
PRINT '----------------------------------------';

-- 2.1: Ki?m tra và x? lý NhanVienId không h?p l?
DECLARE @OrphanNhanVien INT;
SELECT @OrphanNhanVien = COUNT(*) 
FROM DatPhong dp
WHERE dp.NhanVienId IS NOT NULL 
AND NOT EXISTS (SELECT 1 FROM NhanVien nv WHERE nv.NhanVienId = dp.NhanVienId);

IF @OrphanNhanVien > 0
BEGIN
    PRINT '  ?? Phát hi?n ' + CAST(@OrphanNhanVien AS VARCHAR) + ' ??n có NhanVienId không h?p l?';
    PRINT '  ?? ?ang s?a: Set NhanVienId = NULL...';
    
    UPDATE DatPhong
    SET NhanVienId = NULL,
        GhiChu = ISNULL(GhiChu, '') + CHAR(13) + CHAR(10) + 
      '[' + CONVERT(VARCHAR, GETDATE(), 120) + '] Auto-fix: Invalid NhanVienId removed'
    WHERE NhanVienId IS NOT NULL 
    AND NOT EXISTS (SELECT 1 FROM NhanVien nv WHERE nv.NhanVienId = DatPhong.NhanVienId);
    
    PRINT '  ? ?ã x? lý ' + CAST(@OrphanNhanVien AS VARCHAR) + ' b?n ghi';
END
ELSE
BEGIN
    PRINT '  ? D? li?u NhanVienId h?p l?';
END
PRINT '';

-- 2.2: Ki?m tra và x? lý MaKhuyenMai không h?p l?
DECLARE @OrphanKhuyenMai INT;
SELECT @OrphanKhuyenMai = COUNT(*) 
FROM DatPhong dp
WHERE dp.MaKhuyenMai IS NOT NULL 
AND NOT EXISTS (SELECT 1 FROM KhuyenMai km WHERE km.KhuyenMaiId = dp.MaKhuyenMai);

IF @OrphanKhuyenMai > 0
BEGIN
    PRINT '  ?? Phát hi?n ' + CAST(@OrphanKhuyenMai AS VARCHAR) + ' ??n có MaKhuyenMai không h?p l?';
    PRINT '  ?? ?ang s?a: Set MaKhuyenMai = NULL...';
  
    UPDATE DatPhong
    SET MaKhuyenMai = NULL,
        GhiChu = ISNULL(GhiChu, '') + CHAR(13) + CHAR(10) + 
     '[' + CONVERT(VARCHAR, GETDATE(), 120) + '] Auto-fix: Invalid MaKhuyenMai removed'
    WHERE MaKhuyenMai IS NOT NULL 
    AND NOT EXISTS (SELECT 1 FROM KhuyenMai km WHERE km.KhuyenMaiId = DatPhong.MaKhuyenMai);
    
    PRINT '  ? ?ã x? lý ' + CAST(@OrphanKhuyenMai AS VARCHAR) + ' b?n ghi';
END
ELSE
BEGIN
    PRINT '  ? D? li?u MaKhuyenMai h?p l?';
END
PRINT '';

COMMIT TRANSACTION CleanupOrphanData;
GO

-- =============================================
-- B??C 3: THÊM FOREIGN KEY CONSTRAINTS
-- =============================================
BEGIN TRANSACTION AddForeignKeys;
GO

PRINT 'B??C 3: Thêm Foreign Key Constraints...';
PRINT '----------------------------------------';

-- 3.1: Thêm FK DatPhong ? NhanVien
IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys 
    WHERE name = 'FK_DatPhong_NhanVien' 
    AND parent_object_id = OBJECT_ID('DatPhong')
)
BEGIN
    PRINT '  ?? ?ang thêm FK_DatPhong_NhanVien...';
    
    ALTER TABLE [dbo].[DatPhong]
    ADD CONSTRAINT [FK_DatPhong_NhanVien] 
    FOREIGN KEY ([NhanVienId]) 
    REFERENCES [dbo].[NhanVien]([NhanVienId])
    ON DELETE SET NULL      -- N?u xóa nhân viên ? set NULL cho ??n ??t
    ON UPDATE CASCADE;      -- N?u update ID nhân viên ? cascade update
    
  PRINT '  ? ?ã thêm FK_DatPhong_NhanVien thành công!';
PRINT '     - ON DELETE: SET NULL';
    PRINT '     - ON UPDATE: CASCADE';
END
ELSE
BEGIN
    PRINT '  ?? FK_DatPhong_NhanVien ?ã t?n t?i, b? qua';
END
PRINT '';

-- 3.2: Thêm FK DatPhong ? KhuyenMai
IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys 
  WHERE name = 'FK_DatPhong_KhuyenMai' 
    AND parent_object_id = OBJECT_ID('DatPhong')
)
BEGIN
    PRINT '  ?? ?ang thêm FK_DatPhong_KhuyenMai...';
    
    ALTER TABLE [dbo].[DatPhong]
    ADD CONSTRAINT [FK_DatPhong_KhuyenMai] 
    FOREIGN KEY ([MaKhuyenMai]) 
    REFERENCES [dbo].[KhuyenMai]([KhuyenMaiId])
    ON DELETE SET NULL  -- N?u xóa khuy?n mãi ? set NULL (gi? l?ch s?)
    ON UPDATE CASCADE;
    
    PRINT '  ? ?ã thêm FK_DatPhong_KhuyenMai thành công!';
  PRINT '     - ON DELETE: SET NULL';
    PRINT '     - ON UPDATE: CASCADE';
END
ELSE
BEGIN
    PRINT '  ?? FK_DatPhong_KhuyenMai ?ã t?n t?i, b? qua';
END
PRINT '';

COMMIT TRANSACTION AddForeignKeys;
GO

-- =============================================
-- B??C 4: T?O INDEX ?? T?I ?U HI?U SU?T
-- =============================================
BEGIN TRANSACTION CreateIndexes;
GO

PRINT 'B??C 4: T?o Index ?? t?i ?u hi?u su?t...';
PRINT '----------------------------------------';

-- 4.1: Index cho DatPhong.NhanVienId
IF NOT EXISTS (
 SELECT 1 FROM sys.indexes 
    WHERE name = 'IX_DatPhong_NhanVienId' 
    AND object_id = OBJECT_ID('DatPhong')
)
BEGIN
    PRINT '  ?? ?ang t?o index IX_DatPhong_NhanVienId...';
    
 CREATE NONCLUSTERED INDEX [IX_DatPhong_NhanVienId]
  ON [dbo].[DatPhong] ([NhanVienId])
    INCLUDE ([MaDatPhong], [NgayTao], [TrangThaiDatPhong], [TongTien])
    WITH (FILLFACTOR = 90, PAD_INDEX = ON);
    
    PRINT '  ? ?ã t?o index IX_DatPhong_NhanVienId';
END
ELSE
BEGIN
    PRINT '  ?? Index IX_DatPhong_NhanVienId ?ã t?n t?i';
END
PRINT '';

-- 4.2: Index cho DatPhong.MaKhuyenMai
IF NOT EXISTS (
 SELECT 1 FROM sys.indexes 
    WHERE name = 'IX_DatPhong_MaKhuyenMai' 
    AND object_id = OBJECT_ID('DatPhong')
)
BEGIN
  PRINT '  ?? ?ang t?o index IX_DatPhong_MaKhuyenMai...';
    
    CREATE NONCLUSTERED INDEX [IX_DatPhong_MaKhuyenMai]
    ON [dbo].[DatPhong] ([MaKhuyenMai])
    INCLUDE ([TongTien], [NgayDat], [TrangThaiDatPhong])
    WITH (FILLFACTOR = 90, PAD_INDEX = ON);
    
    PRINT '  ? ?ã t?o index IX_DatPhong_MaKhuyenMai';
END
ELSE
BEGIN
    PRINT '  ?? Index IX_DatPhong_MaKhuyenMai ?ã t?n t?i';
END
PRINT '';

COMMIT TRANSACTION CreateIndexes;
GO

-- =============================================
-- B??C 5: KI?M TRA K?T QU?
-- =============================================
PRINT 'B??C 5: Ki?m tra k?t qu?...';
PRINT '----------------------------------------';

DECLARE @FKCount INT;
DECLARE @IndexCount INT;

-- ??m s? FK trên b?ng DatPhong
SELECT @FKCount = COUNT(*)
FROM sys.foreign_keys
WHERE parent_object_id = OBJECT_ID('DatPhong');

-- ??m s? index trên b?ng DatPhong
SELECT @IndexCount = COUNT(*)
FROM sys.indexes
WHERE object_id = OBJECT_ID('DatPhong')
AND name IN ('IX_DatPhong_NhanVienId', 'IX_DatPhong_MaKhuyenMai');

PRINT '  ?? T?ng s? Foreign Key trên DatPhong: ' + CAST(@FKCount AS VARCHAR);
PRINT '  ?? S? index m?i ???c t?o: ' + CAST(@IndexCount AS VARCHAR);
PRINT '';

-- Li?t kê t?t c? FK trên DatPhong
PRINT '  ?? Danh sách Foreign Keys hi?n t?i:';
PRINT '  ????????????????????????????????????????????????????????????';

SELECT 
    '  - ' + fk.name + ' (' + 
  OBJECT_NAME(fk.parent_object_id) + '.' + c_from.name + 
    ' ? ' + 
    OBJECT_NAME(fk.referenced_object_id) + '.' + c_to.name + ')'
    AS ForeignKeyInfo
FROM sys.foreign_keys fk
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
INNER JOIN sys.columns c_from ON fkc.parent_object_id = c_from.object_id 
    AND fkc.parent_column_id = c_from.column_id
INNER JOIN sys.columns c_to ON fkc.referenced_object_id = c_to.object_id 
    AND fkc.referenced_column_id = c_to.column_id
WHERE fk.parent_object_id = OBJECT_ID('DatPhong')
ORDER BY fk.name;

PRINT '';
PRINT '  ?? Danh sách Index m?i:';
PRINT '  ????????????????????????????????????????????????????????????';

SELECT 
    '  - ' + i.name + ' trên c?t [' + 
    STRING_AGG(c.name, ', ') + ']'
    AS IndexInfo
FROM sys.indexes i
INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id 
    AND i.index_id = ic.index_id
INNER JOIN sys.columns c ON ic.object_id = c.object_id 
    AND ic.column_id = c.column_id
WHERE i.object_id = OBJECT_ID('DatPhong')
AND i.name IN ('IX_DatPhong_NhanVienId', 'IX_DatPhong_MaKhuyenMai')
AND ic.is_included_column = 0
GROUP BY i.name;

PRINT '';

-- =============================================
-- B??C 6: TH?NG KÊ SAU KHI C?P NH?T
-- =============================================
PRINT 'B??C 6: Th?ng kê sau khi c?p nh?t...';
PRINT '----------------------------------------';

SELECT 
    '  T?ng ??n: ' + CAST(COUNT(*) AS VARCHAR) AS ThongKe
FROM DatPhong
UNION ALL
SELECT 
    '  ??n có NhanVienId: ' + CAST(COUNT(*) AS VARCHAR)
FROM DatPhong WHERE NhanVienId IS NOT NULL
UNION ALL
SELECT 
    '  ??n có MaKhuyenMai: ' + CAST(COUNT(*) AS VARCHAR)
FROM DatPhong WHERE MaKhuyenMai IS NOT NULL;

PRINT '';

-- =============================================
-- K?T THÚC
-- =============================================
PRINT '========================================';
PRINT '? HOÀN THÀNH SCRIPT THÀNH CÔNG!';
PRINT 'Th?i gian: ' + CONVERT(VARCHAR, GETDATE(), 120);
PRINT '========================================';
PRINT '';

PRINT '?? L?U Ý QUAN TR?NG:';
PRINT '  ??????????????????????????????????????????????????????????';
PRINT '  ? 1. C?n UPDATE Entity Framework Model (.edmx)      ?';
PRINT '  ?    - Cách 1: Update Model from Database (khuy?n ngh?) ?';
PRINT '  ?    - Cách 2: Xóa .edmx và t?o l?i t? database         ?';
PRINT '  ?              ?';
PRINT '  ? 2. Thêm Navigation Properties th? công:               ?';
PRINT '  ?    - Models/DatPhong.cs            ?';
PRINT '  ?      + public virtual NhanVien NhanVien { get; set; } ?';
PRINT '  ?      + public virtual KhuyenMai KhuyenMai{get;set;}   ?';
PRINT '  ?           ?';
PRINT '  ?    - Models/NhanVien.cs        ?';
PRINT '  ?      + public virtual ICollection<DatPhong>           ?';
PRINT '  ?        DatPhongs { get; set; }       ?';
PRINT '  ?     ?';
PRINT '  ?    - Models/KhuyenMai.cs       ?';
PRINT '  ?      + public virtual ICollection<DatPhong>           ?';
PRINT '  ?      DatPhongs { get; set; }      ?';
PRINT '  ? ?';
PRINT '  ? 3. Build l?i project sau khi update             ?';
PRINT '  ??????????????????????????????????????????????????????????';
PRINT '';

PRINT '?? CÁCH S? D?NG NAVIGATION PROPERTY M?I:';
PRINT '  ??????????????????????????????????????????????????????????';
PRINT '  ? // L?y thông tin nhân viên t?o ??n  ?';
PRINT '  ? var datPhong = db.DatPhongs        ?';
PRINT '  ?     .Include(dp => dp.NhanVien)        ?';
PRINT '  ?     .Include(dp => dp.KhuyenMai)    ?';
PRINT '  ?     .FirstOrDefault(dp => dp.DatPhongId == id);       ?';
PRINT '  ?        ?';
PRINT '  ? string tenNV = datPhong.NhanVien?.HoVaTen;        ?';
PRINT '  ? string maKM = datPhong.KhuyenMai?.MaKhuyenMai;        ?';
PRINT '  ??????????????????????????????????????????????????????????';
PRINT '';

SET NOCOUNT OFF;
GO

-- =============================================
-- ROLLBACK SCRIPT (CH?Y N?U C?N H?Y)
-- =============================================
/*
-- ?? CH? CH?Y KHI C?N ROLLBACK ??
-- Uncomment và ch?y script này n?u mu?n h?y các thay ??i

USE [DB_QLKhachSan]
GO

PRINT '?? B?T ??U ROLLBACK...';
PRINT '';

-- Xóa Index
PRINT '  ??? ?ang xóa index...';
DROP INDEX IF EXISTS [IX_DatPhong_NhanVienId] ON [dbo].[DatPhong];
DROP INDEX IF EXISTS [IX_DatPhong_MaKhuyenMai] ON [dbo].[DatPhong];
PRINT '  ? ?ã xóa index';
PRINT '';

-- Xóa Foreign Keys
PRINT '  ??? ?ang xóa Foreign Keys...';
ALTER TABLE [dbo].[DatPhong] DROP CONSTRAINT IF EXISTS [FK_DatPhong_NhanVien];
ALTER TABLE [dbo].[DatPhong] DROP CONSTRAINT IF EXISTS [FK_DatPhong_KhuyenMai];
PRINT '  ? ?ã xóa Foreign Keys';
PRINT '';

PRINT '? ROLLBACK HOÀN T?T!';
GO
*/
