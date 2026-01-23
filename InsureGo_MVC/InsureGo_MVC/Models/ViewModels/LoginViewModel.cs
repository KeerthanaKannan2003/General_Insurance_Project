using System.ComponentModel.DataAnnotations;
namespace InsureGo_MVC.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string EmailId { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
