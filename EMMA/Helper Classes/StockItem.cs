using System;

namespace EMMA
{

    public class StockItem
    {
        public string StockCode { get; set; }

        public string FullDescription { get; set; }

        public string ShortDescription { get; set; }

        public double StockQty { get; set; }

        public double PhysicalQty { get; set; }

        public DateTime LastUpdatedStockQty { get; set; }

        public DateTime LastUpdatedPhysicalQty { get; set; }

        public string Location { get; set; }

        public string Type { get; set; }

        public string PartOf { get; set; }

        public string PartNumber { get; set; }

        public string Size { get; set; }

        public double MinimumQuantity { get; set; }

        public double StandardOrderQuantity { get; set; }

        public string Suppliers { get; set; }

        public string Price { get; set; }
    }
}