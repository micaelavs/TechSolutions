using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TechSolutions.Interfaces;

namespace TechSolutions.Models
{
    public class Producto : IEntity
    {
        public Producto()
        {
            Calificaciones = new List<CalificacionProducto>();
           
        }
        [Key]
        [Column(Order = 1)]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Descripcion { get; set; }
        [Required]
        public float Precio { get; set; }
    
        public CategoriaProducto CategoriaProducto { get; set; }
        
        [ForeignKey("CategoriaProducto")]
        [Column(Order = 2)]
        [Required]
        [Display(Name = "Categoría Producto")] // Aquí se agrega el Display attribute
        public int IdCategoriaProducto { get; set; }
        [Required]
        public int Stock { get; set; }
        [Required]
        //esta luego se transformara en una lista de imagenes... que las imagenes estaran en otra tabla
        public string Foto { get; set; }
        //Activo = 1 esta activo, habilitado... Activo = 0  baja
        public bool Activo { get; set; } = true;

        public IList<CalificacionProducto> Calificaciones { get; private set; }


    }
}