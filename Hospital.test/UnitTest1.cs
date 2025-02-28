using Microsoft.AspNetCore.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml.Linq;

namespace Hospital.test
{
    [TestClass]
    public class UnitTest1
    {
        private static readonly string xmlFilePath = "App_Data/Users.xml";

        [TestMethod]
        public void TestXmlFileExistence()
        {
            // Assert that the file exists
            Assert.IsTrue(File.Exists(xmlFilePath), $"Users.xml file does not exist at path: {xmlFilePath}");
        }


        [TestMethod]
        public void TestAdminAccountExists()
        {
            // Ensure the file exists
            Assert.IsTrue(File.Exists(xmlFilePath), "Users.xml file does not exist.");

            // Read the XML file
            XDocument xmlDoc = XDocument.Load(xmlFilePath);

            // Check if any user has an AccountType of "Admin"
            var adminAccount = xmlDoc.Descendants("User")
                                     .FirstOrDefault(user => user.Element("AccountType")?.Value == "0");

            // Assert that an admin account is found
            Assert.IsNotNull(adminAccount, "Admin account not found in the Users.xml file.");
        }
    }
}