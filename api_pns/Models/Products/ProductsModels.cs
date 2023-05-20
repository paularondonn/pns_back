namespace api_pns.Models.Products
{
    public class ProductsModel
    {
        public int idProduct { get; set; }
        public string name { get; set; }
        public string Namesupplier { get; set; }
        public string price { get; set; }
        public string amount { get; set; }

    }

    public class ProductsConsultModel
    {
        public int idProduct { get; set; }
        public string name { get; set; }
        public string price { get; set; }
        public string amount { get; set; }
        public int idSupplier { get; set; }

    }

    public class ProductsCreateUpdateModel
    {
        public int idProduct { get; set; }
        public string name { get; set; }
        public string price { get; set; }
        public string amount { get; set; }
        public int idSupplier { get; set; }

    }
}