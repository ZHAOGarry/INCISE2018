using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using INCISE2018_MVC.Models;
using System.Data.SqlClient;

namespace INCISE2018_MVC.Managers
{
    public class RegistrationManager : SqlDbConnection
    {
        public RegistrationManager() : base() { }

        public bool IsRegisted(string userName)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Regist WHERE AspNetUsetsName=@p1;", conn);
                cmd.Parameters.AddWithValue("@p1", userName);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    return true;
                }
            }
            return false;
        }

        public bool DoRegist(string userName)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Regist(AspNetUsetsName) VALUES(@p1);", conn);
                cmd.Parameters.AddWithValue("@p1", userName);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CancelRegist(string userName)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Regist WHERE AspNetUsetsName=@p1;", conn);
                cmd.Parameters.AddWithValue("@p1", userName);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    return true;
                }
            }
            return false;
        }

        public List<TripSignUpDateModel> GetResultList()
        {
            List<TripSignUpDateModel> list = new List<TripSignUpDateModel>();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Regist;", conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    TripSignUpDateModel model = new TripSignUpDateModel();
                    model.Mail = dr.GetString(0);
                    model.SignUpDate = dr.GetDateTime(1);
                    list.Add(model);
                }
            }
            return list;
        }
    }
}