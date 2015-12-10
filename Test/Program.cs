using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data.SqlServer;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"server=.\sqlexpress;database=DesktopProductInventory;uid=sa;pwd=123456;MultipleActiveResultSets=True";
            var db = Database.Open(connectionString);
            var products = db.Query("SELECT * FROM PRODUCTS").ToList();

            foreach (var item in products)
            {
                Console.WriteLine(item.ProductCode + "\t\t" + item.ListPrice);
            }
            Console.ReadLine();
        }
    }
}
