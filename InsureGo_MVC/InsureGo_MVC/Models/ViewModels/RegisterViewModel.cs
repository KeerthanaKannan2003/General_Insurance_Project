
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InsureGo_MVC.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        public string EmailId { get; set; }

        [Required]
        public string ContactNumber { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Address { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
