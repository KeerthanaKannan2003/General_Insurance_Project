
using System.ComponentModel.DataAnnotations;

namespace InsureGo_MVC.Models.ViewModels
{
    public class InsuranceViewModel
    {
        internal int PolicyId;
        internal string PolicyNumber;

        // Auto-filled from vehicle step
        public string VehicleType { get; set; }
        public string VehicleModel { get; set; }
        public string RegistrationNumber { get; set; }

        // Plan selection
        [Required(ErrorMessage = "Select a plan")]
        public string PlanType { get; set; }   // Third Party / Comprehensive

        [Required(ErrorMessage = "Select duration")]
        [Range(1, 5, ErrorMessage = "Duration must be 1–5 years")]
        public int Duration { get; set; }
        public object PlanId { get; internal set; }
        public object DurationId { get; internal set; }
    }
}
