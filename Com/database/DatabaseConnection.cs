using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;

namespace QM.Com.database
{
    /// <summary>
    /// This Class is actually responsible for database connection
    /// </summary>
    class DatabaseConnection
    {
        public static OleDbConnection GetConnection() {
            return new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=.\main.mdb");// 
            //this will do a connection via this string!
        }
    }
}
