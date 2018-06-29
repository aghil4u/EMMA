﻿using System;
using System.ComponentModel.DataAnnotations;

namespace EMMA
{
    public class Equipment
    {
        [Required]
        public int id { get; set; }
        public string EquipmentNumber { get; set; }
        public string Description { get; set; }
        public double StockQty { get; set; }
        public double PhysicalQty { get; set; }
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