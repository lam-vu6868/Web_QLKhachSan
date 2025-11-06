// File: DichVuToken.cs
using System;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Lớp 'static' (tĩnh) cung cấp các hàm để tạo và băm token
/// cho chức năng "Ghi nhớ đăng nhập".
/// </summary>
public static class DichVuToken
{
    /// <summary>
    /// Tạo một chuỗi token ngẫu nhiên, an toàn về mặt mật mã.
    /// Dùng để tạo cả MaTraCuu (Selector) và MaXacThuc (Validator).
    /// </summary>
    /// <param name="soByte">Độ dài của token (tính bằng byte). 32 là rất mạnh.</param>
    /// <returns>Một chuỗi Base64 an toàn.</returns>
    public static string TaoTokenAnToan(int soByte = 32)
    {
        // 1. Tạo một mảng byte để chứa số ngẫu nhiên
        byte[] tokenData = new byte[soByte];

        // 2. Sử dụng 'using' để đảm bảo đối tượng được giải phóng
        using (var rng = RandomNumberGenerator.Create())
        {
            // 3. Tạo ra các byte ngẫu nhiên an toàn
            rng.GetBytes(tokenData);
        }

        // 4. Chuyển mảng byte thành chuỗi Base64
        return Convert.ToBase64String(tokenData);
    }

    /// <summary>
    /// Băm (hash) một chuỗi đầu vào bằng thuật toán SHA256.
    /// Dùng để băm MaXacThuc (Validator) trước khi lưu vào CSDL.
    /// </summary>
    /// <param name="input">Chuỗi Validator gốc (chưa băm).</param>
    /// <returns>Một chuỗi Base64 đại diện cho giá trị đã băm.</returns>
    public static string BamSHA256(string input)
    {
        // 1. Sử dụng 'using' để đảm bảo đối tượng được giải phóng
        using (var sha256 = SHA256.Create())
        {
            // 2. Chuyển chuỗi input thành mảng byte (dùng UTF-8)
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            // 3. Băm mảng byte đó
            byte[] hashedBytes = sha256.ComputeHash(inputBytes);

            // 4. Chuyển kết quả băm thành chuỗi Base64 để lưu trữ
            return Convert.ToBase64String(hashedBytes);
        }
    }
}