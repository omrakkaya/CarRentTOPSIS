using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace RentTOPSIS.Utilities
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //Email gönderme işlemlerini buradan yapabiliriz kurs bittikten sonra kontrol et.
            return Task.CompletedTask;//Exception fırlatmamak için
        }
    }
}
