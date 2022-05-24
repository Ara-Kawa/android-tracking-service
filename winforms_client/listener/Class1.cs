using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
//using Microsoft.Reporting.WinForms;

namespace listener
{
    public class Class1
    {
     
     
        public static string myConnectionString = "Data Source=(local);Initial Catalog=location;User ID=sa;Password=sa123; MultipleActiveResultSets=True";
        public static string client_ip; // This is the the android client IP Address

        public static SqlConnection myCon = new SqlConnection(myConnectionString);
        public static void ConnectSQLServer()
        {
            if (myCon.State == 0) { myCon.Open(); }
        }

        public static void DisConnectSQLServer() { if (myCon.State != 0) { myCon.Close(); } }

     

    }
}