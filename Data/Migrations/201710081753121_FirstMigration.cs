namespace DiscordEconomy.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirstMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Actives",
                c => new
                    {
                        Url = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        Stamp = c.DateTime(nullable: false),
                        Rating = c.Double(nullable: false),
                        Value = c.Binary(),
                        FileName = c.String(),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Url)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Rating = c.Double(nullable: false),
                        Money = c.Double(nullable: false),
                        IsAdmin = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Notes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Value = c.Int(nullable: false),
                        Active_Url = c.String(maxLength: 128),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Actives", t => t.Active_Url)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .Index(t => t.Active_Url)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notes", "User_Id", "dbo.Users");
            DropForeignKey("dbo.Notes", "Active_Url", "dbo.Actives");
            DropForeignKey("dbo.Actives", "User_Id", "dbo.Users");
            DropIndex("dbo.Notes", new[] { "User_Id" });
            DropIndex("dbo.Notes", new[] { "Active_Url" });
            DropIndex("dbo.Actives", new[] { "User_Id" });
            DropTable("dbo.Notes");
            DropTable("dbo.Users");
            DropTable("dbo.Actives");
        }
    }
}
