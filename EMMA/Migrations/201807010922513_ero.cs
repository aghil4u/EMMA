namespace EMMA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ero : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transactions", "OldValue", c => c.String());
            AddColumn("dbo.Transactions", "NewValue", c => c.String());
            AddColumn("dbo.Transactions", "Equipment_id", c => c.Int());
            CreateIndex("dbo.Transactions", "Equipment_id");
            AddForeignKey("dbo.Transactions", "Equipment_id", "dbo.Equipments", "id");
            DropColumn("dbo.Transactions", "ItemStockCode");
            DropColumn("dbo.Transactions", "Qty");
            DropColumn("dbo.Transactions", "Project");
            DropColumn("dbo.Transactions", "ItemDescription");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Transactions", "ItemDescription", c => c.String());
            AddColumn("dbo.Transactions", "Project", c => c.String());
            AddColumn("dbo.Transactions", "Qty", c => c.Double(nullable: false));
            AddColumn("dbo.Transactions", "ItemStockCode", c => c.String());
            DropForeignKey("dbo.Transactions", "Equipment_id", "dbo.Equipments");
            DropIndex("dbo.Transactions", new[] { "Equipment_id" });
            DropColumn("dbo.Transactions", "Equipment_id");
            DropColumn("dbo.Transactions", "NewValue");
            DropColumn("dbo.Transactions", "OldValue");
        }
    }
}
