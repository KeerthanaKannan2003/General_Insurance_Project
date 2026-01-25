using System.ComponentModel.DataAnnotations;

namespace InsureGo_MVC.Models.ViewModels
{
    public class PremiumViewModel
    {
        [Required]
        [Display(Name = "Vehicle Type")]
        public int VehicleTypeId { get; set; }  // 1 = 2W, 2 = 4W

        [Required]
        [Range(0, 50)]
        [Display(Name = "Vehicle Age (Years)")]
        public int VehicleAge { get; set; }

        [Display(Name = "Premium Amount")]
        public decimal PremiumAmount { get; set; }
    }
}
