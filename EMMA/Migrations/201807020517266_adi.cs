namespace EMMA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adi : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Equipments", "AcquisitionDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Equipments", "AcquisitionDate", c => c.String());
        }
    }
}
