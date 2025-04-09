using System.ComponentModel.DataAnnotations.Schema;

namespace Reserva_Restaurantes.Models;

public class Pagamento
{
    public int id { get; set; }
    
    
    /// <summary>
    /// metodo de pagamento
    /// </summary>
    public string metodo { get; set; }
    
    /// <summary>
    /// estado do pagamento
    /// </summary>
    public string estado { get; set; }
    
    /* *****************************
     * Definição de Relacionamentos
     * *****************************
     */
    
    [ForeignKey(nameof(Reservas))]
    public int ReservasFK { get; set; }
    public Reservas Reserva { get; set; }
}