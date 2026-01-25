using System;

namespace InsureGo_MVC.Models.ViewModels
{
    public class PolicyViewModel
    {
        public string PolicyNumber { get; set; }

        // Vehicle Info
        public string VehicleModel { get; set; }
        public string RegistrationNumber { get; set; }

        // Plan Info
        public string PlanType { get; set; }
        public int? Duration { get; set; }   // years

        // Amount & Status
        public decimal? PremiumAmount { get; set; }
        public string PolicyStatus { get; set; }

        // Optional dates (future use)
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
