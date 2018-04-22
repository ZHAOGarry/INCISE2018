using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace INCISE2018_MVC.Managers
{
    public class SqlDbConnection
    {
        protected string ConnectionString;

        public SqlDbConnection(string ConnectionName = "DefaultConnection")
        {
            ConnectionString = ConfigurationManager.ConnectionStrings[ConnectionName].ConnectionString;
        }
    }
}