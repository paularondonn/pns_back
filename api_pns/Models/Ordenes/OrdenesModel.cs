using System;

namespace api_pns.Models.Ordenes
{
    public class OrdenesModel
    {
        public int action { get; set; }
        public int idTakeOrder { get; set; }
        public DateTime? date { get; set; }
        public string totalValue { get; set; }
        public int idTable { get; set; }
        public int? idUser { get; set; }
        public bool? paid { get; set; }
    }

    public class OrdenesProductosModel
    {
        public int idOrderProduct { get; set; }
        public int idTakeOrder { get; set; }
        public int idProduct { get; set; }
        public string amount { get; set; }
    }
}
