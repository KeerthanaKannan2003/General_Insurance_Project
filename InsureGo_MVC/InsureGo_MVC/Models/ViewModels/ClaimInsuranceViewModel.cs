using System;
using System.ComponentModel.DataAnnotations;

namespace InsureGo_MVC.Models.ViewModels
{
    public class ClaimInsuranceViewModel
    {
        public int ClaimId { get; set; }
        public int PolicyId { get; set; }

        [Required]
        [Display(Name = "Policy Number")]
        public string PolicyNumber { get; set; }

        [Required]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }
        public DateTime? ClaimDate { get; set; }
        [Required]
        [Display(Name = "Reason for Claim")]
        public string ClaimReason { get; set; }

        [Display(Name = "Claim Amount")]
        public decimal? ClaimAmount { get; set; }

        [Display(Name = "Status")]
        public string ClaimStatus { get; set; }
    }
}
