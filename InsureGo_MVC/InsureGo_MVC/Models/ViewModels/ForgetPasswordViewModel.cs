
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace InsureGo_MVC.Models.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        public string EmailId { get; set; }
    }
}
