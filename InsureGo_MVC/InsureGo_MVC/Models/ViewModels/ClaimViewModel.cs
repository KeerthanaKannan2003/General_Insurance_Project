using System.ComponentModel.DataAnnotations;

namespace InsureGo_MVC.Models.ViewModels
{
    public class ClaimInsuranceViewModel
    {
        [Required]
        [Display(Name = "Policy Number")]
        public string PolicyNumber { get; set; }

        [Required]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }

        [Required]
        [Display(Name = "Reason for Claim")]
        public string ClaimReason { get; set; }
    }

}
