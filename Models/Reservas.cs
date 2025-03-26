using System.ComponentModel.DataAnnotations.Schema;

namespace Reserva_Restaurantes.Models;

public class Reserva
{
    /// <summary>
    /// Id da Reserva
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Horario da reserva
    /// </summary>
    public DateTime DataHora { get; set; }
    
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
    
    [ForeignKey(nameof(Restaurantes))]
    public int RestauranteFK { get; set; }
    public Restaurantes Restaurante { get; set; }
    
    
}