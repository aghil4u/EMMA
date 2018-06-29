namespace EMMA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class extra : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Equipments", "LastUpdatedStockQty");
            DropColumn("dbo.Equipments", "LastUpdatedPhysicalQty");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Equipments", "LastUpdatedPhysicalQty", c => c.DateTime(nullable: false));
            AddColumn("dbo.Equipments", "LastUpdatedStockQty", c => c.DateTime(nullable: false));
        }
    }
}
