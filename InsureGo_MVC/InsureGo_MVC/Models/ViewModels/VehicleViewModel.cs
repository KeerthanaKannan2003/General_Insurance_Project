using System;
using System.ComponentModel.DataAnnotations;

namespace InsureGo_MVC.Models.ViewModels
{
    public class VehicleViewModel
    {
        [Required]
        public string VehicleType { get; set; }

        [Required]
        public string Manufacturer { get; set; }

        [Required]
        public string VehicleModel { get; set; }

        [Required]
        public string DrivingLicence { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime PurchaseDate { get; set; }

        [Required]
        public string RegistrationNumber { get; set; }

        [Required]
        public string EngineNumber { get; set; }

        [Required]
        public string ChassisNumber { get; set; }

        public int VehicleId { get; set; }


        public int UserId { get; set; }
        public int VehicleTypeId { get; set; }
    }
}
