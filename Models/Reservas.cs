using System.ComponentModel.DataAnnotations.Schema;

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
    public DateTime Data { get; set; }
    
    /// <summary>
    /// Hora da reserva
    /// </summary>
    public DateTime Hora { get; set; }
    
    /// <summary>
    /// Quantidade de Pessoas esperadas para a reserva
    /// </summary>
    public int PessoasQtd { get; set; }
    
    /* *****************************
     * Definição de Relacionamentos
     * *****************************
     */
    
    [ForeignKey(nameof(Clientes))]
    public int ClienteFK { get; set; }
    public Clientes Cliente { get; set; }
}