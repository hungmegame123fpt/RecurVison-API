using BusinessObject.DTO.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IEmailService
    {
        Task SendMail(MailRequest mailRequest);
        Task SendOtpMail(string userEmail, string fullName, string otp);
        Task SendResetPassOtpMail(string userEmail, string fullName, string otp);
    }
}
