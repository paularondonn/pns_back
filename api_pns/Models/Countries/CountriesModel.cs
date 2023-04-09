namespace api_pns.Models.Countries
{
    public class CountriesModel
    {
        public int idCountry { get; set; }
        public string name { get; set; }
    }

    public class CountriesCreateUpdateModel
    {
        public int idCountry { get; set; }
        public string name { get; set; }
    }
}
