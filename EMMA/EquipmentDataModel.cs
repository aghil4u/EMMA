namespace EMMA
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class EquipmentDataModel : DbContext
    {
        public EquipmentDataModel()
            : base("name=EquipmentDataModel")
        {
        }

        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
