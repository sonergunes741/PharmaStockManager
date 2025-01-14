using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PharmaStockManager.Models.Identity;
using System.Net.Mail;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Mvc.Filters;
using PharmaStockManager.Filters;
using PharmaStockManager.Models;
using QRCoder;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using PharmaStockManager.Models.ViewModel;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using PharmaStockManager.Services;
using Microsoft.EntityFrameworkCore;

namespace StockManager.Controllers
{

    [ServiceFilter(typeof(LogFilter))]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IWebHostEnvironment _environment;
        private readonly PharmaContext _dbContext;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            IWebHostEnvironment environment, PharmaContext dbContext, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _environment = environment;
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> ContactMail(ContactViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    MimeMessage mimeMessage = new MimeMessage();
                    mimeMessage.From.Add(new MailboxAddress("Admin", _configuration["MailInfos:EmailAddress"]));
                    mimeMessage.To.Add(new MailboxAddress("Admin", "webwizardssol@gmail.com"));
                    var bodyBuilder = new BodyBuilder();
                    bodyBuilder.HtmlBody = $"Sender Name:{viewModel.Name}<br><br>Sender Email: {viewModel.Email}<br><br>Sender Message:{viewModel.Content}";
                    mimeMessage.Body = bodyBuilder.ToMessageBody();
                    mimeMessage.Subject = "Contact Mail";
                    // Kalan kısım aynı kalacak
                    using (var client = new MailKit.Net.Smtp.SmtpClient())
                    {
                        var sendermail = _configuration["MailInfos:EmailAddress"];
                        var senderkey = _configuration["MailInfos:EmailKey"];
                        var host = _configuration["MailInfos:Host"];
                        var port = int.Parse(_configuration["MailInfos:Port"]);
                        client.Connect(host, port, false);
                        client.Authenticate(sendermail, senderkey);
                        client.Send(mimeMessage);
                        client.Disconnect(true);
                    }
                    TempData["Message"] = "Your message has been sent successfully!";
                    TempData["MessageType"] = "success";
                }
                catch (Exception ex) {
                    TempData["Message"] = "There was an error sending your message. Please try again later.";
                    TempData["MessageType"] = "error";
                }
            } else
            {
                TempData["Message"] = "Please fill out the form correctly.";
                TempData["MessageType"] = "warning";
            }
            return RedirectToAction("Contact", "Home");
        }


        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(viewModel.Email);
                if (user == null)
                {
                    return View();
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var resetLink = Url.Action("ResetPassword", "Account",
                    new { token, email = viewModel.Email }, Request.Scheme);

                if (resetLink == null)
                {
                    return BadRequest();
                }

                SendPasswordResetEmail(viewModel.Email, resetLink);

                return RedirectToAction("Login", "Account");
            }
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                return RedirectToAction("ForgotPassword", "Account");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return RedirectToAction("ForgotPassword", "Account");
            }

            var model = new ResetPasswordViewModel { Email = email, Token = token };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    return RedirectToAction("ForgotPassword", "Account");
                }

                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            return View(model);
        }


        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
        private string SetResetLinkButton(string resetLink)
        {
            return $@"
                <div style='padding: 20px; text-align: center;'>
                <a href='{resetLink}' 
                               target='_blank' 
                               style='background-color: #007bff; 
                                      color: #ffffff; 
                                      padding: 12px 30px; 
                                      text-decoration: none; 
                                      border-radius: 4px; 
                                      font-family: Arial, sans-serif; 
                                      font-size: 16px; 
                                      font-weight: bold; 
                                      display: inline-block;
                                      line-height: 1.5;
                                      margin: 10px 0;
                                      border: 1px solid #0056b3;'>
                                Go to Link
                </a>
                </div>";
        }

        private void SendPasswordResetEmail(string email, string resetLink)
        {
            MimeMessage mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress("Admin", "webwizardssol@gmail.com"));
            mimeMessage.To.Add(new MailboxAddress("User", email));
            var bodyBuilder = new BodyBuilder();

            var link = SetResetLinkButton(resetLink);
            bodyBuilder.HtmlBody = $"<div style='font-family: Arial, sans-serif;'>Şifrenizi Sıfırlamak için linke tıklayınız:<br/><br/>{link}</div>"; // Değişen kısım
            mimeMessage.Body = bodyBuilder.ToMessageBody();
            mimeMessage.Subject = "Reset Password";
            // Kalan kısım aynı kalacak
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                var sendermail = _configuration["MailInfos:EmailAddress"];
                var senderkey = _configuration["MailInfos:EmailKey"];
                var host = _configuration["MailInfos:Host"];
                var port = int.Parse(_configuration["MailInfos:Port"]);
                client.Connect(host, port, false);
                client.Authenticate(sendermail, senderkey);
                client.Send(mimeMessage);
                client.Disconnect(true);
            }
        }



        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendActivationCode()
        {
            try
            {
                var appUser = await _userManager.GetUserAsync(User);
                Console.WriteLine("firstcheck");
                if (appUser != null)
                {
                    Random random = new Random();
                    int code = random.Next(100000, 1000000); // 6-digit code
                    appUser.ActivationCode = code;
                    await _userManager.UpdateAsync(appUser);

                    MimeMessage mimeMessage = new MimeMessage();
                    MailboxAddress mailboxAddressFrom = new MailboxAddress("Admin", "webwizardssol@gmail.com");
                    MailboxAddress mailboxAddressTo = new MailboxAddress("User", appUser.Email);
                    mimeMessage.From.Add(mailboxAddressFrom);
                    mimeMessage.To.Add(mailboxAddressTo);
                    var bodyBuilder = new BodyBuilder();
                    bodyBuilder.TextBody = "Kayıt işlemini gerçekleştirmek için onay kodunuz: " + code;
                    mimeMessage.Body = bodyBuilder.ToMessageBody();
                    mimeMessage.Subject = "PharmaStockManager Onay Kodu";
                    Console.WriteLine("secondcheck");
                    using (var client = new MailKit.Net.Smtp.SmtpClient())
                    {
                        var sendermail = _configuration["MailInfos:EmailAddress"];
                        var senderkey = _configuration["MailInfos:EmailKey"];
                        var host = _configuration["MailInfos:Host"];
                        var port = int.Parse(_configuration["MailInfos:Port"]);

                        client.Connect(host, port, false);
                        client.Authenticate(sendermail, senderkey);
                        client.Send(mimeMessage);
                        client.Disconnect(true);
                    }
                    Console.WriteLine("thirdcheck");
                    return Json(new { success = true, message = "Activation code sent successfully" });
                }
                else
                {
                    Console.WriteLine("foyrcheck");
                    return Json(new { success = false, message = "User not found" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                // Log the exception
                return Json(new { success = false, message = "Failed to send activation code" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {

                AppUser appUser = new AppUser()
                {
                    UserName = viewModel.Email,
                    Email = viewModel.Email,
                    RefCode = viewModel.RefCode,
                    ActivationCode = null,
                    FullName = viewModel.FullName,
                    ActiveUser = true
                };

                var result = await _userManager.CreateAsync(appUser, viewModel.Password);
                if (result.Succeeded)
                {
                    Permissions permissions = new Permissions()
                    {
                        EditStocks = false,
                        StockIn = false,
                        StockOut = false,
                        UserID = appUser.Id
                    };

                    _dbContext.Permissions.Add(permissions);
                    await _dbContext.SaveChangesAsync();

                    await _signInManager.PasswordSignInAsync(viewModel.Email, viewModel.Password, false, true);
                    return RedirectToAction("MailConfirm", "Account");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        Console.WriteLine(item.Description);
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult Verify2faCode()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Verify2faCode(string code)
        {

            if (ModelState.IsValid)
            {
                var result = await _signInManager.TwoFactorSignInAsync("Authenticator", code, false, false);

                if (result.Succeeded)
                {
                    var user = await _userManager.GetUserAsync(User);
                    var roles = await _userManager.GetRolesAsync(user);
                    if(user != null && roles != null)
                    {

                        if (roles.Contains("Admin"))
                        {
                            return RedirectToAction("Index", "AdminDashboard");
                        }
                        else if (roles.Contains("Employee"))
                        {
                            return RedirectToAction("Index", "Manager");
                        }
                        else if (roles.Contains("Customer"))
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else if (roles.Contains("SuperAdmin"))
                        {
                            return RedirectToAction("Index", "SuperAdmin");
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid code.");
                }
            }
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginViewModel.Email);

                if (user != null && user.ActiveUser)
                {
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, loginViewModel.Password, false, lockoutOnFailure: true);

                    if (result.RequiresTwoFactor)
                    {
                        ViewData["RequiresTwoFactor"] = true;
                        return View(loginViewModel); // Re-render the page with the 2FA form
                    }

                    if (result.Succeeded)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        if (roles.Contains("Admin"))
                        {
                            return RedirectToAction("Index", "AdminDashboard");
                        }
                        else if (roles.Contains("Employee"))
                        {
                            return RedirectToAction("Index", "Employee");
                        }
                        else if (roles.Contains("Customer"))
                        {
                            return RedirectToAction("Index", "UserPanel");
                        }
                        else if (roles.Contains("SuperAdmin"))
                        {
                            return RedirectToAction("Index", "SuperAdmin");
                        }

                        // Default fallback
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            ModelState.AddModelError("", "Invalid login attempt.");
            return View(loginViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> MailConfirm()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null && user.EmailConfirmed)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Email = user.Email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> MailConfirm(MailConfirmViewModel viewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null && user.ActivationCode == viewModel.ConfirmCode)
            {
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
                return RedirectToAction("Login", "Account");
            }
            else
            {
                ModelState.AddModelError("", "Onay kodu hatalı.");
            }
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult TwoFactorAuthentication()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ChangeEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangeEmail(ChangeEmailViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var passwordCheck = await _userManager.CheckPasswordAsync(user, viewModel.Password);
                if (!passwordCheck)
                {
                    ModelState.AddModelError(string.Empty, "Şifre yanlış");
                    return View(viewModel);
                }

                var duplicatecheck = await _userManager.FindByEmailAsync(viewModel.NewEmail);
                if (duplicatecheck != null)
                {
                    ModelState.AddModelError(string.Empty, "Başka bir kullanıcı bu eposta adresiyle kayıtlı");
                    return RedirectToAction("ChangeEmail", "Account");

                }

                user.UserName = viewModel.NewEmail;
                user.Email = viewModel.NewEmail;
                user.EmailConfirmed = false;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Böyle bir kullanıcı yok");
                    return View(viewModel);
                }

                Console.WriteLine("1: " + viewModel.ConfirmPassword + "\n" + "2: " + viewModel.NewPassword + "\n" + "3: " + viewModel.OldPassword);

                var result = await _userManager.ChangePasswordAsync(user, viewModel.OldPassword, viewModel.NewPassword);

                if (result.Succeeded)
                {
                    await _signInManager.RefreshSignInAsync(user);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(viewModel);
        }

        public async Task<IActionResult> AdminProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new AdminProfileViewModel
            {
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                // Diğer profil bilgileri eklenebilir
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }
            if (user.TwoFactorEnabled) {
                TempData["TwoFactorEnabled"] = "true";
                return View();
            }
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }
            var formattedKey = FormatKey(unformattedKey);
            var qrCodeUri = GenerateQrCodeUri("PharmaStockManager", user.Email, unformattedKey);

            ViewBag.AuthenticatorKey = formattedKey;
            ViewBag.QrCodeImage = GenerateQrCodeImage(qrCodeUri);

            TempData["TwoFactorEnabled"] = "false";
            return View();
        }

        private static string GenerateQrCodeUri(string issuer, string email, string unformattedKey)
        {
            return $"otpauth://totp/{issuer}:{email}?secret={unformattedKey}&issuer={issuer}&digits=6";
        }

        private static string GenerateQrCodeImage(string qrCodeUri)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(qrCodeUri, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new Base64QRCode(qrCodeData);
            var qrCodeImageBase64 = qrCode.GetGraphic(20);
            return $"data:image/png;base64,{qrCodeImageBase64}";
        }

        private static string FormatKey(string unformattedKey)
        {
            const int chunkSize = 4;
            int keyLength = unformattedKey.Length;
            var sb = new StringBuilder();
            for (int i = 0; i < keyLength; i += chunkSize)
            {
                if (i + chunkSize < keyLength)
                {
                    sb.Append(unformattedKey.Substring(i, chunkSize)).Append(" ");
                }
                else
                {
                    sb.Append(unformattedKey.Substring(i));
                }
            }
            return sb.ToString().ToLowerInvariant();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUsername([FromBody] UpdateUsernameViewModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "Kullanıcı bulunamadı" });
                }

                user.UserName = model.Username;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return Json(new { success = false, message = "Kullanıcı adı güncellenirken bir hata oluştu" });
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Bir hata oluştu" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailViewModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "Kullanıcı bulunamadı" });
                }

                user.Email = model.Email;
                user.EmailConfirmed = false; // Require re-verification
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return Json(new { success = false, message = "E-posta güncellenirken bir hata oluştu" });
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Bir hata oluştu" });
            }
        }

        public class UpdateUsernameViewModel
        {
            public string Username { get; set; }
        }

        public class UpdateEmailViewModel
        {
            public string Email { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePhone([FromBody] UpdatePhoneViewModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "Kullanıcı bulunamadı" });
                }

                // Update phone number
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    return Json(new { success = false, message = "Telefon numarası güncellenirken bir hata oluştu" });
                }

                await _userManager.UpdateAsync(user);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the exception
                return Json(new { success = false, message = "Bir hata oluştu" });
            }
        }

        public class UpdatePhoneViewModel
        {
            public string PhoneNumber { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> VerifyAuthenticatorCode(string code)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }
            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, _userManager.Options.Tokens.AuthenticatorTokenProvider, code);
            if (!isValid)
            {
                ModelState.AddModelError(string.Empty, "Invalid code.");
                return View("EnableAuthenticator");
            }
            user.TwoFactorEnabled = true;
            await _userManager.UpdateAsync(user);
            return RedirectToAction("EnableAuthenticator", "Account");
        }

        // Disable 2FA - POST
        [HttpPost]
        public async Task<IActionResult> Disable2FAConfirm()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            if (!user.TwoFactorEnabled)
            {
                TempData["Message"] = "2FA is not currently enabled.";
                TempData["MessageType"] = "Error";
                return RedirectToAction("EnableAuthenticator", "Account");
            }

            // Disable Two-Factor Authentication
            var result = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!result.Succeeded)
            {
                TempData["Message"] = "Failed to disable 2FA. Please try again.";
                TempData["MessageType"] = "Error";
                return RedirectToAction("EnableAuthenticator", "Account");
            }

            // Clear user's authenticator key if needed
            await _userManager.ResetAuthenticatorKeyAsync(user);

            TempData["Message"] = "2FA has been disabled successfully.";
            TempData["MessageType"] = "Success";
            return RedirectToAction("EnableAuthenticator", "Account");
        }

    }
}
