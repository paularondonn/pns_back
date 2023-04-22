namespace api_pns.Models.Tables
{
    public class TablesModel
    {
        public int idTable { get; set; }
        public string nameCountry { get; set; }
        public string nameCity { get; set; }
        public string nameHeadquarters { get; set; }
        public string name { get; set; }
    }

    public class TablesConsultModel
    {
        public int? idTable { get; set; }
        public int? idCountry { get; set; }
        public int? idCity { get; set; }
        public int? idHeadquarters { get; set; }
        public string name { get; set; }
    }

    public class TablesCreateUpdateModel
    {
        public int idTable { get; set; }
        public int idHeadquarters { get; set; }
        public string name { get; set; }
    }
}
