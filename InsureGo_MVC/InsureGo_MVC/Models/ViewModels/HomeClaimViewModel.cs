using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InsureGo_MVC.Models.ViewModels
{
    public class HomeClaimViewModel
    {
        public string PolicyNumber { get; set; }
        public string VehicleNumber { get; set; }
        public decimal? ClaimAmount { get; set; }
        public string ClaimStatus { get; set; }
    }

}