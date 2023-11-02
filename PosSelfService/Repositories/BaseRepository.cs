using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosSelfService.Repositories
{
    public class BaseRepository
    {

        public static string connection = "server=localhost;port=3306;pooling=false;user id=root;password=1234;database=mytestdb;";

        public MySqlConnection openConnection()
        {
            MySqlConnection sqlConnection = new MySqlConnection(connection);
            return sqlConnection;
        }
    }
}