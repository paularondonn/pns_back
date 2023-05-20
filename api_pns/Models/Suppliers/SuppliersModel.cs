namespace api_pns.Models.Suppliers
{
    public class SuppliersModel
    {
        public int idSupplier { get; set; }
        public string nit { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string telephone { get; set; }

    }

    public class SuppliersConsultModel
    {
        public int idSupplier { get; set; }
        public string nit { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string telephone { get; set; }

    }

    public class SuppliersCreateUpdateModel
    {
        public int idSupplier { get; set; }
        public string nit { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string telephone { get; set; }

    }
}