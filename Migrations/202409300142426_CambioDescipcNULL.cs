namespace TechSolutions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CambioDescipcNULL : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DetalleDevolucions", "Descripcion", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DetalleDevolucions", "Descripcion", c => c.String(nullable: false));
        }
    }
}
