namespace TechSolutions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregoNotaCreditoYDetalle2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DetalleNotaCreditoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdNotaCredito = c.Int(nullable: false),
                        IdProducto = c.Int(nullable: false),
                        Cantidad = c.Int(nullable: false),
                        PrecioUnitario = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.NotaDeCreditoes", t => t.IdNotaCredito, cascadeDelete: true)
                .ForeignKey("dbo.Productoes", t => t.IdProducto, cascadeDelete: true)
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
                        Monto = c.Single(nullable: false),
                        EstadoNota = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EncabezadoFacturas", t => t.IdFactura, cascadeDelete: true)
                .ForeignKey("dbo.SolicitudDevolucions", t => t.IdSolicitudDevolucion, cascadeDelete: true)
                .Index(t => t.IdSolicitudDevolucion)
                .Index(t => t.IdFactura);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DetalleNotaCreditoes", "IdProducto", "dbo.Productoes");
            DropForeignKey("dbo.DetalleNotaCreditoes", "IdNotaCredito", "dbo.NotaDeCreditoes");
            DropForeignKey("dbo.NotaDeCreditoes", "IdSolicitudDevolucion", "dbo.SolicitudDevolucions");
            DropForeignKey("dbo.NotaDeCreditoes", "IdFactura", "dbo.EncabezadoFacturas");
            DropIndex("dbo.NotaDeCreditoes", new[] { "IdFactura" });
            DropIndex("dbo.NotaDeCreditoes", new[] { "IdSolicitudDevolucion" });
            DropIndex("dbo.DetalleNotaCreditoes", new[] { "IdProducto" });
            DropIndex("dbo.DetalleNotaCreditoes", new[] { "IdNotaCredito" });
            DropTable("dbo.NotaDeCreditoes");
            DropTable("dbo.DetalleNotaCreditoes");
        }
    }
}
