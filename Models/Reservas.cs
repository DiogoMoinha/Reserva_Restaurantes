﻿using System.ComponentModel.DataAnnotations;
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
    [Range(1, int.MaxValue, ErrorMessage = "O número da mesa deve ser positivo.")]
    public int PessoasQtd { get; set; }
    
    /* *****************************
     * Definição de Relacionamentos
     * *****************************
     */
    [Display(Name = "Cliente")]
    [ForeignKey(nameof(Cliente))]
    public int ClienteFK { get; set; }
    public Clientes Cliente { get; set; }
    
    [Display(Name = "Restaurante")]
    [ForeignKey(nameof(Restaurante))]
    public int RestauranteFK { get; set; }
    public Restaurantes Restaurante { get; set; }
}