using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMMA
{
    public class Equipment
    {
        [Required]
        public int id { get; set; }
        public string EquipmentNumber { get; set; }
        public string AssetNumber { get; set; }
        public string AcquisitionDate { get; set; }
        public bool PendingUpdate { get; set; }
        public float AcquisitionValue { get; set; }
        public string BookValue { get; set; }
        public Details Old { get { return _old; } set { _old = value; }}
        public Details New { get { return _new; } set { _new = value; } }

        private Details _new;
        private Details _old;

        public Equipment()
        {
            _new = new Details();
            _old = new Details();
        }
        public class Details
        {
            public string AssetDescription { get; set; }
            public string EquipmentDescription { get; set; }
            public string OperationId { get; set; } 
            public string SubType { get; set; }
            public string Weight { get; set; }
            public string WeightUnit { get; set; }
            public string Dimensions { get; set; }
            public string Tag { get; set; }
            public string Type { get; set; }
            public string Connection { get; set; }
            public string Length { get; set; }
            public string ModelNumber { get; set; }
            public string SerialNumber { get; set; }
            public string AssetLocation { get; set; }
            public string AssetLocationText { get; set; }
            public string EquipmentLocation { get; set; }
       


        }

      
      


    }
}
