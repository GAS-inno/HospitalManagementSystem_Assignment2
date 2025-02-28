using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using HospitalManagementSystem_Assignment2.Models;

namespace HospitalManagementSystem_Assignment2.Controllers
{
    public class PatientController : Controller
    {
        private static readonly string xmlFilePathForBookings = "App_Data/Bookings.xml";
        private static readonly string xmlFilePathForUsers = "App_Data/Users.xml";
        private static readonly string xmlFilePathForDoctors = "App_Data/Doctors.xml";
        private static readonly string xmlFilePathForAttendances = "App_Data/Attendances.xml";
        public IActionResult BookingOverview(string id)
        {
            XDocument bookingsDoc = XDocument.Load(xmlFilePathForBookings);
            string userId = string.IsNullOrEmpty(id) ? TempData["UserId"] as string : id;
            TempData["UserId"] = userId;
            List<Booking> bookingList = (
                from book in bookingsDoc.Descendants("Booking")
                where book.Element("PatientID")?.Value == userId
                select new Booking
                {
                    BookingNo = int.Parse(book.Element("BookingNo").Value),
                    PatientID = int.Parse(book.Element("PatientID").Value),
                    DoctorName = GetDoctorNameById(int.Parse(book.Element("EmployeeID").Value)),
                    BookingDate = book.Element("BookingDate").Value,
                    BookingTime = book.Element("BookingTime").Value,
                    Status = int.Parse(book.Element("Status").Value)
                }).ToList();

            return View(bookingList);
        }

        public IActionResult CreateBooking(Booking booking, int selectedDoctor, int id)
        {
            XDocument bookingsXmlDoc = XDocument.Load(xmlFilePathForBookings);
            TempData["UserId"] = id;
            // If bookingList is null, initialize it to an empty list
            if (booking.bookingList == null)
            {
                booking.bookingList = new List<Booking>();
            }

            // new redirect to this page
            if(booking.BookingDate == null && booking.BookingTime == null)
            {
                return View();
            }

            XElement checkBooking = bookingsXmlDoc.Descendants("Booking")
                                      .FirstOrDefault(u =>
                                          u.Element("BookingDate").Value == booking.BookingDate &&
                                          u.Element("BookingTime").Value == booking.BookingTime
                                      );
            if (checkBooking != null)
            {
                TempData["AddBookingErrorMessage"] = "You had booked a same time slot on the day. Please book another time slot.";
                return View();
            }

            int newBookingNo = GetNewBookingNo(bookingsXmlDoc); // Get New ID

            //Add the new user to the XML documents
            XElement newBookingElement = new XElement("Booking",
            new XElement("BookingNo", newBookingNo),
                new XElement("EmployeeID", selectedDoctor),
                new XElement("PatientID", id),
                new XElement("BookingDate", booking.BookingDate),
                new XElement("BookingTime", booking.BookingTime),
                new XElement("Status", "0") // 0: created
            );

            bookingsXmlDoc.Element("Bookings").Add(newBookingElement);

            // Save the updated XML documents back to their respective paths
            bookingsXmlDoc.Save(xmlFilePathForBookings);

            TempData["BookingMessage"] = "Booking No: " + newBookingNo + " was created.";
            // Redirect to the AccountManagement page or any other desired page
            return RedirectToAction("BookingOverview", "Patient", new { id = id });
        }

        public IActionResult CancelBooking(int id)
        {
            // Load the XML document
            XDocument bookingsXmlDoc = XDocument.Load(xmlFilePathForBookings);

            // Find the user element with the matching ID and remove it
            XElement bookingToCancel = bookingsXmlDoc.Root.Elements("Booking").FirstOrDefault(u => (int)u.Element("BookingNo") == id);
            if (bookingToCancel != null)
            {
                bookingToCancel.Remove();

                // Save the changes back to the XML file
                bookingsXmlDoc.Save(xmlFilePathForBookings);
            }

            TempData["BookingMessage"] = "Booking Cancelled.";
            string userid = bookingToCancel.Element("PatientID").Value;

            return RedirectToAction("BookingOverview", "Patient", new { id = userid });
        }

