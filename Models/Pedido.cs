﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechSolutions.Interfaces;
using TechSolutions.SharedKernel;


namespace TechSolutions.Models
{
    public class Pedido: IEntity
    {
        public Pedido()
        {
            DetallesPedidos = new List<DetallePedido>(); 
            HistorialPedidos = new List<HistorialPedido>(); 
        }
        [Key]
        [Column(Order = 1)]
        [Required]
        public int Id { get; set; }
        [Required]
        public int Numero { get; set; }
    
        [ForeignKey("Usuario")]
        [Column(Order = 2)]
        [Required]
        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; }
        [Required]
        public EstadoPedido Estado { get; set; }

        public float MontoTotal { get; set; }
        [Required]
        public DateTime FechaOperacion { get; set; }

        public IList<DetallePedido> DetallesPedidos { get; private set; }

        public IList<HistorialPedido> HistorialPedidos { get; private set; }
    }
}
