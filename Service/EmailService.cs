using BusinessObject.DTO.Email;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using Service.Interface;
namespace Service
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings emailSettings;

        public EmailService(IOptions<EmailSettings> options)
        {
            this.emailSettings = options.Value;
        }

        public async Task SendMail(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(emailSettings.Email);
            email.To.Add(MailboxAddress.Parse(mailRequest.Email));
            email.Subject = mailRequest.Subject;
            var builder = new BodyBuilder();
            builder.HtmlBody = mailRequest.Emailbody;
            email.Body = builder.ToMessageBody();

            using var smptp = new SmtpClient();

            smptp.Connect(emailSettings.Host, emailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            smptp.Authenticate(emailSettings.Email, emailSettings.Password);
            await smptp.SendAsync(email);
            smptp.Disconnect(true);
        }
        private string GenerateEmailBody(string fullName, string otp)
        {
            return $@"
                <div style='font-family:Arial;'>
                    <h3>Hello {fullName},</h3>
                    <p>Thank you for registering an account. Your OTP code is:<strong>{otp}</strong></p>
                    <p>Please use this code to activate your account. The code will expire after 5 minutes.</p>
                </div>";
        }
        public async Task SendOtpMail(string userEmail, string fullName, string otp)
        {
            var mailRequest = new MailRequest
            {
                Email = userEmail,
                Subject = "Thanks for registering : OTP",
                Emailbody = GenerateEmailBody(fullName, otp)
            };

            await SendMail(mailRequest);
        }
        private string GenerateResetPassEmailBody(string fullName, string otp)
        {
            return $@"
                <div style='font-family:Arial;'>
                    <h3>Hello {fullName},</h3>
                    <p>Please confirm to reset your password. Your OTP code is:<strong>{otp}</strong></p>
                    <p>The code will expire after 5 minutes.</p>
                </div>";
        }
        public async Task SendResetPassOtpMail(string userEmail, string fullName, string otp)
        {
            var mailRequest = new MailRequest
            {
                Email = userEmail,
                Subject = "Reset password : OTP",
                Emailbody = GenerateResetPassEmailBody(fullName, otp)
            };

            await SendMail(mailRequest);
        }
    }
}
