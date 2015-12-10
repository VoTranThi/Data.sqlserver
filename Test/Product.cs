using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test
{
    public class Product
    {
        public int ID { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public Nullable<decimal> StandardCost { get; set; }
        public Nullable<decimal> ListPrice { get; set; }
        public string QuantityPerUnit { get; set; }
        public bool Discontinued { get; set; }
        public string Attachments { get; set; }
        public string Description { get; set; }
        public int SupplierID { get; set; }
        public Nullable<int> CategoryID { get; set; }
    }
}
