namespace api_pns.Models.Users
{
    public class UsersModel
    {
        public int idUser { get; set; }
        public string documentNumber { get; set; }
        public string names { get; set; }
        public string surnames { get; set; }
        public string nameUser { get; set; }
        public string password { get; set; }
        public string birthDate { get; set; }
        public int idDocumentType { get; set; }
        public int idCountry { get; set; }
        public int idCity { get; set; }
        public int idRole { get; set; }
    }

    public class UserPasswordModel
    {
        public int idUser { get; set; }
        public string password { get; set; }
    }
}
