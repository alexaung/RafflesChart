namespace RafflesChart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGuestUser : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EventGuestUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EventId = c.Int(nullable: false),
                        Name = c.String(),
                        PhoneNumber = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.EventGuestUsers");
        }
    }
}
