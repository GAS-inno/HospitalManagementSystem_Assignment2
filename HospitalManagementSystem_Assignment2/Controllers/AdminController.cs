
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using HospitalManagementSystem_Assignment2.Models;

namespace HospitalManagementSystem_Assignment2.Controllers.Admin
{
    public class AdminController : Controller
    {
        private static readonly string xmlFilePathForUsers = "App_Data/Users.xml";
        private static readonly string xmlFilePathForDoctors = "App_Data/Doctors.xml";
        private static readonly string xmlFilePathForPatients = "App_Data/Patients.xml";

        public IActionResult AccountManagement()
        {
            XDocument usersDoc = XDocument.Load(xmlFilePathForUsers);
            XDocument doctorsDoc = XDocument.Load(xmlFilePathForDoctors);
            XDocument patientsDoc = XDocument.Load(xmlFilePathForPatients);

            List<DoctorList> doctorList = (
            from user in usersDoc.Descendants("User")
            join doctor in doctorsDoc.Descendants("Doctor") on user.Element("ID").Value equals doctor.Element("EmployeeID").Value
            select new DoctorList
            {
                EmployeeID = int.Parse(user.Element("ID").Value),
                FirstName = user.Element("FirstName").Value,
                LastName = user.Element("LastName").Value,
                Email = user.Element("Email").Value,
                Password = user.Element("Password").Value,
                DOB = user.Element("DOB").Value,
                Phone = int.Parse(user.Element("Phone").Value),
                Gender = user.Element("Gender").Value,
                AccountType = int.Parse(user.Element("AccountType").Value),
                Specialist = doctor.Element("Specialist").Value
            }).ToList();

            List<PatientList> patientList = (
            from user in usersDoc.Descendants("User")
            join patient in patientsDoc.Descendants("Patient") on user.Element("ID").Value equals patient.Element("PatientID").Value
            select new PatientList
            {
                ID = int.Parse(user.Element("ID").Value),
                FirstName = user.Element("FirstName").Value,
                LastName = user.Element("LastName").Value,
                Email = user.Element("Email").Value,
                Password = user.Element("Password").Value,
                DOB = user.Element("DOB").Value,
                Phone = int.Parse(user.Element("Phone").Value),
                Gender = user.Element("Gender").Value,
                AccountType = int.Parse(user.Element("AccountType").Value),
                BloodGroup = patient.Element("BloodGroup").Value,
                MedicalHistory = patient.Element("MedicalHistory").Value,
                DrugAllergy = patient.Element("DrugAllergy").Value
            }).ToList();


            List<AdminList> adminList = (
        from user in usersDoc.Descendants("User")
        where (int)user.Element("AccountType") == 0 // AccountType 0 for Admin
        select new AdminList
        {
            ID = int.Parse(user.Element("ID").Value),
            FirstName = user.Element("FirstName").Value,
            LastName = user.Element("LastName").Value,
            Email = user.Element("Email").Value,
            Password = user.Element("Password").Value,
            DOB = user.Element("DOB").Value,
            Phone = int.Parse(user.Element("Phone").Value),
            Gender = user.Element("Gender").Value
        }).ToList();


            UserList userList = new UserList
            {
                doctorList = doctorList,
                patientList = patientList,
                 adminList = adminList
            };

            return View(userList);
        }

