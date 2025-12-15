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
        private readonly IEmailService _emailService; 
        private readonly WateenDbContext _dbContext;

        public AuthService(UserManager<User> userManager, 
            SignInManager<User> signInManager,
            ITokenService tokenService,
            IEmailService emailService,
            WateenDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _emailService = emailService;
            _dbContext= dbContext;
        }

        public async Task<AuthResponseDto> RegisterPatientAsync(RegisterPatientRequestDto request)
        {
            var userExists = await _userManager.FindByEmailAsync(request.Email);
            if (userExists != null)
            {
                return new AuthResponseDto { IsSuccess = false, Errors = new[] { "User with this email already exists." } };
            }

            var user = new User
            {
                Email = request.Email,
                FirstName = request.FirstName,
                UserName = request.FirstName + request.LastName,
                LastName = request.LastName,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, "Patient");

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, request.Password);

               user.Patient = new Patient();
 

                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();

               
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


        public async Task<AuthResponseDto> RegisterDoctorAsync(RegisterDoctorRequestDto request)
        {
            var userExists = await _userManager.FindByEmailAsync(request.Email);
            if (userExists != null)
            {
                return new AuthResponseDto { IsSuccess = false, Errors = new[] { "User with this email already exists." } };
            }

       
            var user = new User
            {
                Email = request.Email,
                UserName = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.DoctorPhoneNumber.ToString(), 
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
              
                const string doctorRole = "Doctor";
                await _userManager.AddToRoleAsync(user, doctorRole);

                var doctor = new Doctor
                {
                  
                    Specialization = request.Specialization,
                    LicenseNumber = request.LicenseNumber,
                    Bio = request.Bio,
                    PhoneNumber = request.DoctorPhoneNumber,
                    AvailabilitySchedule = request.AvailabilitySchedule,
                    User = user
                };

                
                _dbContext.Doctors.Add(doctor);

          
                await _dbContext.SaveChangesAsync();

                // 4. Generate Token and Response
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
            if (user == null) return true;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

          

            await _emailService.SendPasswordResetEmailAsync(user.Email, user.Id.ToString(), token);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) return false;

            var decodedToken = request.Token;
          
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);

            return result.Succeeded;
        }
    }
}
