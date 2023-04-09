namespace api_pns.Models.Cities
{
    public class CitiesModel
    {
        public int idCity { get; set; }
        public string name { get; set; }
        public string countryName { get; set; }

    }

    public class CitiesConsultModel
    {
        public int idCity { get; set; }
        public string name { get; set; }
        public int idCountry { get; set; }

    }

    public class CitiesCreateUpdateModel
    {
        public int idCity { get; set; }
        public string name { get; set; }
        public int idCountry { get; set; }

    }
}
