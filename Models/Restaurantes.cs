using System.ComponentModel.DataAnnotations;
using Reserva_Restaurantes.Models.Validações;

namespace Reserva_Restaurantes.Models;

public class Restaurantes
{
    /// <summary>
    /// identificador do restaurante
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// nome do restaurante
    /// </summary>
    [RegularExpression(@"^[A-ZÀ-Ý].*", ErrorMessage = "O Nome deve começar com letra maiúscula.")]
    [StringLength(64)]
    public string Nome { get; set; }
    
    /// <summary>
    /// endereço do restaurante
    /// </summary>
    [RegularExpression(@"^[A-ZÀ-Ý].*", ErrorMessage = "O endereço deve começar com letra maiúscula.")]
    [StringLength(64)]
    public string Endereco { get; set; }
    
    /// <summary>
    /// Codigo postal do restaurante
    /// </summary>
    [Display(Name = "Código Postal")]
    [Required(ErrorMessage = "O código postal é obrigatório.")]
    [RegularExpression(@"^\d{4}-\d{3}$", ErrorMessage = "O código postal deve ter o formato 1234-567.")]
    public string CodPostal { get; set; }
    
    /// <summary>
    /// Horario de Abertura do restaurante
    /// </summary>
    [DataType(DataType.Time)]
    public DateTime HoraAbertura { get; set; }
    
    /// <summary>
    /// Horario de Fecho do restaurante
    /// </summary>
    [DataType(DataType.Time)]
    [HoraFechoMaiorQueHoraAbertura(ErrorMessage = "A hora de fecho deve ser posterior à de abertura.")]
    public DateTime HoraFecho { get; set; }
    
    
    /// <summary>
    /// Nome do ficheiro da fotografia no disco rígido
    /// do Restaurante
    /// </summary>
    [Display(Name = "Fotografia")]
    [StringLength(2048)]
    public string? Foto { get; set; }
    
    /* *****************************
     * Definição de Relacionamentos
     * *****************************
     */
    
    public ICollection<Reservas> ListaReservas { get; set; }
    
    
    public ICollection<Mesas> ListaMesas { get; set; }
}