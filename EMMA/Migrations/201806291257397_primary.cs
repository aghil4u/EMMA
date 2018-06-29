namespace EMMA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class primary : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Equipments",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        EquipmentNumber = c.String(),
                        Description = c.String(),
                        StockQty = c.Double(nullable: false),
                        PhysicalQty = c.Double(nullable: false),
                        LastUpdatedStockQty = c.DateTime(nullable: false),
                        LastUpdatedPhysicalQty = c.DateTime(nullable: false),
                        Location = c.String(),
                        Type = c.String(),
                        PartOf = c.String(),
                        PartNumber = c.String(),
                        Size = c.String(),
                        MinimumQuantity = c.Double(nullable: false),
                        StandardOrderQuantity = c.Double(nullable: false),
                        Suppliers = c.String(),
                        Price = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        ItemStockCode = c.String(),
                        Type = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Qty = c.Double(nullable: false),
                        Remarks = c.String(),
                        Project = c.String(),
                        ItemDescription = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Transactions");
            DropTable("dbo.Equipments");
        }
    }
}
