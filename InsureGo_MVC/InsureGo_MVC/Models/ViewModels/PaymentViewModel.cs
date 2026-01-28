
using System;
using System.ComponentModel.DataAnnotations;

namespace InsureGo_MVC.Models.ViewModels
{
    public class PaymentViewModel
    {
        [Required]
        public int PolicyId { get; set; }

        [Required]
        [Range(1, double.MaxValue)]
        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }
    }
}
