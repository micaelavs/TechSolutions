namespace TechSolutions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UsuarioCalificacion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CalificacionProductoes", "IdUsuario", c => c.Int(nullable: false));
            CreateIndex("dbo.CalificacionProductoes", "IdUsuario");
            AddForeignKey("dbo.CalificacionProductoes", "IdUsuario", "dbo.Usuarios", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CalificacionProductoes", "IdUsuario", "dbo.Usuarios");
            DropIndex("dbo.CalificacionProductoes", new[] { "IdUsuario" });
            DropColumn("dbo.CalificacionProductoes", "IdUsuario");
        }
    }
}
