using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Serialization;

namespace Reserva_Restaurantes.Models;

public class Clientes
{
    /// <summary>
    /// Id do Cliente
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Nome do Cliente
    /// </summary>
    [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
    [StringLength(64)]
    public string Nome { get; set; }
    
    /// <summary>
    /// Email do Cliente
    /// </summary>
    [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")]
    [StringLength(64)]
    public string Email { get; set; }
    
    /// <summary>
    /// Telefone do Cliente
    /// </summary>
    [Display(Name = "Telemóvel")]
    [Required(ErrorMessage = "O {0} é de preenchimento obrigatório.")] 
    [RegularExpression("([+]|00)?[0-9]{6,17}", ErrorMessage = "O {0} só pode conter digitos. No mínimo 6. No máximo 17.")]
    [StringLength(17)]
    public string Telefone { get; set; }
    
    /* *****************************
     * Definição de Relacionamentos
     * *****************************
     */
    
    public ICollection<Reservas> ListaReservas { get; set; }
}