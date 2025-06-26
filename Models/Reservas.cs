using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Reserva_Restaurantes.Models.Validações;

namespace Reserva_Restaurantes.Models;

public class Reservas
{
    /// <summary>
    /// Id da Reserva
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Data da reserva
    /// </summary>
    [Required(ErrorMessage = "A data é obrigatória.")]
    [DataType(DataType.Date)]
    [FutureDate(ErrorMessage = "A data da reserva tem de ser no futuro.")]
    public DateTime Data { get; set; }
    
    /// <summary>
    /// Hora da reserva
    /// </summary>
    [Required(ErrorMessage = "A hora é obrigatória.")]
    [DataType(DataType.Time)]
    public DateTime Hora { get; set; }
    
    /// <summary>
    /// Quantidade de Pessoas esperadas para a reserva
    /// </summary>
    [Required(ErrorMessage = "O número de pessoas é obrigatório.")]
    [Range(1, 20, ErrorMessage = "A reserva deve ser para entre 1 e 20 pessoas.")]
    public int PessoasQtd { get; set; }
    
    /* *****************************
     * Definição de Relacionamentos
     * *****************************
     */
    [Required]
    [Display(Name = "Cliente")]
    [ForeignKey(nameof(Cliente))]
    public int ClienteFK { get; set; }
    public Clientes Cliente { get; set; }
    
    [Required]
    [Display(Name = "Restaurante")]
    [ForeignKey(nameof(Restaurante))]
    public int RestauranteFK { get; set; }
    public Restaurantes Restaurante { get; set; }
}