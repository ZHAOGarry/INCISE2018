using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using INCISE2018_MVC.Models;
using System.Data.SqlClient;

namespace INCISE2018_MVC.Managers
{
    public class AbstractManager : SqlDbConnection
    {
        public AbstractManager() : base() { }

        private AbstractModel FillAbstractNode(ref SqlDataReader dr)
        {
            AbstractModel model = new AbstractModel();
            model.Id = dr.GetInt32(0);
            model.GroupId = dr.GetInt32(1);
            model.Statement = (State)dr.GetInt32(2);
            model.EditTime = dr.GetDateTime(3);
            model.Author = dr.GetString(4);
            model.CoAuthors = dr.IsDBNull(5) ? null : dr.GetString(5);
            model.AbstarctTitle = dr.GetString(6);
            model.AbstarctBody = dr.GetString(7);
            model.INCISETheme = dr.GetString(8);
            model.PresentationType = dr.GetString(9);
            model.KeyWords = dr.GetString(10);
            model.Owner = dr.GetString(11);
            return model;
        }

        public List<AbstractModel> GetListOnShow()
        {
            List<AbstractModel> list = new List<AbstractModel>();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = "select t2.*,t3.UserName from" +
                    "(select max(Id) as Id from AbstractNodes group by GroupId) as t1 " +
                    "left join AbstractNodes as t2 on t1.Id = t2.Id " +
                    "left join AbstractGroups as t3 on t2.GroupId=t3.GroupId " +
                    "where t2.[Statement]=2;";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(FillAbstractNode(ref dr));
                }
            }
            return list;
        }

        public AbstractModel GetItemById(int id, string userName)
        {
            string sql = "";
            // 当前用户为管理员时
            PermissionManager permission = new PermissionManager();
            if (permission.IsAdmin(userName))
            {
                sql = "select t2.*,t3.UserName from " +
                    "(select max(Id) as Id from AbstractNodes group by GroupId) as t1 " +
                    "left join AbstractNodes as t2 on t1.Id = t2.Id " +
                    "left join AbstractGroups as t3 on t2.GroupId = t3.GroupId " +
                    "where t2.Id=@p1 ;";
            }
            else
            {
                sql = "select t2.*,t3.UserName from " +
                    "(select max(Id) as Id from AbstractNodes group by GroupId) as t1 " +
                    "left join AbstractNodes as t2 on t1.Id = t2.Id " +
                    "left join AbstractGroups as t3 on t2.GroupId = t3.GroupId " +
                    "where t2.[Statement]=2 and t2.Id=@p1 " +
                    "union select t4.*,t5.UserName from AbstractNodes as t4 " +
                    "left join AbstractGroups as t5 on t4.GroupId=t5.GroupId " +
                    "where t5.UserName = @p2 and t4.Id=@p1;";
            }

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@p1", id);
                cmd.Parameters.AddWithValue("@p2", userName);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    return FillAbstractNode(ref dr);
                }
            }
            return null;
        }

        public List<AbstractModel> GetMyList(string userName)
        {
            List<AbstractModel> list = new List<AbstractModel>();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = "select t2.*,t3.UserName from AbstractNodes as t2 " +
                    "left join AbstractGroups as t3 on t2.GroupId=t3.GroupId " +
                    "where t3.UserName=@p1 " +
                    "order by GroupId desc,Id desc;";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@p1", userName);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(FillAbstractNode(ref dr));
                }
            }
            return list;
        }

        public bool Withdraw(int Id)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                // SQL 事务
                SqlCommand cmd = conn.CreateCommand();
                SqlTransaction transaction = conn.BeginTransaction();
                cmd.Transaction = transaction;
                cmd.CommandType = System.Data.CommandType.Text;
                try
                {
                    cmd.CommandText = "UPDATE AbstractNodes SET [Statement]=4 WHERE Id=@p1;";
                    cmd.Parameters.AddWithValue("@p1", Id);
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "UPDATE AbstractGroups SET ShowNode=null WHERE ShowNode=@p2";
                    cmd.Parameters.AddWithValue("@p2", Id);
                    cmd.ExecuteNonQuery();

                    transaction.Commit();
                    return true;
                }
                catch (SqlException e1)
                {
                    try
                    {
                        transaction.Rollback();
                        return false;
                    }
                    catch (Exception e2) { return false; }
                }
            }
        }

        public bool NewAbstract(AbstractModel model, string userName)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                SqlTransaction transaction = conn.BeginTransaction();
                cmd.Transaction = transaction;
                cmd.CommandType = System.Data.CommandType.Text;
                try
                {
                    cmd.CommandText = "INSERT INTO AbstractGroups([UserName]) VALUES(@pn1);";
                    cmd.Parameters.AddWithValue("@pn1", userName); ;
                    cmd.ExecuteNonQuery();

                    int groupId;
                    cmd.CommandText = "SELECT top 1 GroupId FROM AbstractGroups WHERE UserName=@pn2 ORDER BY GroupId desc;";
                    cmd.Parameters.AddWithValue("@pn2", userName);
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        groupId = dr.GetInt32(0);
                        dr.Close();
                    }
                    else
                    {
                        dr.Close();
                        transaction.Rollback();
                        return false;
                    }

                    cmd.CommandText = "INSERT INTO AbstractNodes" +
                        "([GroupId],[Author],[CoAuthors],[Body],[KeyWords],[PreType],[Theme],[Title]) VALUES" +
                        "(@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8);";
                    cmd.Parameters.AddWithValue("@p1", groupId);
                    cmd.Parameters.AddWithValue("@p2", model.Author);
                    cmd.Parameters.AddWithValue("@p3", model.CoAuthors == null ? "" : model.CoAuthors);
                    cmd.Parameters.AddWithValue("@p4", model.AbstarctBody);
                    cmd.Parameters.AddWithValue("@p5", model.KeyWords);
                    cmd.Parameters.AddWithValue("@p6", model.PresentationType);
                    cmd.Parameters.AddWithValue("@p7", model.INCISETheme);
                    cmd.Parameters.AddWithValue("@p8", model.AbstarctTitle);
                    cmd.ExecuteNonQuery();

                    transaction.Commit();
                    return true;
                }
                catch (SqlException e1)
                {
                    try
                    {
                        transaction.Rollback();
                        return false;
                    }
                    catch (Exception e2) { return false; }
                }
            }
        }

        public bool Submit(int Id)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = "UPDATE AbstractNodes SET [Statement]=1 WHERE Id=@p1 and Statement=0";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@p1", Id);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool Edit(AbstractModel model)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = "INSERT INTO AbstractNodes" +
                        "([GroupId],[Author],[CoAuthors],[Body],[KeyWords],[PreType],[Theme],[Title]) VALUES" +
                        "(@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8);";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@p1", model.GroupId);
                cmd.Parameters.AddWithValue("@p2", model.Author);
                cmd.Parameters.AddWithValue("@p3", model.CoAuthors == null ? "" : model.CoAuthors);
                cmd.Parameters.AddWithValue("@p4", model.AbstarctBody);
                cmd.Parameters.AddWithValue("@p5", model.KeyWords);
                cmd.Parameters.AddWithValue("@p6", model.PresentationType);
                cmd.Parameters.AddWithValue("@p7", model.INCISETheme);
                cmd.Parameters.AddWithValue("@p8", model.AbstarctTitle);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public List<EditBrief> GetPreviousVersions(int groupId)
        {
            List<EditBrief> list = new List<EditBrief>();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                string sql = "SELECT Id,[Statement],EditTime FROM AbstractNodes WHERE GroupId=@p1;";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@p1", groupId);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    EditBrief eb = new EditBrief();
                    eb.Id = dr.GetInt32(0);
                    eb.Statement = (State)dr.GetInt32(1);
                    eb.EditDate = dr.GetDateTime(2);
                    list.Add(eb);
                }
            }
            return list;
        }

        public List<AbstractModel> GetWaitingList()
        {
            List<AbstractModel> list = new List<AbstractModel>();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = "select t2.*,t3.UserName from " +
                    "(select max(Id) as Id from AbstractNodes where [Statement] = 1 group by GroupId) as t1 " +
                    "left join AbstractNodes as t2 on t1.Id = t2.Id " +
                    "left join AbstractGroups as t3 on t2.GroupId=t3.GroupId; ";
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(FillAbstractNode(ref dr));
                }
            }
            return list;
        }

        public bool Pass(int Id,int groupId)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                SqlTransaction transaction = conn.BeginTransaction();
                cmd.Transaction = transaction;
                cmd.CommandType = System.Data.CommandType.Text;
                try
                {
                    cmd.CommandText = "UPDATE AbstractNodes SET [Statement]=2 WHERE Id=@p1 and Statement=1;";
                    cmd.Parameters.AddWithValue("@p1", Id);
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "UPDATE AbstractNodes SET [Statement]=5 WHERE GroupId=@p2 and Statement=1;";
                    cmd.Parameters.AddWithValue("@p2", groupId);
                    cmd.ExecuteNonQuery();

                    transaction.Commit();
                    return true;
                }
                catch (SqlException e1)
                {
                    try
                    {
                        transaction.Rollback();
                        return false;
                    }
                    catch (Exception e2) { return false; }
                }
            }
        }

        public bool Reject(int Id)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = "UPDATE AbstractNodes SET [Statement]=3 WHERE Id=@p1 and Statement=1;";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@p1", Id);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool Delete(int Id)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = "UPDATE AbstractNodes SET [Statement]=3 WHERE Id=@p1 and Statement=2;";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@p1", Id);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public MailBrief GetMailBrief(int Id)
        {
            MailBrief brief = new MailBrief();
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                string sql = "select t1.UserName,t2.* from AbstractGroups as t1 " +
                    "left join AbstractNodes as t2 on t2.GroupId = t1.GroupId " +
                    "where Id = @p1; ";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@p1", Id);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    brief.MailAddress = dr.GetString(0);
                    brief.Id = dr.GetInt32(1);
                    brief.GroupId = dr.GetInt32(2);
                    brief.SubmitTime = dr.GetDateTime(4);
                    brief.Title = dr.GetString(7);
                }
                return brief;
            }
        }
    }
}