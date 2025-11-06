using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System;
using System.Threading.Tasks;

public class MailKitEmailService
{
    // Cấu hình máy chủ SMTP (Đây là ví dụ cho Gmail)
    private const string SmtpHost = "smtp.gmail.com";
    private const int SmtpPort = 587;

    // Đăng nhập (Dùng 'Mật khẩu ứng dụng' nếu là Gmail)
    private const string SmtpUsername = "vul59170@gmail.com";
    private const string SmtpPassword = "mubtusberdckbiwq";

    // Địa chỉ người gửi mặc định
    private const string FromEmail = "vul59170@gmail.com";
    private const string FromName = "Serene Horizon";

    public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        try
        {
            System.Diagnostics.Debug.WriteLine($"[Email Service] Khởi tạo email...");

            // 1. Khởi tạo Nội dung Email (MimeMessage)
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(FromName, FromEmail));
            email.To.Add(new MailboxAddress("", toEmail)); // Tên người nhận có thể để trống
            email.Subject = subject;

            // Đặt nội dung email ở định dạng HTML
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = htmlBody
            };

            System.Diagnostics.Debug.WriteLine($"[Email Service] Email được tạo thành công");
            System.Diagnostics.Debug.WriteLine($"[Email Service] Kết nối đến SMTP server: {SmtpHost}:{SmtpPort}");

            // 2. Gửi Email
            using (var client = new SmtpClient())
            {
                // Thêm logging cho SmtpClient (chỉ trong development)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                System.Diagnostics.Debug.WriteLine($"[Email Service] Đang kết nối...");

                // Cổng 587 yêu cầu StartTls (encryption)
                await client.ConnectAsync(SmtpHost, SmtpPort, SecureSocketOptions.StartTls);

                System.Diagnostics.Debug.WriteLine($"[Email Service] Kết nối thành công! Đang xác thực...");

                // Xác thực tài khoản (Đăng nhập vào SMTP Server)
                await client.AuthenticateAsync(SmtpUsername, SmtpPassword);

                System.Diagnostics.Debug.WriteLine($"[Email Service] Xác thực thành công! Đang gửi email...");

                // Gửi tin nhắn
                await client.SendAsync(email);

                System.Diagnostics.Debug.WriteLine($"[Email Service] Email đã được gửi thành công!");

                // Ngắt kết nối
                await client.DisconnectAsync(true);

                System.Diagnostics.Debug.WriteLine($"[Email Service] Đã ngắt kết nối SMTP");
            }

            return true;
        }
        catch (MailKit.Security.AuthenticationException authEx)
        {
            System.Diagnostics.Debug.WriteLine($"[Email Service] LỖI XÁC THỰC: {authEx.Message}");
            System.Diagnostics.Debug.WriteLine($"[Email Service] Kiểm tra lại username/password hoặc bật 'App Password' trên Gmail");
            System.Diagnostics.Debug.WriteLine($"[Email Service] Chi tiết: {authEx.StackTrace}");
            return false;
        }
        catch (MailKit.Net.Smtp.SmtpCommandException smtpEx)
        {
            System.Diagnostics.Debug.WriteLine($"[Email Service] LỖI SMTP COMMAND: {smtpEx.Message}");
            System.Diagnostics.Debug.WriteLine($"[Email Service] StatusCode: {smtpEx.StatusCode}");
            System.Diagnostics.Debug.WriteLine($"[Email Service] ErrorCode: {smtpEx.ErrorCode}");
            return false;
        }
        catch (MailKit.Net.Smtp.SmtpProtocolException smtpProtoEx)
        {
            System.Diagnostics.Debug.WriteLine($"[Email Service] LỖI SMTP PROTOCOL: {smtpProtoEx.Message}");
            return false;
        }
        catch (System.Net.Sockets.SocketException socketEx)
        {
            System.Diagnostics.Debug.WriteLine($"[Email Service] LỖI KẾT NỐI MẠNG: {socketEx.Message}");
            System.Diagnostics.Debug.WriteLine($"[Email Service] Kiểm tra kết nối internet hoặc firewall");
            return false;
        }
        catch (Exception ex)
        {
            // Ghi log lỗi chung
            System.Diagnostics.Debug.WriteLine($"[Email Service] LỖI KHÔNG XÁC ĐỊNH: {ex.GetType().Name}");
            System.Diagnostics.Debug.WriteLine($"[Email Service] Message: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"[Email Service] StackTrace: {ex.StackTrace}");

            if (ex.InnerException != null)
            {
                System.Diagnostics.Debug.WriteLine($"[Email Service] Inner Exception: {ex.InnerException.Message}");
            }

            return false;
        }
    }
}