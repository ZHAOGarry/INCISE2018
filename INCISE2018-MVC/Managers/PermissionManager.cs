using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace INCISE2018_MVC.Managers
{
    public class PermissionManager:SqlDbConnection
    {
        public PermissionManager() : base() { }

        public bool IsAdmin(string UserName)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                string sql = "select * from Permission where AspNetUsersName=@p1";
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@p1", UserName);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    if (dr.HasRows)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}