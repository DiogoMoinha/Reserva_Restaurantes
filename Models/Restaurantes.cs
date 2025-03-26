namespace Reserva_Restaurantes.Models;

public class Restaurante
{
    /// <summary>
    /// identificador do restaurante
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// nome do restaurante
    /// </summary>
    public string Nome { get; set; }
    
    /// <summary>
    /// endereço do restaurante
    /// </summary>
    public string Endereco { get; set; }
    
    /// <summary>
    /// Capacidade total de clientes do restaurante
    /// </summary>
    public int Capacidade { get; set; }
    
    /// <summary>
    /// Horario de funcionamento do restaurante
    /// </summary>
    public DateTime Horario { get; set; }
}