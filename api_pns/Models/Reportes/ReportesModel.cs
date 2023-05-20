using System;

namespace api_pns.Models.Reportes
{
    public class ReportesModel
    {
        public int action { get; set; }
        public DateTime initialDate { get; set; }
        public DateTime finalDate { get; set; }
        public int idSede { get; set; }
    }
}
