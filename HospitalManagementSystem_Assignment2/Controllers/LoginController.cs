using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using HospitalManagementSystem_Assignment2.Models;

namespace HospitalManagementSystem_Assignment2.Controllers
{
    public class LoginController : Controller
    {
        private static readonly string xmlFilePath = "App_Data/Users.xml"; // Path to the XML file storing user info

        public IActionResult Login(User model)
        {
            // Clear previous messages each login attempt
            TempData.Clear();

           //input validate
            if (!string.IsNullOrWhiteSpace(model.Email) && !string.IsNullOrWhiteSpace(model.Password))
            {
                // Load the XML file 
                XDocument doc = XDocument.Load(xmlFilePath);

                // Look for a user element 
                XElement matchingUser = doc.Descendants("User")
                    .FirstOrDefault(user =>
                        (string)user.Element("Email") == model.Email &&
                        (string)user.Element("Password") == model.Password);

                // If a matching user was found in the XML file
                if (matchingUser != null)
                {
                    string accountType = (string)matchingUser.Element("AccountType"); // Read the account type
                    TempData["UserId"] = (string)matchingUser.Element("ID"); // Store user ID in TempData for later use

                    // Redirect account type
                    if (accountType == "1")
                    {
                        return RedirectToAction("BookingList", "Doctor"); // For doctors
                    }
                    else if (accountType == "2")
                    {
                        return RedirectToAction("BookingOverview", "Patient"); // For patients
                    }
                    else
                    {
                        return RedirectToAction("AccountManagement", "Admin"); // For admins
                    }
                }
                else
                {
                    // No matching user found, show an error
                    TempData["ErrorMessage"] = "No such user and password found";
                }
            }          

            // Return to the login view with any error message
            return View();
        }
    }
}
