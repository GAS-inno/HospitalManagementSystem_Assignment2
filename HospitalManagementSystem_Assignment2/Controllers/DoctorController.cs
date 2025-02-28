
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using HospitalManagementSystem_Assignment2.Models;

namespace HospitalManagementSystem_Assignment2.Controllers
{
    public class DoctorController : Controller
    {
        private static readonly string xmlFilePathForBookings = "App_Data/Bookings.xml";
        private static readonly string xmlFilePathForAttendances = "App_Data/Attendances.xml";
        private static readonly string xmlFilePathForUsers = "App_Data/Users.xml";

        public IActionResult BookingList(string id)
        {
            XDocument bookingsDoc = XDocument.Load(xmlFilePathForBookings);
            string userId = string.IsNullOrEmpty(id) ? TempData["UserId"] as string : id;
            TempData["UserId"] = userId;
            List<Booking> bookingList = (
                from book in bookingsDoc.Descendants("Booking")
                where book.Element("EmployeeID")?.Value == userId
                select new Booking
                {
                    BookingNo = int.Parse(book.Element("BookingNo").Value),
                    PatientID = int.Parse(book.Element("PatientID").Value),
                    PatientName = GetPatientNameById(int.Parse(book.Element("PatientID").Value)),
                    BookingDate = book.Element("BookingDate").Value,
                    BookingTime = book.Element("BookingTime").Value,
                    Status = int.Parse(book.Element("Status").Value)
                }).ToList();

            return View(bookingList);
        }

        public IActionResult CompleteAttendance(Attendance attendance, int id)
        {
            XDocument bookingsXmlDoc = XDocument.Load(xmlFilePathForBookings);
            XDocument attendancesXmlDoc = XDocument.Load(xmlFilePathForAttendances);

            // new redirect to this page
            if (attendance.DiagnosisInfo == null && attendance.Remark == null && attendance.Therapy == null)
            {
                return View();
            }

            XElement checkBooking = bookingsXmlDoc.Descendants("Booking")
                                      .FirstOrDefault(u =>
                                          u.Element("BookingNo").Value == id.ToString());
            if (checkBooking == null)
            {
                TempData["CompleteErrorMessage"] = "Booking not found.";
                return View();
            }

            // Find the booking element based on the booking no
            XElement bookingToUpdate = bookingsXmlDoc.Root.Elements("Booking").FirstOrDefault(u => (int)u.Element("BookingNo") == id);
            if (bookingToUpdate != null)
            {
                bookingToUpdate.Element("Status").Value = "1"; // update to complete

                // Save the changes back to the XML file
                bookingsXmlDoc.Save(xmlFilePathForBookings);
            }

            //Add the new attendance to the XML documents
            XElement newAttendanceElement = new XElement("Attendance",
            new XElement("BookingNo", id),
                new XElement("DiagnosisInfo", attendance.DiagnosisInfo),
                new XElement("Remark", attendance.Remark),
                new XElement("Therapy", attendance.Therapy)
            );

            attendancesXmlDoc.Element("Attendances").Add(newAttendanceElement);

            // Save the updated XML documents back to their respective paths
            attendancesXmlDoc.Save(xmlFilePathForAttendances);

            TempData["BookingMessage"] = "Booking No: " + id + " completed.";

            string userId = GetEmployeeIdByBookingNo(id);

            // Redirect to the AccountManagement page or any other desired page
            return RedirectToAction("BookingList", "Doctor", new { id = userId });
        }

        public IActionResult ShowAttendance(int id)
        {
            // Load the XML document
            XDocument attendanceXmlDoc = XDocument.Load(xmlFilePathForAttendances);

            // Find the user element with the matching ID and remove it
            XElement bookingDetail = attendanceXmlDoc.Root.Elements("Attendance").FirstOrDefault(u => (int)u.Element("BookingNo") == id);
            if (bookingDetail != null)
            {
                TempData["DetailMessage"] = "Diagnosis Detail : " + bookingDetail.Element("DiagnosisInfo").Value;
                TempData["RemarkMessage"] = "Remark : " + (string.IsNullOrEmpty(bookingDetail.Element("Remark").Value) ? "N/A" : bookingDetail.Element("Remark").Value);
                TempData["TherapyMessage"] = "Therapy required : " + (string.IsNullOrEmpty(bookingDetail.Element("Therapy").Value) ? "N/A" : bookingDetail.Element("Therapy").Value);
            }

            string userId = GetEmployeeIdByBookingNo(id);

            // Redirect to the AccountManagement page or any other desired page
            return RedirectToAction("BookingList", "Doctor", new { id = userId });
        }

        private string GetPatientNameById(int id)
        {
            // Load the XML file
            XDocument doc = XDocument.Load(xmlFilePathForUsers);

            // Find the user element with the specified ID
            XElement userElement = doc.Descendants("User")
                                      .FirstOrDefault(u => u.Element("ID").Value == id.ToString());

            // If the user with the specified ID exists, get the Name
            if (userElement != null)
            {
                return userElement.Element("FirstName").Value + " " + userElement.Element("LastName").Value;
            }

            return "";
        }

        private string GetEmployeeIdByBookingNo(int id)
        {
            // Load the XML file
            XDocument doc = XDocument.Load(xmlFilePathForBookings);

            // Find the user element with the specified ID
            XElement bookingElement = doc.Descendants("Booking")
                                      .FirstOrDefault(u => u.Element("BookingNo").Value == id.ToString());

            // If the user with the specified ID exists, get the Name
            if (bookingElement != null)
            {
                return bookingElement.Element("EmployeeID").Value;
            }

            return "";
        }
    }
}
