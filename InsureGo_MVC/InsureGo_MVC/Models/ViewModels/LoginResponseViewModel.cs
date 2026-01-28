using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InsureGo_MVC.Models.ViewModels
{
    public class LoginResponseViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
    }
}