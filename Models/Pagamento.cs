using System.ComponentModel.DataAnnotations.Schema;

namespace Reserva_Restaurantes.Models;

public class Pagamento
{
    /// <summary>
    /// id do pagamento
    /// </summary>
    public int id { get; set; }
    
    /// <summary>
    /// metodo de pagamento
    /// </summary>
    public string metodo { get; set; }
    
    /// <summary>
    /// estado do pagamento
    /// </summary>
    public Estados estado { get; set; }

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
    
    [ForeignKey(nameof(Reserva))]
    public int ReservasFK { get; set; }
    public Reservas Reserva { get; set; }
}