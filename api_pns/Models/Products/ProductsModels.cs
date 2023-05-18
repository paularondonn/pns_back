namespace api_pns.Models.Products
{
    public class ProductsModel
    {
        public int idProduct { get; set; }
        public string name { get; set; }
        public string supplierName { get; set; }

    }

    public class ProductsConsultModel
    {
        public int idProduct { get; set; }
        public string name { get; set; }
        public int idSuppliers { get; set; }

    }

    public class ProductsCreateUpdateModel
    {
        public int idProduct { get; set; }
        public string name { get; set; }
        public int price { get; set; }
        public int amount { get; set; }
        public int idSuppliers { get; set; }

    }
}