namespace Project1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Permissions", "CodeName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Permissions", "CodeName");
        }
    }
}
