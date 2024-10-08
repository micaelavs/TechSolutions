namespace TechSolutions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotaCredito : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DetalleNotaCreditoes", "IdNotaCredito", "dbo.NotaDeCreditoes");
            DropForeignKey("dbo.DetalleNotaCreditoes", "IdProducto", "dbo.Productoes");
            DropForeignKey("dbo.NotaDeCreditoes", "IdFactura", "dbo.EncabezadoFacturas");
            DropForeignKey("dbo.NotaDeCreditoes", "IdSolicitudDevolucion", "dbo.SolicitudDevolucions");
            AddForeignKey("dbo.DetalleNotaCreditoes", "IdNotaCredito", "dbo.NotaDeCreditoes", "Id");
            AddForeignKey("dbo.DetalleNotaCreditoes", "IdProducto", "dbo.Productoes", "Id");
            AddForeignKey("dbo.NotaDeCreditoes", "IdFactura", "dbo.EncabezadoFacturas", "Id");
            AddForeignKey("dbo.NotaDeCreditoes", "IdSolicitudDevolucion", "dbo.SolicitudDevolucions", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.NotaDeCreditoes", "IdSolicitudDevolucion", "dbo.SolicitudDevolucions");
            DropForeignKey("dbo.NotaDeCreditoes", "IdFactura", "dbo.EncabezadoFacturas");
            DropForeignKey("dbo.DetalleNotaCreditoes", "IdProducto", "dbo.Productoes");
            DropForeignKey("dbo.DetalleNotaCreditoes", "IdNotaCredito", "dbo.NotaDeCreditoes");
            AddForeignKey("dbo.NotaDeCreditoes", "IdSolicitudDevolucion", "dbo.SolicitudDevolucions", "Id", cascadeDelete: true);
            AddForeignKey("dbo.NotaDeCreditoes", "IdFactura", "dbo.EncabezadoFacturas", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DetalleNotaCreditoes", "IdProducto", "dbo.Productoes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.DetalleNotaCreditoes", "IdNotaCredito", "dbo.NotaDeCreditoes", "Id", cascadeDelete: true);
        }
    }
}
