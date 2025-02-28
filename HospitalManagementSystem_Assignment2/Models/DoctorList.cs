namespace HospitalManagementSystem_Assignment2.Models
{
    public class DoctorList
    {
        public int EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string DOB { get; set; }
        public int Phone { get; set; }
        public string Gender { get; set; }
        public int AccountType { get; set; } // 0: Admin | 1: Doctor | 2: Patient
        public string Specialist { get; set; }
    }
}
