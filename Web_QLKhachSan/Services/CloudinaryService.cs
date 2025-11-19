using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;
using System.Configuration;
using System.IO;
using System.Web;

namespace Web_QLKhachSan.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService()
        {
            var cloudName = ConfigurationManager.AppSettings["CloudinaryCloudName"];
            var apiKey = ConfigurationManager.AppSettings["CloudinaryApiKey"];
            var apiSecret = ConfigurationManager.AppSettings["CloudinaryApiSecret"];

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        /// <summary>
        /// Upload ảnh avatar khách hàng lên Cloudinary
        /// </summary>
        /// <param name="file">File ảnh từ form</param>
        /// <param name="maKhachHang">Mã khách hàng</param>
        /// <param name="hoVaTen">Họ và tên khách hàng</param>
        /// <returns>URL của ảnh đã upload</returns>
        public string UploadAvatarKhachHang(HttpPostedFileBase file, int maKhachHang, string hoVaTen)
        {
            try
            {
                if (file == null || file.ContentLength == 0)
                {
                    throw new Exception("File không hợp lệ");
                }

                // Tạo folder path: LuuTruAnh_QLKhachSan/AnhKhachHang/Avatar/{hoVaTen}_MaKhachHang{maKhachHang}
                var folderName = $"{hoVaTen}_MaKhachHang-{maKhachHang}";
                var folderPath = $"LuuTruAnh_QLKhachSan/AnhKhachHang/Avatar/{folderName}";

                // Tạo public_id (tên file trên Cloudinary)
                var timestamp = DateTime.Now.Ticks;
                var publicId = $"{folderPath}/avatar_{timestamp}";

                using (var stream = file.InputStream)
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream),
                        PublicId = publicId,
                        Folder = folderPath,
                        Overwrite = true,
                        Transformation = new Transformation()
                            .Width(500)
                            .Height(500)
                            .Crop("fill")
                            .Gravity("face")
                            .Quality("auto")
                    };

                    var uploadResult = _cloudinary.Upload(uploadParams);

                    if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return uploadResult.SecureUrl.ToString();
                    }
                    else
                    {
                        throw new Exception($"Upload thất bại: {uploadResult.Error?.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi upload ảnh lên Cloudinary: {ex.Message}");
            }
        }

        /// <summary>
        /// Xóa ảnh cũ trên Cloudinary
        /// </summary>
        /// <param name="imageUrl">URL của ảnh cần xóa</param>
        public void DeleteImage(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                    return;

                // Extract public_id từ URL
                var uri = new Uri(imageUrl);
                var segments = uri.AbsolutePath.Split('/');
                
                // Lấy phần sau /upload/
                var uploadIndex = Array.IndexOf(segments, "upload");
                if (uploadIndex >= 0 && uploadIndex < segments.Length - 1)
                {
                    var publicIdParts = new string[segments.Length - uploadIndex - 2];
                    Array.Copy(segments, uploadIndex + 2, publicIdParts, 0, publicIdParts.Length);
                    var publicId = string.Join("/", publicIdParts);
                    
                    // Remove file extension
                    publicId = Path.GetFileNameWithoutExtension(publicId);

                    var deletionParams = new DeletionParams(publicId);
                    _cloudinary.Destroy(deletionParams);
                }
            }
            catch (Exception ex)
            {
                // Log error nhưng không throw để không ảnh hưởng flow chính
                System.Diagnostics.Debug.WriteLine($"Lỗi khi xóa ảnh: {ex.Message}");
            }
        }

        /// <summary>
        /// Upload ảnh đánh giá của khách hàng lên Cloudinary
        /// </summary>
        /// <param name="file">File ảnh từ form</param>
        /// <param name="maKhachHang">Mã khách hàng</param>
        /// <param name="hoVaTen">Họ và tên khách hàng</param>
        /// <returns>URL của ảnh đã upload</returns>
        public string UploadAnhDanhGia(HttpPostedFileBase file, int maKhachHang, string hoVaTen)
        {
            try
            {
                if (file == null || file.ContentLength == 0)
                {
                    throw new Exception("File không hợp lệ");
                }

                // Tạo folder path: LuuTruAnh_QLKhachSan/AnhKhachHang/DanhGia/{hoVaTen}_MaKhachHang-{maKhachHang}
                var folderName = $"{hoVaTen}_MaKhachHang-{maKhachHang}";
                var folderPath = $"LuuTruAnh_QLKhachSan/AnhKhachHang/DanhGia/{folderName}";

                // Tạo public_id (tên file trên Cloudinary)
                var timestamp = DateTime.Now.Ticks;
                var publicId = $"{folderPath}/danhgia_{timestamp}";

                using (var stream = file.InputStream)
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream),
                        PublicId = publicId,
                        Folder = folderPath,
                        Overwrite = false,
                        Transformation = new Transformation()
                            .Width(1200)
                            .Height(800)
                            .Crop("limit")
                            .Quality("auto")
                    };

                    var uploadResult = _cloudinary.Upload(uploadParams);

                    if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return uploadResult.SecureUrl.ToString();
                    }
                    else
                    {
                        throw new Exception($"Upload thất bại: {uploadResult.Error?.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi upload ảnh đánh giá lên Cloudinary: {ex.Message}");
            }
        }
    }
}
