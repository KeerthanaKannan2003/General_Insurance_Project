using System.ComponentModel.DataAnnotations;

namespace InsureGo_MVC.Models.ViewModels
{
    public class PremiumViewModel
    {
        [Required(ErrorMessage = "Please select vehicle type")]
        [Display(Name = "Vehicle Type")]
        public int? VehicleTypeId { get; set; }  

        [Required(ErrorMessage = "Vehicle age is required")]
        [Range(0, 50, ErrorMessage = "Enter a valid vehicle age (0-50)")]
        [Display(Name = "Vehicle Age (Years)")]
        public int VehicleAge { get; set; }

        [Display(Name = "Premium Amount")]
        public decimal PremiumAmount { get; set; }
    }
}
