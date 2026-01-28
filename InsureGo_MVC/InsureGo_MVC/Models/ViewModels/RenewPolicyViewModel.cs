using System.ComponentModel.DataAnnotations;

namespace InsureGo_MVC.Models.ViewModels
{
    public class RenewPolicyViewModel
    {
        [Required(ErrorMessage = "Policy Number is required")]
        [Display(Name = "Policy Number")]
        public string PolicyNumber { get; set; }
    }
}
