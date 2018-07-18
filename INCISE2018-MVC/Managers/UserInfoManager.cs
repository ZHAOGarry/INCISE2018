using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.ComponentModel.DataAnnotations;

namespace INCISE2018_MVC.Managers
{
    public class UserInfoNode
    {
        public string Email { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string Institution { get; set; }
        public string Address { get; set; }
        public StudentTypes StudentType { get; set; }
    }

    public enum StudentTypes
    {
        NotStudent = 0,
        Undergraduate = 1,
        Master = 2,
        Doctor = 3
    }

    public class UserInfoManager : SqlDbConnection
    {
        public UserInfoManager() : base() { }

        public UserInfoNode GetUserInfoNode(string email)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM UserInfo WHERE Email=@p1;",conn);
                cmd.Parameters.AddWithValue("@p1", email);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    UserInfoNode node = new UserInfoNode();
                    node.Email = dr.IsDBNull(0) ? null : dr.GetString(0);
                    node.FirstName = dr.IsDBNull(1) ? null : dr.GetString(1);
                    node.LastName = dr.IsDBNull(2) ? null : dr.GetString(2);
                    node.Institution = dr.IsDBNull(3) ? null : dr.GetString(3);
                    node.Address = dr.IsDBNull(4) ? null : dr.GetString(4);
                    node.StudentType = (StudentTypes)dr.GetInt32(5);
                    node.FirstName = dr.GetString(1);
                    return node;
                }
            }
            return null;
        }

        /// <summary>
        /// 更新用户用户
        /// </summary>
        /// <param name="node">用户节点信息</param>
        /// <param name="insert">是否以新建插入方式操作</param>
        /// <returns></returns>
        public bool SetUserInfo(UserInfoNode node,bool insert)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                if(insert == true)
                {
                    cmd.CommandText = "INSERT INTO UserInfo VALUES(@p1,@p2,@p3,@p4,@p5,@p6)";
                }
                else
                {
                    cmd.CommandText = "UPDATE UserInfo SET FirstName=@p2,LastName=@p3,Institution=@p4,Address=@p5,StudentType=@p6 WHERE Email=@p1";
                }
                cmd.Parameters.AddWithValue("@p1",node.Email);
                cmd.Parameters.AddWithValue("@p2", node.FirstName);
                cmd.Parameters.AddWithValue("@p3", node.LastName);
                cmd.Parameters.AddWithValue("@p4", node.Institution);
                cmd.Parameters.AddWithValue("@p5", node.Address==null?"":node.Address);
                cmd.Parameters.AddWithValue("@p6", (int)node.StudentType);

                if(cmd.ExecuteNonQuery()==1)
                {
                    return true;
                }
            }
            return false;
        }
    }
}