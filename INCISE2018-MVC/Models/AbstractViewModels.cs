using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace INCISE2018_MVC.Models
{
    public class AbstractModel
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int ParentId { get; set; }
        public DateTime EditTime { get; set; }
        public bool IsChecked { get; set; }
        public string Author { get; set; }
        public string CoAuthors { get; set; }
        public string AbstarctTitle { get; set; }
        public string AbstarctBody { get; set; }
        public string INCISETheme { get; set; }
        public string PresentationType { get; set; }
        public string KeyWords { get; set; }
    }

    public class AbstractGroupModel
    {
        public int GroupId { get; set; }
        public string AspNetUsersName { get; set; }
        public int LastNodeId { get; set; }
        public int ShowNodeId { get; set; }
        public List<AbstractModel> AbstractNodes { get; set; }
    }

    public class AbstractViewModel
    {
        public List<AbstractModel> AbstarctList { get; set; }
    }

}