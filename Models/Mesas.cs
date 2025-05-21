using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reserva_Restaurantes.Models;

public class Mesas
{
    /// <summary>
    /// Id da mesa
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Numero da Mesa
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "O número da mesa deve ser positivo.")]
    public int NumMesa { get; set; }
    
    /// <summary>
    /// Capacidade da mesa
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "O número da mesa deve ser positivo.")]
    public int Capacidade { get; set; }
    
    /* *****************************
     * Definição de Relacionamentos
     * *****************************
     */
    
    [ForeignKey(nameof(Restaurante))]
    public int RestauranteFK { get; set; }
    public Restaurantes Restaurante { get; set; }
}