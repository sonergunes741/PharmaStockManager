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

namespace StockManager.Controllers
{
    
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

        private async Task SendActivationCode(AppUser appUser)
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

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    var sendermail = _configuration["MailInfos:EmailAddress"];
                    var senderkey = _configuration["MailInfox:EmailKey"];
                    var host = _configuration["MailInfos:Host"];
                    var port = int.Parse(_configuration["MailInfos:Port"]);
                    client.Connect(host, port, false);
                    client.Authenticate(sendermail, senderkey);
                    client.Send(mimeMessage);
                    client.Disconnect(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendActivationCode()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser != null)
            {
                await SendActivationCode(appUser);
            }
            else
            {
                Console.WriteLine("Activationcode failed");
            }
            return RedirectToAction("MailConfirm");
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
                    await SendActivationCode(appUser);
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
                    return RedirectToAction("Dashboard", "Home");
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
                    return RedirectToAction("Manage", "Account");

                }

                user.UserName = viewModel.NewEmail;
                user.Email = viewModel.NewEmail;
                user.EmailConfirmed = false;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    await SendActivationCode(user);
                    return RedirectToAction("Manage", "Account");
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
                    return RedirectToAction("Manage", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
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
            return RedirectToAction("Manage", "Account");
        }

    }
}
