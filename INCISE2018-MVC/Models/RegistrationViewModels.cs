using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace INCISE2018_MVC.Models
{
    public class RegistrationViewModel
    {
        public bool IsRegisted { get; set; }
    }

    public class ResultViewModel
    {
        public List<TripSignUpDateModel> List { get; set; }
    }

    public class TripSignUpDateModel
    {
        public string Mail { get; set; }
        public DateTime SignUpDate { get; set; }
    }
}