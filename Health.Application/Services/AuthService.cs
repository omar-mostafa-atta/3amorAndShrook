using Health.Application.IServices;
using Health.Application.Models;
using Health.Contracts.Requests;
using Health.Contracts.Responses.Users;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Health.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService; // For sending reset links

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, ITokenService tokenService, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            var userExists = await _userManager.FindByEmailAsync(request.Email);
            if (userExists != null)
            {
                return new AuthResponseDto { IsSuccess = false, Errors = new[] { "User with this email already exists." } };
            }

            var user = new User
            {
                Email = request.Email,
                UserName = request.Email, // Use email as username
                FirstName = request.FirstName,
                LastName = request.LastName,
                EmailConfirmed = true // For simplicity, confirm email here. In production, send a confirmation email.
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                // 1. Assign Role
                await _userManager.AddToRoleAsync(user, request.Role);

                // 2. Create the specific entity (Patient, Doctor, or Nurse)
                switch (request.Role)
                {
                    case "Patient":
                        user.Patient = new Patient();
                        break;
                    case "Doctor":
                        user.Doctor = new Doctor();
                        break;
                    case "Nurse":
                        user.Nurse = new Nurse();
                        break;
                }
                // NOTE: You'll need to save changes to the database context here to persist the associated entity.

                // 3. Generate Token and Response
                return new AuthResponseDto
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Token = await _tokenService.CreateToken(user),
                    IsSuccess = true
                };
            }

            return new AuthResponseDto
            {
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description)
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return new AuthResponseDto { IsSuccess = false, Errors = new[] { "Invalid credentials." } };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (result.Succeeded)
            {
                return new AuthResponseDto
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Token = await _tokenService.CreateToken(user),
                    IsSuccess = true
                };
            }

            return new AuthResponseDto { IsSuccess = false, Errors = new[] { "Invalid credentials." } };
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return true; // Avoid user enumeration

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // In a real application, you would generate a secure URL containing the token 
            // and the user's ID, and send it to the user's email.
            // Example link: /reset-password?userId={user.Id}&token={token}

            await _emailService.SendPasswordResetEmailAsync(user.Email, user.Id.ToString(), token);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) return false;

            // The token needs to be URL-decoded if it was sent in a query string.
            // Replace spaces with '+' for base64 encoding if needed.
            var decodedToken = request.Token;
            // Example: var decodedToken = WebEncoders.Base64UrlDecode(request.Token); 
            // or simply: var decodedToken = request.Token.Replace(' ', '+'); if it was URL-encoded.

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);

            return result.Succeeded;
        }
    }
}