        public IActionResult CreateAccount(Account account)
        {
            if (ModelState.IsValid)
            {
                XDocument usersDoc = XDocument.Load(xmlFilePathForUsers);
                XDocument doctorsDoc = XDocument.Load(xmlFilePathForDoctors);
                XDocument patientsDoc = XDocument.Load(xmlFilePathForPatients);

                XElement checkEmail = usersDoc.Root.Elements("User").FirstOrDefault(u => (string)u.Element("Email") == account.Email);
                if (checkEmail != null)
                {
                    TempData["ErrorMessage"] = "Email existed, please use another email.";
                    return View();
                }

                int newUserId = GetNewUserId(usersDoc); // Get New ID

                //Add the new user to the XML documents
                XElement newUserElement = new XElement("User",
                new XElement("ID", newUserId),
                    new XElement("FirstName", account.FirstName),
                    new XElement("LastName", account.LastName),
                    new XElement("Email", account.Email),
                    new XElement("Phone", account.Phone),
                    new XElement("Password", account.Password),
                    new XElement("Gender", account.Gender),
                    new XElement("DOB", account.DOB),
                    new XElement("AccountType", account.AccountType)
                );

                usersDoc.Element("Users").Add(newUserElement);

                if (account.AccountType == 1) // Doctor
                {
                    // Add doctor details to the doctorsDoc
                    XElement newDoctorElement = new XElement("Doctor",
                        new XElement("EmployeeID", newUserId),
                        new XElement("Specialist", account.Specialist)
                    );

                    doctorsDoc.Element("Doctors").Add(newDoctorElement);
                }
                else if (account.AccountType == 2) // Patient
                {
                    // Add patient details to the patientsDoc
                    XElement newPatientElement = new XElement("Patient",
                        new XElement("PatientID", newUserId),
                        new XElement("BloodGroup", account.BloodGroup),
                        new XElement("MedicalHistory", account.MedicalHistory),
                        new XElement("DrugAllergy", account.DrugAllergy)
                    );

                    patientsDoc.Element("Patients").Add(newPatientElement);
                }

                // Save the updated XML documents back to their respective paths
                usersDoc.Save(xmlFilePathForUsers);
                doctorsDoc.Save(xmlFilePathForDoctors);
                patientsDoc.Save(xmlFilePathForPatients);


                // Add success message to TempData
                TempData["SuccessMessage"] = "Account created successfully.";

                // Redirect to the AccountManagement page or any other desired page
                return RedirectToAction("AccountManagement", "Admin");
            }

            // If the model is not valid, return the same view with validation errors
            return View(account);
        }

        public IActionResult EditDoctor(int id)
        {
            XDocument usersDoc = XDocument.Load(xmlFilePathForUsers);
            XDocument doctorsDoc = XDocument.Load(xmlFilePathForDoctors);

            var doc = from user in usersDoc.Descendants("User")
                        join doctor in doctorsDoc.Descendants("Doctor")
                        on user.Element("ID").Value equals doctor.Element("EmployeeID").Value
                        where (int)user.Element("ID") == id
                        select new DoctorList
                        {
                            EmployeeID = int.Parse(user.Element("ID").Value),
                            FirstName = user.Element("FirstName").Value,
                            LastName = user.Element("LastName").Value,
                            Email = user.Element("Email").Value,
                            Password = user.Element("Password").Value,
                            DOB = user.Element("DOB").Value,
                            Phone = int.Parse(user.Element("Phone").Value),
                            Gender = user.Element("Gender").Value,
                            AccountType = int.Parse(user.Element("AccountType").Value),
                            Specialist = doctor.Element("Specialist").Value
                        };

            var doctorInfo = doc.FirstOrDefault();
            var accountModel = new Account
            {
                ID = doctorInfo.EmployeeID,
                FirstName = doctorInfo.FirstName,
                LastName = doctorInfo.LastName,
                Email = doctorInfo.Email,
                Phone = doctorInfo.Phone,
                Gender = doctorInfo.Gender,
                DOB = doctorInfo.DOB,
                Specialist = doctorInfo.Specialist
            };

            return View(accountModel);
        }

