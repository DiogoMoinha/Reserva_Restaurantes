using System.ComponentModel.DataAnnotations;
namespace Reserva_Restaurantes.Models.Validações;

public class HoraFechoMaiorQueHoraAberturaAttribute: ValidationAttribute

{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var restaurante = (Restaurantes)validationContext.ObjectInstance;

        TimeSpan horaAbertura = restaurante.HoraAbertura.TimeOfDay;
        TimeSpan horaFecho = restaurante.HoraFecho.TimeOfDay;

        if (horaFecho <= horaAbertura)
        {
            return new ValidationResult("A hora de fecho deve ser posterior à hora de abertura.");
        }

        return ValidationResult.Success;
    }
}