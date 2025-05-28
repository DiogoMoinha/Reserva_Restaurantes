using System.ComponentModel.DataAnnotations;

namespace Reserva_Restaurantes.Models;

public class Restaurantes
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
    /// Horario de Abertura do restaurante
    /// </summary>
    public DateTime HoraAbertura { get; set; }
    
    /// <summary>
    /// Horario de Fecho do restaurante
    /// </summary>
    public DateTime HoraFecho { get; set; }
    
    
    /// <summary>
    /// Nome do ficheiro da fotografia no disco rígido
    /// do Restaurante
    /// </summary>
    [Display(Name = "Fotografia")]
    public string? Foto { get; set; }
    
    /* *****************************
     * Definição de Relacionamentos
     * *****************************
     */
    
    public ICollection<Reservas> ListaReservas { get; set; }
}