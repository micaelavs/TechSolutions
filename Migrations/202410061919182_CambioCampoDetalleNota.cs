namespace TechSolutions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CambioCampoDetalleNota : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DetalleNotaCreditoes", "PrecioUnitario", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DetalleNotaCreditoes", "PrecioUnitario", c => c.Int(nullable: false));
        }
    }
}
