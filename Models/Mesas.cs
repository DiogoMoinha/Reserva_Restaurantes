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
<<<<<<< Restaurante-Foto
    [Required(ErrorMessage = "O número da mesa é obrigatório.")]
    [Range(1, 500, ErrorMessage = "O número da mesa deve ser maior que zero.")]
=======
    [Range(1, int.MaxValue, ErrorMessage = "O número da mesa deve ser positivo.")]
>>>>>>> master
    public int NumMesa { get; set; }
    
    /// <summary>
    /// Capacidade da mesa
    /// </summary>
<<<<<<< Restaurante-Foto
    [Required(ErrorMessage = "A capacidade da mesa é obrigatória.")]
    [Range(1, 20, ErrorMessage = "A mesa deve ter capacidade entre 1 e 20 pessoas.")]
=======
    [Range(1, int.MaxValue, ErrorMessage = "O número da mesa deve ser positivo.")]
>>>>>>> master
    public int Capacidade { get; set; }
    
    /* *****************************
     * Definição de Relacionamentos
     * *****************************
     */
    
    [Display(Name = "Restaurante")]
    [ForeignKey(nameof(Restaurante))]
    [Required(ErrorMessage = "É necessário indicar o restaurante.")]
    public int RestauranteFK { get; set; }
    public Restaurantes Restaurante { get; set; }
}