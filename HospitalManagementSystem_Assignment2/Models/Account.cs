using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem_Assignment2.Models
{
    public class Account
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "DOB is required.")]
        public string DOB { get; set; }

        [Required(ErrorMessage = "Phone is required.")]
        [RegularExpression(@"^[0-9]{8,}$", ErrorMessage = "Invalid phone number.")]
        public int Phone { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; }
        public int AccountType { get; set; } 
        // 0: Admin | 1: Doctor | 2: Patient
        public string? Specialist { get; set; }
        
        public string? MedicalHistory { get; set; }
        public string? BloodGroup { get; set; }
        public string? DrugAllergy { get; set; }
    }
}
