using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace INCISE2018_MVC.Models
{
    public class AbstractModel
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public DateTime EditTime { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string Author { get; set; }
        [Display(Name = "Co-Author(s)")]
        public string CoAuthors { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Abstarct Title")]
        public string AbstarctTitle { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Abstarct Body")]
        public string AbstarctBody { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "INCISE Theme")]
        public string INCISETheme { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Presentation Type")]
        public string PresentationType { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Keywords")]
        public string KeyWords { get; set; }
        public State Statement { get; set; }
        public string Owner { get; set; }
    }

    public enum State
    {
        Saved = 0,
        Auditing = 1,
        Approved = 2,
        Fail = 3,
        Withdraw = 4,
        Old =5
    }

    public class AbstractGroupModel
    {
        public int GroupId { get; set; }
        public string AspNetUsersName { get; set; }
        public int ShowNodeId { get; set; }
        public List<AbstractModel> AbstractNodes { get; set; }
    }

    public class AbstractViewModel
    {
        public List<AbstractModel> AbstarctList { get; set; }
    }

    public class MyAbstractViewModel
    {
        public Dictionary<AbstractModel,List<AbstractModel>> AbstractDictionary { get; set; }
    }

    public class EditBrief
    {
        public int Id { get; set; }
        public DateTime EditDate { get; set; }
        public State Statement { get; set; }
    }

    public class ConsoleViewModel
    {
        public List<AbstractModel> WaitList { get; set; }
        public List<AbstractModel> ShowList { get; set; }
    }

    public class MailBrief
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string Title { get; set; }
        public string MailAddress { get; set; }
        public DateTime SubmitTime { get; set; }
    }

}