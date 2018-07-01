namespace EMMA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class erd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Equipments", "AssetNumber", c => c.String());
            AddColumn("dbo.Equipments", "AcquisitionDate", c => c.String());
            AddColumn("dbo.Equipments", "PendingUpdate", c => c.Boolean(nullable: false));
            AddColumn("dbo.Equipments", "AcquisitionValue", c => c.String());
            AddColumn("dbo.Equipments", "BookValue", c => c.String());
            AddColumn("dbo.Equipments", "Old_AssetDescription", c => c.String());
            AddColumn("dbo.Equipments", "Old_EquipmentDescription", c => c.String());
            AddColumn("dbo.Equipments", "Old_OperationId", c => c.String());
            AddColumn("dbo.Equipments", "Old_SubType", c => c.String());
            AddColumn("dbo.Equipments", "Old_Weight", c => c.String());
            AddColumn("dbo.Equipments", "Old_WeightUnit", c => c.String());
            AddColumn("dbo.Equipments", "Old_Dimensions", c => c.String());
            AddColumn("dbo.Equipments", "Old_Tag", c => c.String());
            AddColumn("dbo.Equipments", "Old_Type", c => c.String());
            AddColumn("dbo.Equipments", "Old_Connection", c => c.String());
            AddColumn("dbo.Equipments", "Old_Length", c => c.String());
            AddColumn("dbo.Equipments", "Old_ModelNumber", c => c.String());
            AddColumn("dbo.Equipments", "Old_SerialNumber", c => c.String());
            AddColumn("dbo.Equipments", "Old_AssetLocation", c => c.String());
            AddColumn("dbo.Equipments", "Old_AssetLocationText", c => c.String());
            AddColumn("dbo.Equipments", "Old_EquipmentLocation", c => c.String());
            AddColumn("dbo.Equipments", "New_AssetDescription", c => c.String());
            AddColumn("dbo.Equipments", "New_EquipmentDescription", c => c.String());
            AddColumn("dbo.Equipments", "New_OperationId", c => c.String());
            AddColumn("dbo.Equipments", "New_SubType", c => c.String());
            AddColumn("dbo.Equipments", "New_Weight", c => c.String());
            AddColumn("dbo.Equipments", "New_WeightUnit", c => c.String());
            AddColumn("dbo.Equipments", "New_Dimensions", c => c.String());
            AddColumn("dbo.Equipments", "New_Tag", c => c.String());
            AddColumn("dbo.Equipments", "New_Type", c => c.String());
            AddColumn("dbo.Equipments", "New_Connection", c => c.String());
            AddColumn("dbo.Equipments", "New_Length", c => c.String());
            AddColumn("dbo.Equipments", "New_ModelNumber", c => c.String());
            AddColumn("dbo.Equipments", "New_SerialNumber", c => c.String());
            AddColumn("dbo.Equipments", "New_AssetLocation", c => c.String());
            AddColumn("dbo.Equipments", "New_AssetLocationText", c => c.String());
            AddColumn("dbo.Equipments", "New_EquipmentLocation", c => c.String());
            DropColumn("dbo.Equipments", "Description");
            DropColumn("dbo.Equipments", "StockQty");
            DropColumn("dbo.Equipments", "PhysicalQty");
            DropColumn("dbo.Equipments", "Location");
            DropColumn("dbo.Equipments", "Type");
            DropColumn("dbo.Equipments", "PartOf");
            DropColumn("dbo.Equipments", "PartNumber");
            DropColumn("dbo.Equipments", "Size");
            DropColumn("dbo.Equipments", "MinimumQuantity");
            DropColumn("dbo.Equipments", "StandardOrderQuantity");
            DropColumn("dbo.Equipments", "Suppliers");
            DropColumn("dbo.Equipments", "Price");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Equipments", "Price", c => c.String());
            AddColumn("dbo.Equipments", "Suppliers", c => c.String());
            AddColumn("dbo.Equipments", "StandardOrderQuantity", c => c.Double(nullable: false));
            AddColumn("dbo.Equipments", "MinimumQuantity", c => c.Double(nullable: false));
            AddColumn("dbo.Equipments", "Size", c => c.String());
            AddColumn("dbo.Equipments", "PartNumber", c => c.String());
            AddColumn("dbo.Equipments", "PartOf", c => c.String());
            AddColumn("dbo.Equipments", "Type", c => c.String());
            AddColumn("dbo.Equipments", "Location", c => c.String());
            AddColumn("dbo.Equipments", "PhysicalQty", c => c.Double(nullable: false));
            AddColumn("dbo.Equipments", "StockQty", c => c.Double(nullable: false));
            AddColumn("dbo.Equipments", "Description", c => c.String());
            DropColumn("dbo.Equipments", "New_EquipmentLocation");
            DropColumn("dbo.Equipments", "New_AssetLocationText");
            DropColumn("dbo.Equipments", "New_AssetLocation");
            DropColumn("dbo.Equipments", "New_SerialNumber");
            DropColumn("dbo.Equipments", "New_ModelNumber");
            DropColumn("dbo.Equipments", "New_Length");
            DropColumn("dbo.Equipments", "New_Connection");
            DropColumn("dbo.Equipments", "New_Type");
            DropColumn("dbo.Equipments", "New_Tag");
            DropColumn("dbo.Equipments", "New_Dimensions");
            DropColumn("dbo.Equipments", "New_WeightUnit");
            DropColumn("dbo.Equipments", "New_Weight");
            DropColumn("dbo.Equipments", "New_SubType");
            DropColumn("dbo.Equipments", "New_OperationId");
            DropColumn("dbo.Equipments", "New_EquipmentDescription");
            DropColumn("dbo.Equipments", "New_AssetDescription");
            DropColumn("dbo.Equipments", "Old_EquipmentLocation");
            DropColumn("dbo.Equipments", "Old_AssetLocationText");
            DropColumn("dbo.Equipments", "Old_AssetLocation");
            DropColumn("dbo.Equipments", "Old_SerialNumber");
            DropColumn("dbo.Equipments", "Old_ModelNumber");
            DropColumn("dbo.Equipments", "Old_Length");
            DropColumn("dbo.Equipments", "Old_Connection");
            DropColumn("dbo.Equipments", "Old_Type");
            DropColumn("dbo.Equipments", "Old_Tag");
            DropColumn("dbo.Equipments", "Old_Dimensions");
            DropColumn("dbo.Equipments", "Old_WeightUnit");
            DropColumn("dbo.Equipments", "Old_Weight");
            DropColumn("dbo.Equipments", "Old_SubType");
            DropColumn("dbo.Equipments", "Old_OperationId");
            DropColumn("dbo.Equipments", "Old_EquipmentDescription");
            DropColumn("dbo.Equipments", "Old_AssetDescription");
            DropColumn("dbo.Equipments", "BookValue");
            DropColumn("dbo.Equipments", "AcquisitionValue");
            DropColumn("dbo.Equipments", "PendingUpdate");
            DropColumn("dbo.Equipments", "AcquisitionDate");
            DropColumn("dbo.Equipments", "AssetNumber");
        }
    }
}
