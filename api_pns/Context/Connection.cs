using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace api_pns.Context
{
    public class Connection
    {
        private static IConfiguration configuration;
        public static SqlConnection objConexion;
        private static string error;
        private static string conecction;
        public static Queue<string> conecctionName = new Queue<string>();

        public SqlConnection ConnectBD(IConfiguration configuration, string name = null)
        {
            name = "bdconnection";
            if (name != null) conecction = name;
            return new SqlConnection(configuration.GetConnectionString(conecction));
        }

        public static void SetConnection(string name)
        {
            conecction = name;
        }

        public string NameConnectBD()
        {
            return conecction;
        }
    }
}
