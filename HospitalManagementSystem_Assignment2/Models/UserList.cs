namespace HospitalManagementSystem_Assignment2.Models
{
    public class UserList
    {
        public List<DoctorList> doctorList { get; set; }
        public List<PatientList> patientList { get; set; }
        public List<AdminList> adminList { get; set; }
    }
}
