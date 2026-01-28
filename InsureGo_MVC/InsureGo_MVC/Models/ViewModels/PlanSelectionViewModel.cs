using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InsureGo_MVC.Models.ViewModels
{
    public class PlanSelectionViewModel
    {
        public int PolicyId { get; set; }
        public string PlanName { get; set; }
        public decimal PremiumAmount { get; set; }
        public int DurationYears { get; set; }
        public string VehicleModel { get; set; }
        public string RegistrationNumber { get; set; }
    }

}