        public IActionResult EditPatient(int id)
        {
            XDocument usersDoc = XDocument.Load(xmlFilePathForUsers);
            XDocument patientsDoc = XDocument.Load(xmlFilePathForPatients);

            var doc = from user in usersDoc.Descendants("User")
                      join patient in patientsDoc.Descendants("Patient")
                      on user.Element("ID").Value equals patient.Element("PatientID").Value
                      where (int)user.Element("ID") == id
                      select new PatientList
                      {
                          ID = int.Parse(user.Element("ID").Value),
                          FirstName = user.Element("FirstName").Value,
                          LastName = user.Element("LastName").Value,
                          Email = user.Element("Email").Value,
                          Password = user.Element("Password").Value,
                          DOB = user.Element("DOB").Value,
                          Phone = int.Parse(user.Element("Phone").Value),
                          Gender = user.Element("Gender").Value,
                          AccountType = int.Parse(user.Element("AccountType").Value),
                          BloodGroup = patient.Element("BloodGroup").Value,
                          MedicalHistory = patient.Element("MedicalHistory").Value,
                          DrugAllergy = patient.Element("DrugAllergy").Value
                      };

            var patientInfo = doc.FirstOrDefault();
            var accountModel = new Account
            {
                ID = patientInfo.ID,
                FirstName = patientInfo.FirstName,
                LastName = patientInfo.LastName,
                Email = patientInfo.Email,
                Phone = patientInfo.Phone,
                Gender = patientInfo.Gender,
                DOB = patientInfo.DOB,
                BloodGroup = patientInfo.BloodGroup,
                MedicalHistory = patientInfo.MedicalHistory,
                DrugAllergy = patientInfo.DrugAllergy
            };

            return View(accountModel);
        }

        public IActionResult DeleteDoctor(int id)
        {
            // Load the XML document
            XDocument userXmlDoc = XDocument.Load(xmlFilePathForUsers);
            XDocument doctorXmlDoc = XDocument.Load(xmlFilePathForDoctors);

            // Find the user element with the matching ID and remove it
            XElement userToDelete = userXmlDoc.Root.Elements("User").FirstOrDefault(u => (int)u.Element("ID") == id);
            if (userToDelete != null)
            {
                userToDelete.Remove();

                // Save the changes back to the XML file
                userXmlDoc.Save(xmlFilePathForUsers);
            }

            XElement doctorToDelete = doctorXmlDoc.Root.Elements("Doctor").FirstOrDefault(u => (int)u.Element("EmployeeID") == id);
            if (doctorToDelete != null)
            {
                doctorToDelete.Remove();

                // Save the changes back to the XML file
                doctorXmlDoc.Save(xmlFilePathForDoctors);
            }

            TempData["DoctorMessage"] = "Doctor removed.";

            // Redirect back to the account management page after the delete operation
            return RedirectToAction("AccountManagement", "Admin");
        }

        public IActionResult DeletePatient(int id)
        {
            // Load the XML document
            XDocument userXmlDoc = XDocument.Load(xmlFilePathForUsers);
            XDocument patientXmlDoc = XDocument.Load(xmlFilePathForPatients);

            // Find the user element with the matching ID and remove it
            XElement userToDelete = userXmlDoc.Root.Elements("User").FirstOrDefault(u => (int)u.Element("ID") == id);
            if (userToDelete != null)
            {
                userToDelete.Remove();

                // Save the changes back to the XML file
                userXmlDoc.Save(xmlFilePathForUsers);
            }

            XElement patientToDelete = patientXmlDoc.Root.Elements("Patient").FirstOrDefault(u => (int)u.Element("PatientID") == id);
            if (patientToDelete != null)
            {
                patientToDelete.Remove();

                // Save the changes back to the XML file
                patientXmlDoc.Save(xmlFilePathForPatients);
            }

            TempData["PatientMessage"] = "Patient removed.";

            // Redirect back to the account management page after the delete operation
            return RedirectToAction("AccountManagement", "Admin");
        }

        private int GetNewUserId(XDocument usersDoc)
        {
            var lastUser = usersDoc.Descendants("User").LastOrDefault();
            if (lastUser != null && int.TryParse(lastUser.Element("ID").Value, out int lastUserId))
            {
                return lastUserId + 1;
            }

            return 1; // Default to 1 if there are no existing users
        }
    }
}
