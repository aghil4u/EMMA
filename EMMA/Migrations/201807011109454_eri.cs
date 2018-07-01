namespace EMMA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class eri : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Equipments", "AcquisitionValue", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Equipments", "AcquisitionValue", c => c.String());
        }
    }
}
