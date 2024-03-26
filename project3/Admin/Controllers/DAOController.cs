using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace project3.Admin.Controllers
{
    public class DAOController : Controller
    {
        private string connectionString = "Data Source=MSI;Initial Catalog=dbauctionsystem;Integrated Security=True;";


        public DataTable ExecuteQuery(string query)
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                connection.Open();
                adapter.Fill(dt);
                connection.Close();
            }
            return dt;
        }
    }
}