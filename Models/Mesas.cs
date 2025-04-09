using System.ComponentModel.DataAnnotations.Schema;

namespace Reserva_Restaurantes.Models;

public class Mesas
{
    public int Id { get; set; }
    
    public int NumMesa { get; set; }
    
    public int Capacidade { get; set; }
    
    /* *****************************
     * Definição de Relacionamentos
     * *****************************
     */
    
    [ForeignKey(nameof(Restaurantes))]
    public int RestauranteFK { get; set; }
    public Restaurantes Restaurante { get; set; }
}