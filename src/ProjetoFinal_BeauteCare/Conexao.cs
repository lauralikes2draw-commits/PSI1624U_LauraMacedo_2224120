using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoFinal
{
    public class Conexao
    {
            private static string connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=BeauteCareDB;Trusted_Connection=True;";

        public static SqlConnection Conectar()
        {
            return new SqlConnection(connectionString);
        }

        public static string GetConnectionString()
        {
            return connectionString;
        }
    
    }
}
