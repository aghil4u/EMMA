using System;
using System.ComponentModel.DataAnnotations;

namespace EMMA
{
    public class Transaction
    {
        public enum TransactionTypes
        {
            EquipmentDescription,
            AssetDescription,
            OperationId,
            Dimensions,
            ModelNumber,
            SerialNumber
        }

        [Required]
        public int id { get; set; }
        public Equipment Equipment { get; set; }
        public TransactionTypes Type { get; set; }
        public DateTime Date { get; set; }
        public string Remarks { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }
}