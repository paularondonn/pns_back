namespace api_pns.Models.Consults
{
    public class ConsultsModel
    {
        public int idHeadquarters { get; set; }
        public string? countryName { get; set; }
        public string? cityName { get; set; }
        public string name { get; set;}
    }

    public class HeadquartersListModel
    {
        public int idHeadquarters { get; set; }
        public string name { get; set; }
    }
}