        public IActionResult ShowDetail(int id)
        {
            // Load the XML document
            XDocument attendanceXmlDoc = XDocument.Load(xmlFilePathForAttendances);

            // Find the user element with the matching ID and remove it
            XElement bookingDetail = attendanceXmlDoc.Root.Elements("Attendance").FirstOrDefault(u => (int)u.Element("BookingNo") == id);
            if (bookingDetail != null)
            {
                TempData["DetailMessage"] = "Diagnosis Detail : " + bookingDetail.Element("DiagnosisInfo").Value;
                TempData["RemarkMessage"] = "Remark from Doctor : " + (string.IsNullOrEmpty(bookingDetail.Element("Remark").Value) ? "N/A" : bookingDetail.Element("Remark").Value);
                TempData["TherapyMessage"] = "Therapy required : " + (string.IsNullOrEmpty(bookingDetail.Element("Therapy").Value) ? "N/A" : bookingDetail.Element("Therapy").Value);
            }
            string userid = GetPatientIdByBookingNo(id);
            return RedirectToAction("BookingOverview", "Patient", new { id = userid });
        }

        public IActionResult GetDoctorList(string bookingDate, string bookingTime)
        {
            // Create a Booking object with the selected date and time
            Booking booking = new Booking
            {
                BookingDate = bookingDate,
                BookingTime = bookingTime
            };

            // Get the list of available doctors
            List<Booking> availableDoctors = GetAvailableDoctorsForBooking(booking);

            // Return the data as JSON
            return Json(availableDoctors);
        }

        private string GetDoctorNameById(int id)
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

        private List<Booking> GetAvailableDoctorsForBooking(Booking booking)
        {
            // Load the XML data for doctors and bookings
            XDocument doctorsDoc = XDocument.Load(xmlFilePathForDoctors);
            XDocument bookingsDoc = XDocument.Load(xmlFilePathForBookings);

            // Create a string representation of the selected date and time
            string selectedDateTime = booking.BookingDate + " " + booking.BookingTime;

            // Query the XML data to get a list of doctors who do not have a booking at the selected date and time
            var availableDoctors = (
                from doctor in doctorsDoc.Descendants("Doctor")
                where !bookingsDoc.Descendants("Booking")
                    .Any(bookingElement =>
                        bookingElement.Element("EmployeeID").Value == doctor.Element("EmployeeID").Value &&
                        (bookingElement.Element("BookingDate").Value + " " + bookingElement.Element("BookingTime").Value == selectedDateTime)
                    )
                select new Booking
                {
                    EmployeeID = int.Parse(doctor.Element("EmployeeID")?.Value),
                    DoctorName = GetDoctorNameById(int.Parse(doctor.Element("EmployeeID")?.Value))
                }
            ).ToList();

            return availableDoctors;
        }

        private int GetNewBookingNo(XDocument bookingXml)
        {
            var lastBooking = bookingXml.Descendants("Booking").LastOrDefault();
            if (lastBooking != null && int.TryParse(lastBooking.Element("BookingNo").Value, out int lastBookingNo))
            {
                return lastBookingNo + 1;
            }

            return 1; // Default to 1 if there are no existing users
        }

        private string GetPatientIdByBookingNo(int id)
        {
            // Load the XML file
            XDocument doc = XDocument.Load(xmlFilePathForBookings);

            // Find the user element with the specified ID
            XElement bookingElement = doc.Descendants("Booking")
                                      .FirstOrDefault(u => u.Element("BookingNo").Value == id.ToString());

            // If the user with the specified ID exists, get the Name
            if (bookingElement != null)
            {
                return bookingElement.Element("PatientID").Value;
            }

            return "";
        }

    }
}
