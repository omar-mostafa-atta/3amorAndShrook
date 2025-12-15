using Health.Application.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.Services
{
    public class EmailService : IEmailService
    {
        // msh email service 72i2ia
        public Task SendPasswordResetEmailAsync(string toEmail, string userId, string token)
        {
       
            Console.WriteLine($"\n EMAIL MOCK");
            Console.WriteLine($"To: {toEmail}");
            Console.WriteLine($"Subject: Password Reset Request");
            Console.WriteLine($"Reset Link/Code for {userId}: {token}");
            Console.WriteLine($"--------------------\n");

            return Task.CompletedTask;
        }
    }
}
