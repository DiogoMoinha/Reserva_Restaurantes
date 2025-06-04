using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reserva_Restaurantes.Models;

public class Pagamento
{
    /// <summary>
    /// id do pagamento
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Método de pagamento
    /// </summary>
    [Required(ErrorMessage = "O método de pagamento é obrigatório.")]
    public MetodoPagamento Metodo { get; set; }
    
    public enum MetodoPagamento
    {
        Multibanco,
        MBWay,
        Cartao
    }
    
    /// <summary>
    /// estado do pagamento
    /// </summary>
    public Estados Estado { get; set; }

    /// <summary>
    /// Estados associados a um pagamento
    /// </summary>
    public enum Estados
    {
        Pendente,
        Pago,
        Cancelado
    }
    
    /* *****************************
     * Definição de Relacionamentos
     * *****************************
     */
    [Display(Name = "Reserva")]
    [ForeignKey(nameof(Reserva))]
    [Required(ErrorMessage = "É necessário associar uma reserva.")]
    public int ReservasFK { get; set; }
    public Reservas Reserva { get; set; }
}