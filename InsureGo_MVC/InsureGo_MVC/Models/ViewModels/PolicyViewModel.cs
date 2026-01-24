
using System;

namespace InsureGo_MVC.Models.ViewModels
{
    public class PolicyViewModel
    {
        public int PolicyId { get; set; }
        public int VehicleId { get; set; }
        public int UserId { get; set; }
        public string PolicyNumber { get; set; }
        public string VehicleModel { get; set; }
        public string RegistrationNumber { get; set; }
        public decimal PremiumAmount { get; set; }
        public string PolicyStatus { get; set; }
        public decimal ClaimAmount { get; set; }
        public string PlanType { get; set; }
        public int Duration { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
