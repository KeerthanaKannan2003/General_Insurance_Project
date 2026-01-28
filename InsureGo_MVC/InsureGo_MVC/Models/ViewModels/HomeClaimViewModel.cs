using System;

namespace InsureGo_MVC.Models.ViewModels
{
    public class HomeClaimViewModel
    {
        public int ClaimId { get; set; }            
        public string PolicyNumber { get; set; }
        public string VehicleNumber { get; set; }
        public decimal? ClaimAmount { get; set; }
        public string ClaimStatus { get; set; }
        public string ClaimReason { get; set; }    
        public DateTime ClaimDate { get; set; }    
    }
}
