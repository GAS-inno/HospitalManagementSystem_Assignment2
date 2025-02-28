namespace HospitalManagementSystem_Assignment2.Models
{
    public class Booking
    {
        public int BookingNo { get; set; }
        public int EmployeeID { get; set; }
        public string DoctorName { get; set; }
        public int PatientID { get; set; }
        public string PatientName { get; set; }
        public string BookingDate { get; set; }
        public string BookingTime { get; set; }
        public int Status { get; set; } // 0: Created | 1: Completed
        public List<Booking> bookingList { get; set; }
    }
}
