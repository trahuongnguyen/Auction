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
        public int ExecuteInsert(string query)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
                connection.Close();
            }
            return rowsAffected;
        }
        public bool ExecuteDelete(string query)
        {
            bool success = false;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                success = (rowsAffected > 0);
            }
            return success;
        }
    }
}
