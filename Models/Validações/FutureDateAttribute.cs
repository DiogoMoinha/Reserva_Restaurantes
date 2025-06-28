using System.ComponentModel.DataAnnotations;

namespace Reserva_Restaurantes.Models.Validações
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                if (date.Date < DateTime.Now.Date)
                {
                    return new ValidationResult(ErrorMessage ?? "A data deve ser no futuro.");
                }
            }

            return ValidationResult.Success;
        }
    }
}