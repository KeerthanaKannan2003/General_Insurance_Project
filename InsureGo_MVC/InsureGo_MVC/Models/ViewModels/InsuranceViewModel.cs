using System.ComponentModel.DataAnnotations;

namespace InsureGo_MVC.Models.ViewModels
{
    public class InsuranceViewModel
    {
        public int PolicyId { get; set; }
        public string PolicyNumber { get; set; }

        public string VehicleType { get; set; }
        public string VehicleModel { get; set; }
        public string RegistrationNumber { get; set; }

        [Required(ErrorMessage = "Please select an insurance plan")]
        public int PlanId { get; set; }

        [Required(ErrorMessage = "Please enter duration in years")]
        [Range(1, 5, ErrorMessage = "Duration must be between 1 and 5 years")]
        public int DurationYears { get; set; }

        public string PlanName { get; set; }
        public int DurationId { get; set; }
    }
}