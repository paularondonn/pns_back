using System;
using System.Collections.Generic;
using System.Text;

namespace api_pns.Models.Login
{
    public class LoginModel
    {
        public string name_user { get; set; }
        public string password { get; set; }
        public int id_headquarters { get; set; }
    }

    public class LoginValidUserNameModel
    {
        public int idSities { get; set; }
        public int idHeadquarters { get; set; }
        public string name { get; set; }
    }

    public class DetailsUserLoginReplyModel
    {
        public int idUser { get; set; }
        public int idRole { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public int idHeadquarters { get; set; }
    }
}
