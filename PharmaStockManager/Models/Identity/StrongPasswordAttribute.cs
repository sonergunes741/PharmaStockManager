using System.ComponentModel.DataAnnotations;

namespace PharmaStockManager.Models.Identity
{
    public class StrongPasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var password = value as string;

            if (password == null)
                return new ValidationResult("Şifre Gerekli");

            if (password.Length < 8)
                return new ValidationResult("Şifre en az 8 karakterli olmalıdır.");

            if (!password.Any(char.IsLower))
                return new ValidationResult("Şifre en az bir küçük harf içermelidir.");

            if (!password.Any(char.IsUpper))
                return new ValidationResult("Şifre en az bir büyük harf içermelidir.");

            if (!password.Any(char.IsDigit))
                return new ValidationResult("Şifre en az bir sayı içermelidir.");

            if (!password.Any(c => !char.IsLetterOrDigit(c)))
                return new ValidationResult("Şifre en az bir sembol içermelidir.");

            return ValidationResult.Success;
        }
    }
}