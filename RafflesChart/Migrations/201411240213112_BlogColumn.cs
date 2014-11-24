namespace RafflesChart.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BlogColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Blogs", "UpdatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Blogs", "Page", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Blogs", "Page");
            DropColumn("dbo.Blogs", "UpdatedDate");
        }
    }
}
