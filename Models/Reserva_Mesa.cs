using System.ComponentModel.DataAnnotations.Schema;

namespace Reserva_Restaurantes.Models;

public class Reserva_Mesa
{
    public int Id { get; set; }
    
    [ForeignKey(nameof(Reservas))]
    public int ReservasFK { get; set; }
    public Reservas Reservas { get; set; }
    
    [ForeignKey(nameof(Mesas))]
    public int MesasFK { get; set; }
    public Mesas Mesas { get; set; }
}