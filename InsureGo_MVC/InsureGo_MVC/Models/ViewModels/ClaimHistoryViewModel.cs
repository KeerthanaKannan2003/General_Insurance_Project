using System;

namespace InsureGo_MVC.Models.ViewModels
{
    public class ClaimHistoryViewModel
    {
        public int ClaimId { get; set; }
        public string PolicyNumber { get; set; }
        public DateTime? ClaimDate { get; set; }
        public decimal? ClaimAmount { get; set; }
        public string ClaimStatus { get; set; }
    }
}
