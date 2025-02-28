namespace HospitalManagementSystem_Assignment2.Models
{
    public class Patient : User
    {
        public int PatientID { get; set; }
        public string BloodGroup { get; set;}
        public string MedicalHistory { get; set;}
        public string DrugAllergy { get; set; }
    }
}
