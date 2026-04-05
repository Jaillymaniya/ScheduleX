//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using ScheduleX.Infrastructure.Data;
//using System.Net;
//using System.Net.Mail;
//using ScheduleX.Web.Models.DTOs.Account;

//namespace ScheduleX.Web.Controllers.Public
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class AccountController : Controller
//    {
//        private readonly AppDbContext _context;
//        private readonly EmailService _emailService;


//        public AccountController(AppDbContext context, EmailService emailService)
//        {
//            _context = context;
//            _emailService = emailService;   
//        }

//        // ✅ SEND OTP
//        [HttpPost("send-otp")]
//        public async Task<IActionResult> SendOTP([FromBody] SendOtpRequest   model)
//        {
//            string email = model.Email;

//            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

//            if (user == null)
//                return BadRequest("Email is not registered");

//            var otp = new Random().Next(100000, 999999).ToString();

//            HttpContext.Session.SetString("OTP", otp);
//            HttpContext.Session.SetString("Email", email);
//            HttpContext.Session.SetString("OTPExpiry", DateTime.Now.AddMinutes(2).ToString());

//            await SendEmail(email, "OTP Code", $"Your OTP is {otp}");

//            return Ok("OTP sent successfully");
//        }

//        // ✅ VERIFY OTP
//        [HttpPost("verify-otp")]
//        public IActionResult VerifyOTP([FromBody] VerifyOtpRequest model)
//        {
//            string otp = model.OTP;

//            var sessionOtp = HttpContext.Session.GetString("OTP");
//            var expiryStr = HttpContext.Session.GetString("OTPExpiry");

//            if (sessionOtp == null || expiryStr == null)
//                return BadRequest("Session expired");

//            var expiry = DateTime.Parse(expiryStr);

//            if (DateTime.Now > expiry)
//                return BadRequest("OTP expired");

//            if (otp != sessionOtp)
//                return BadRequest("Invalid OTP");

//            HttpContext.Session.SetString("Verified", "true");

//            return Ok("OTP verified");
//        }

//        // ✅ RESEND OTP
//        [HttpPost("resend-otp")]
//        public async Task<IActionResult> ResendOTP()
//        {
//            var email = HttpContext.Session.GetString("Email");

//            if (string.IsNullOrEmpty(email))
//                return BadRequest("Session expired");

//            var otp = new Random().Next(100000, 999999).ToString();

//            HttpContext.Session.SetString("OTP", otp);
//            HttpContext.Session.SetString("OTPExpiry", DateTime.Now.AddMinutes(2).ToString());

//            await SendEmail(email, "New OTP", $"Your OTP is {otp}");

//            return Ok("OTP resent");
//        }

//        // ✅ RESET PASSWORD
//        [HttpPost("reset-password")]
//        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest model)
//        {
//            string newPassword = model.NewPassword;

//            var verified = HttpContext.Session.GetString("Verified");
//            var email = HttpContext.Session.GetString("Email");

//            if (verified != "true")
//                return BadRequest("OTP not verified");

//            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

//            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

//            await _context.SaveChangesAsync();

//            HttpContext.Session.Clear();

//            return Ok("Password reset successful");
//        }


//    }
//}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScheduleX.Infrastructure.Data;
using ScheduleX.Web.Models.DTOs.Account;

namespace ScheduleX.Web.Controllers.Public
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public AccountController(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // ================= SEND OTP =================
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOTP([FromBody] SendOtpRequest model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email))
                return BadRequest("Email is required");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null)
                return BadRequest("Email is not registered");

            var otp = new Random().Next(100000, 999999).ToString();

            HttpContext.Session.SetString("OTP", otp);
            HttpContext.Session.SetString("Email", model.Email);
            HttpContext.Session.SetString("OTPExpiry", DateTime.Now.AddMinutes(2).ToString());

            await _emailService.SendEmailAsync(
                model.Email,
                "OTP Code",
                $"Your OTP for password reset is: {otp}. It will expire in 2 minutes."
            );

            return Ok("OTP sent successfully");
        }

        // ================= VERIFY OTP =================
        [HttpPost("verify-otp")]
        public IActionResult VerifyOTP([FromBody] VerifyOtpRequest model)
        {
            if (model == null || string.IsNullOrEmpty(model.OTP))
                return BadRequest("OTP is required");

            var sessionOtp = HttpContext.Session.GetString("OTP");
            var expiryStr = HttpContext.Session.GetString("OTPExpiry");

            if (sessionOtp == null || expiryStr == null)
                return BadRequest("Session expired");

            var expiry = DateTime.Parse(expiryStr);

            if (DateTime.Now > expiry)
                return BadRequest("OTP expired");

            if (model.OTP != sessionOtp)
                return BadRequest("Invalid OTP");

            HttpContext.Session.SetString("Verified", "true");

            return Ok("OTP verified successfully");
        }

        // ================= RESEND OTP =================
        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOTP()
        {
            var email = HttpContext.Session.GetString("Email");

            if (string.IsNullOrEmpty(email))
                return BadRequest("Session expired");

            var otp = new Random().Next(100000, 999999).ToString();

            HttpContext.Session.SetString("OTP", otp);
            HttpContext.Session.SetString("OTPExpiry", DateTime.Now.AddMinutes(2).ToString());

            await _emailService.SendEmailAsync(
                email,
                "New OTP",
                $"Your new OTP is: {otp}. It will expire in 2 minutes."
            );

            return Ok("OTP resent successfully");
        }

        // ================= RESET PASSWORD =================
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest model)
        {
            if (model == null || string.IsNullOrEmpty(model.NewPassword))
                return BadRequest("Password is required");

            var verified = HttpContext.Session.GetString("Verified");
            var email = HttpContext.Session.GetString("Email");

            if (verified != "true")
                return BadRequest("OTP not verified");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
                return BadRequest("User not found");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

            await _context.SaveChangesAsync();

            HttpContext.Session.Clear();

            return Ok("Password reset successful");
        }
    }
}