using System;

using System.ComponentModel.DataAnnotations;

namespace InsureGo_MVC.Models.ViewModels

{

    public class RegisterViewModel

    {

        [Required(ErrorMessage = "Full name is required")]

        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]

        [EmailAddress(ErrorMessage = "Invalid email format")]

        public string EmailId { get; set; }

        [Required(ErrorMessage = "Contact number is required")]

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Contact number must be exactly 10 digits")]

        public string ContactNumber { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]

        [DataType(DataType.Date)]

        [CustomValidation(typeof(RegisterViewModel), nameof(ValidateDOB))]

        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Address is required")]

        public string Address { get; set; }

        [Required(ErrorMessage = "Password is required")]

        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]

        public string Password { get; set; }



        public static ValidationResult ValidateDOB(DateTime dob, ValidationContext context)

        {

            if (dob >= DateTime.Today)

            {

                return new ValidationResult("Date of birth must be less than current date");

            }

            return ValidationResult.Success;

        }

    }

}

