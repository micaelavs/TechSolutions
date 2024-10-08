namespace TechSolutions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CalificacionProductoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdProducto = c.Int(nullable: false),
                        IdUsuario = c.Int(nullable: false),
                        Puntaje = c.Int(nullable: false),
                        Comentario = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Productoes", t => t.IdProducto, cascadeDelete: true)
                .ForeignKey("dbo.Usuarios", t => t.IdUsuario, cascadeDelete: true)
                .Index(t => t.IdProducto)
                .Index(t => t.IdUsuario);
            
            CreateTable(
                "dbo.Productoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdCategoriaProducto = c.Int(nullable: false),
                        Nombre = c.String(nullable: false),
                        Descripcion = c.String(nullable: false),
                        Precio = c.Single(nullable: false),
                        Stock = c.Int(nullable: false),
                        Foto = c.String(nullable: false),
                        Activo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CategoriaProductoes", t => t.IdCategoriaProducto, cascadeDelete: true)
                .Index(t => t.IdCategoriaProducto);
            
            CreateTable(
                "dbo.CategoriaProductoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(nullable: false),
                        Descripcion = c.String(nullable: false),
                        Activo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Usuarios",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        Rol = c.Int(nullable: false),
                        Activo = c.Boolean(nullable: false),
                        Nombre = c.String(),
                        Apellido = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DetalleDevolucions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdSolicitudDevolucion = c.Int(nullable: false),
                        IdProducto = c.Int(nullable: false),
                        Cantidad = c.Int(nullable: false),
                        PrecioUnitario = c.Single(nullable: false),
                        Motivo = c.Int(nullable: false),
                        Descripcion = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Productoes", t => t.IdProducto, cascadeDelete: true)
                .ForeignKey("dbo.SolicitudDevolucions", t => t.IdSolicitudDevolucion, cascadeDelete: true)
                .Index(t => t.IdSolicitudDevolucion)
                .Index(t => t.IdProducto);
            
            CreateTable(
                "dbo.SolicitudDevolucions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdPedido = c.Int(nullable: false),
                        IdFactura = c.Int(nullable: false),
                        IdIUsuario = c.Int(nullable: false),
                        NumeroDevolucion = c.Int(nullable: false),
                        FechaOperacion = c.DateTime(nullable: false),
                        EstadoSolicitud = c.Int(nullable: false),
                        Monto = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EncabezadoFacturas", t => t.IdFactura)
                .ForeignKey("dbo.Pedidoes", t => t.IdPedido)
                .ForeignKey("dbo.Usuarios", t => t.IdIUsuario)
                .Index(t => t.IdPedido)
                .Index(t => t.IdFactura)
                .Index(t => t.IdIUsuario);
            
            CreateTable(
                "dbo.EncabezadoFacturas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdPedido = c.Int(nullable: false),
                        IdUsuario = c.Int(nullable: false),
                        Numero = c.Int(nullable: false),
                        TipoFactura = c.Int(nullable: false),
                        MedioPago = c.Int(nullable: false),
                        TipoTarjeta = c.Int(nullable: false),
                        NombreTarjeta = c.String(nullable: false),
                        ApellidoTarjeta = c.String(nullable: false),
                        DNI = c.String(nullable: false),
                        Nrotarjeta = c.String(nullable: false),
                        Cuota = c.Int(nullable: false),
                        Monto = c.Single(nullable: false),
                        Fecha = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pedidoes", t => t.IdPedido)
                .ForeignKey("dbo.Usuarios", t => t.IdUsuario)
                .Index(t => t.IdPedido)
                .Index(t => t.IdUsuario);
            
            CreateTable(
                "dbo.DetalleFacturas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdFactura = c.Int(nullable: false),
                        IdProducto = c.Int(nullable: false),
                        Cantidad = c.Int(nullable: false),
                        PrecioUnitario = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EncabezadoFacturas", t => t.IdFactura, cascadeDelete: true)
                .ForeignKey("dbo.Productoes", t => t.IdProducto, cascadeDelete: true)
                .Index(t => t.IdFactura)
                .Index(t => t.IdProducto);
            
            CreateTable(
                "dbo.Pedidoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdUsuario = c.Int(nullable: false),
                        Numero = c.Int(nullable: false),
                        Estado = c.Int(nullable: false),
                        MontoTotal = c.Single(nullable: false),
                        FechaOperacion = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Usuarios", t => t.IdUsuario)
                .Index(t => t.IdUsuario);
            
            CreateTable(
                "dbo.DetallePedidoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdProducto = c.Int(nullable: false),
                        IdPedido = c.Int(nullable: false),
                        Cantidad = c.Int(nullable: false),
                        PrecioUnitario = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pedidoes", t => t.IdPedido, cascadeDelete: true)
                .ForeignKey("dbo.Productoes", t => t.IdProducto, cascadeDelete: true)
                .Index(t => t.IdProducto)
                .Index(t => t.IdPedido);
            
            CreateTable(
                "dbo.HistorialPedidoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdPedido = c.Int(nullable: false),
                        EstadoPedido = c.Int(nullable: false),
                        FechaOperacion = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pedidoes", t => t.IdPedido, cascadeDelete: true)
                .Index(t => t.IdPedido);
            
            CreateTable(
                "dbo.DetalleNotaCreditoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdNotaCredito = c.Int(nullable: false),
                        IdProducto = c.Int(nullable: false),
                        Cantidad = c.Int(nullable: false),
                        PrecioUnitario = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.NotaDeCreditoes", t => t.IdNotaCredito)
                .ForeignKey("dbo.Productoes", t => t.IdProducto)
                .Index(t => t.IdNotaCredito)
                .Index(t => t.IdProducto);
            
            CreateTable(
                "dbo.NotaDeCreditoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdSolicitudDevolucion = c.Int(nullable: false),
                        IdFactura = c.Int(nullable: false),
                        Numero = c.Int(nullable: false),
                        Fecha_emision = c.DateTime(nullable: false),
                        Monto = c.Single(nullable: false),
                        EstadoNota = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EncabezadoFacturas", t => t.IdFactura)
                .ForeignKey("dbo.SolicitudDevolucions", t => t.IdSolicitudDevolucion)
                .Index(t => t.IdSolicitudDevolucion)
                .Index(t => t.IdFactura);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DetalleNotaCreditoes", "IdProducto", "dbo.Productoes");
            DropForeignKey("dbo.DetalleNotaCreditoes", "IdNotaCredito", "dbo.NotaDeCreditoes");
            DropForeignKey("dbo.NotaDeCreditoes", "IdSolicitudDevolucion", "dbo.SolicitudDevolucions");
            DropForeignKey("dbo.NotaDeCreditoes", "IdFactura", "dbo.EncabezadoFacturas");
            DropForeignKey("dbo.DetalleDevolucions", "IdSolicitudDevolucion", "dbo.SolicitudDevolucions");
            DropForeignKey("dbo.SolicitudDevolucions", "IdIUsuario", "dbo.Usuarios");
            DropForeignKey("dbo.SolicitudDevolucions", "IdPedido", "dbo.Pedidoes");
            DropForeignKey("dbo.SolicitudDevolucions", "IdFactura", "dbo.EncabezadoFacturas");
            DropForeignKey("dbo.EncabezadoFacturas", "IdUsuario", "dbo.Usuarios");
            DropForeignKey("dbo.EncabezadoFacturas", "IdPedido", "dbo.Pedidoes");
            DropForeignKey("dbo.Pedidoes", "IdUsuario", "dbo.Usuarios");
            DropForeignKey("dbo.HistorialPedidoes", "IdPedido", "dbo.Pedidoes");
            DropForeignKey("dbo.DetallePedidoes", "IdProducto", "dbo.Productoes");
            DropForeignKey("dbo.DetallePedidoes", "IdPedido", "dbo.Pedidoes");
            DropForeignKey("dbo.DetalleFacturas", "IdProducto", "dbo.Productoes");
            DropForeignKey("dbo.DetalleFacturas", "IdFactura", "dbo.EncabezadoFacturas");
            DropForeignKey("dbo.DetalleDevolucions", "IdProducto", "dbo.Productoes");
            DropForeignKey("dbo.CalificacionProductoes", "IdUsuario", "dbo.Usuarios");
            DropForeignKey("dbo.CalificacionProductoes", "IdProducto", "dbo.Productoes");
            DropForeignKey("dbo.Productoes", "IdCategoriaProducto", "dbo.CategoriaProductoes");
            DropIndex("dbo.NotaDeCreditoes", new[] { "IdFactura" });
            DropIndex("dbo.NotaDeCreditoes", new[] { "IdSolicitudDevolucion" });
            DropIndex("dbo.DetalleNotaCreditoes", new[] { "IdProducto" });
            DropIndex("dbo.DetalleNotaCreditoes", new[] { "IdNotaCredito" });
            DropIndex("dbo.HistorialPedidoes", new[] { "IdPedido" });
            DropIndex("dbo.DetallePedidoes", new[] { "IdPedido" });
            DropIndex("dbo.DetallePedidoes", new[] { "IdProducto" });
            DropIndex("dbo.Pedidoes", new[] { "IdUsuario" });
            DropIndex("dbo.DetalleFacturas", new[] { "IdProducto" });
            DropIndex("dbo.DetalleFacturas", new[] { "IdFactura" });
            DropIndex("dbo.EncabezadoFacturas", new[] { "IdUsuario" });
            DropIndex("dbo.EncabezadoFacturas", new[] { "IdPedido" });
            DropIndex("dbo.SolicitudDevolucions", new[] { "IdIUsuario" });
            DropIndex("dbo.SolicitudDevolucions", new[] { "IdFactura" });
            DropIndex("dbo.SolicitudDevolucions", new[] { "IdPedido" });
            DropIndex("dbo.DetalleDevolucions", new[] { "IdProducto" });
            DropIndex("dbo.DetalleDevolucions", new[] { "IdSolicitudDevolucion" });
            DropIndex("dbo.Productoes", new[] { "IdCategoriaProducto" });
            DropIndex("dbo.CalificacionProductoes", new[] { "IdUsuario" });
            DropIndex("dbo.CalificacionProductoes", new[] { "IdProducto" });
            DropTable("dbo.NotaDeCreditoes");
            DropTable("dbo.DetalleNotaCreditoes");
            DropTable("dbo.HistorialPedidoes");
            DropTable("dbo.DetallePedidoes");
            DropTable("dbo.Pedidoes");
            DropTable("dbo.DetalleFacturas");
            DropTable("dbo.EncabezadoFacturas");
            DropTable("dbo.SolicitudDevolucions");
            DropTable("dbo.DetalleDevolucions");
            DropTable("dbo.Usuarios");
            DropTable("dbo.CategoriaProductoes");
            DropTable("dbo.Productoes");
            DropTable("dbo.CalificacionProductoes");
        }
    }
}
