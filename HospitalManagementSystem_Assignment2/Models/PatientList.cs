namespace HospitalManagementSystem_Assignment2.Models
{
    public class PatientList
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string DOB { get; set; }
        public int Phone { get; set; }
        public string Gender { get; set; }
        public int AccountType { get; set; } 
        // 0: Admin | 1: Doctor | 2: Patient
        public string BloodGroup { get; set; }
        public string MedicalHistory { get; set; }
        public string DrugAllergy { get; set; }
    }
}
