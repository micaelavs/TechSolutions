using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Hierarchy;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechSolutions.Models;

namespace TechSolutions.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext() :
           base("DefaultConnection")
        {

        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<SolicitudDevolucion> SolicitudesDevoluciones { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<HistorialPedido> HistorialPedidos { get; set; }
        public DbSet<EncabezadoFactura> EncabezadosFacturas { get; set; }
        public DbSet<DetallePedido> DetallesPedidos { get; set; }
        public DbSet<DetalleFactura> DetallesFacturas { get; set; }
        public DbSet<DetalleDevolucion> DetallesDevoluciones { get; set; }
        public DbSet<CalificacionProducto> CalificacionesProductos { get; set; }
        public DbSet<CategoriaProducto> Categorias { get; set; }

        public DbSet<NotaDeCredito> NotasCreditos { get; set; }

        public DbSet<DetalleNotaCredito> DetallesNotasCreditos { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configuración de la relación entre EncabezadoFactura y Pedido
            modelBuilder.Entity<EncabezadoFactura>()
                .HasRequired(e => e.Pedido)
                .WithMany()
                .HasForeignKey(e => e.IdPedido)
                .WillCascadeOnDelete(false); // Desactiva la eliminación en cascada

            // Configuración de la relación entre EncabezadoFactura y Usuario
            modelBuilder.Entity<EncabezadoFactura>()
                .HasRequired(e => e.Usuario)
                .WithMany()
                .HasForeignKey(e => e.IdUsuario)
                .WillCascadeOnDelete(false); // Desactiva la eliminación en cascada

            // Configuración de la relación entre Pedido y Usuario
            modelBuilder.Entity<Pedido>()
                .HasRequired(p => p.Usuario)
                .WithMany()
                .HasForeignKey(p => p.IdUsuario)
                .WillCascadeOnDelete(false); // Desactiva la eliminación en cascada

            // Configuración de la relación entre SolicitudDevolucion y Pedido
            modelBuilder.Entity<SolicitudDevolucion>()
                .HasRequired(sd => sd.Pedido)
                .WithMany()
                .HasForeignKey(sd => sd.IdPedido)
                .WillCascadeOnDelete(false); // Desactiva la eliminación en cascada

            // Configuración de la relación entre SolicitudDevolucion y EncabezadoFactura
            modelBuilder.Entity<SolicitudDevolucion>()
                .HasRequired(sd => sd.EncabezadoFactura)
                .WithMany()
                .HasForeignKey(sd => sd.IdFactura)
                .WillCascadeOnDelete(false); // Desactiva la eliminación en cascada

            // Configuración de la relación entre SolicitudDevolucion y Usuario
            modelBuilder.Entity<SolicitudDevolucion>()
                .HasRequired(sd => sd.Usuario)
                .WithMany()
                .HasForeignKey(sd => sd.IdIUsuario)
                .WillCascadeOnDelete(false); // Desactiva la eliminación en cascada

            // Configuración de la relación entre NotaDeCredito y SolicitudDevolucion
            modelBuilder.Entity<NotaDeCredito>()
                .HasRequired(nc => nc.SolicitudDevolucion)
                .WithMany()
                .HasForeignKey(nc => nc.IdSolicitudDevolucion)
                .WillCascadeOnDelete(false); // Desactiva la eliminación en cascada

            // Configuración de la relación entre NotaDeCredito y EncabezadoFactura
            modelBuilder.Entity<NotaDeCredito>()
                .HasRequired(nc => nc.EncabezadoFactura)
                .WithMany()
                .HasForeignKey(nc => nc.IdFactura)
                .WillCascadeOnDelete(false); // Desactiva la eliminación en cascada

            // Configuración de la relación entre DetalleNotaCredito y NotaDeCredito
            modelBuilder.Entity<DetalleNotaCredito>()
                .HasRequired(dnc => dnc.NotaDeCredito)
                .WithMany()
                .HasForeignKey(dnc => dnc.IdNotaCredito)
                .WillCascadeOnDelete(false); // Desactiva la eliminación en cascada

            // Configuración de la relación entre DetalleNotaCredito y Producto
            modelBuilder.Entity<DetalleNotaCredito>()
                .HasRequired(dnc => dnc.Producto)
                .WithMany()
                .HasForeignKey(dnc => dnc.IdProducto)
                .WillCascadeOnDelete(false); // Desactiva la eliminación en cascada

            base.OnModelCreating(modelBuilder);
        }
    }
}
