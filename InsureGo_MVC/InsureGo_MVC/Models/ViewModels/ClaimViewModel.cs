using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace InsureGo_MVC.Models.ViewModels
{
    public class ClaimViewModel
    {
        public int ClaimId { get; set; }
        public string PolicyNumber { get; set; }
        public string ClaimStatus { get; set; }
        public decimal? Amount { get; set; }
    }
}

