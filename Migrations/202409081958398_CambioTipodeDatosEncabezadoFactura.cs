namespace TechSolutions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CambioTipodeDatosEncabezadoFactura : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.EncabezadoFacturas", "DNI", c => c.String(nullable: false));
            AlterColumn("dbo.EncabezadoFacturas", "Nrotarjeta", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EncabezadoFacturas", "Nrotarjeta", c => c.Int(nullable: false));
            AlterColumn("dbo.EncabezadoFacturas", "DNI", c => c.Int(nullable: false));
        }
    }
}
