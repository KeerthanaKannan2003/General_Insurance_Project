
using System.ComponentModel.DataAnnotations;

namespace InsureGo_MVC.Models.ViewModels
{
    public class AdminLoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class SetClaimAmountViewModel
    {
        public int ClaimId { get; set; }
        [Required]
        public decimal Amount { get; set; }
    }
}
