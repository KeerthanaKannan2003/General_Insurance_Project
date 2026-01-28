using System;

namespace InsureGo_API.Repository
{
    public class ClaimViewModel
    {
        public int ClaimId { get; set; }
        public int PolicyId { get; set; }
        public string PolicyNumber { get; set; }
        public decimal? ClaimAmount { get; set; }
        public DateTime? ClaimDate { get; set; }
        public string ClaimStatus { get; set; }
    }
}