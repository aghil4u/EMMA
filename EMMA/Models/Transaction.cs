using System;
using System.ComponentModel.DataAnnotations;

namespace EMMA
{
    public class Transaction
    {
        public enum TransactionTypes
        {
            Addition,
            Release
        }

        [Required]
        public int id { get; set; }
        public string ItemStockCode { get; set; }
        public TransactionTypes Type { get; set; }
        public DateTime Date { get; set; }
        public double Qty { get; set; }
        public string Remarks { get; set; }
        public string Project { get; set; }
        public string ItemDescription { get; set; }
    }
}