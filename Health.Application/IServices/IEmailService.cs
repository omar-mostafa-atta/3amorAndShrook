using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.IServices
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string toEmail, string userId, string token);
    }
}
