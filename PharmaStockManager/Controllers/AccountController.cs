using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PharmaStockManager.Models.Identity;
using System.Net.Mail;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Mvc.Filters;
using PharmaStockManager.Filters;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace StockManager.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return RedirectToAction("ForgotPasswordConfirmation");
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var resetLink = Url.Action("ResetPassword", "Account",
                    new { token, email = viewModel.Email }, Request.Scheme);

                if(resetLink == null)
                {
                    return BadRequest();
                }


                SendPasswordResetEmail(viewModel.Email, resetLink);

                return RedirectToAction("ForgotPasswordConfirmation");
            }
            return View(viewModel);
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        private static void SendPasswordResetEmail(string email, string resetLink)
        {
            MimeMessage mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress("Admin", "webwizardssol@gmail.com"));
            mimeMessage.To.Add(new MailboxAddress("User", email));

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.TextBody = $"Şifrenizi Sıfırlamak için linke tıklayınız: {resetLink}";
            mimeMessage.Body = bodyBuilder.ToMessageBody();
            mimeMessage.Subject = "Reset Password";

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("webwizardssol@gmail.com", "wrhrfryczskuksyc");
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
                return RedirectToAction("Index", "Home");
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
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("webwizardssol@gmail.com", "wrhrfryczskuksyc"); //email and app password
                client.Send(mimeMessage);
                client.Disconnect(true);
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
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid code.");
                }
            }
            return View();
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
                var user = await _userManager.FindByEmailAsync(loginViewModel.Username);

                if (user != null && user.ActiveUser)
                {
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, loginViewModel.Password, false, lockoutOnFailure: true);

                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToAction("Verify2faCode","Account");
                    }

                    if (result.Succeeded)
                    {
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
            if (user != null && user.EmailConfirmed) { 
                return RedirectToAction("Index", "Home");
            }
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
            return RedirectToAction("Login","Account");
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
                if(duplicatecheck != null)
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
                    return RedirectToAction("Manage","Account");
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

            var qrCodeUri = GenerateQrCodeUri(user.Email, unformattedKey);

            ViewBag.AuthenticatorKey = formattedKey;
            ViewBag.QrCodeUri = qrCodeUri;

            return View();
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

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            var encodedEmail = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(email));
            return $"otpauth://totp/PharmaStockManager:{encodedEmail}?secret={unformattedKey}&issuer=PharmaStockManager&digits=6";
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
