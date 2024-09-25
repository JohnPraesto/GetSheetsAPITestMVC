using GetSheetsAPITestMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GetSheetsAPITestMVC.Controllers
{
    public class EmployeeController : Controller
    {
        // URL till Google Sheets webhook
        private readonly string apiUrl = "https://script.google.com/macros/s/AKfycbx46bJ844Wp96zuAv1QN71ulh6IGk7xiXY_fKhrusngXCqwPwgZeOEd6rOCQvVDtUx0fQ/exec?path=Test_API&action=read";

        // GET: Employee
        public async Task<IActionResult> Index()
        {
            // Skicka HTTP GET-förfrågan till din API-länk
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                // Läs JSON-svaret som en sträng
                string responseBody = await response.Content.ReadAsStringAsync();

                // Omvandla JSON-svaret till en lista med Employee-objekt
                var json = JObject.Parse(responseBody);
                var dataArray = json["data"] as JArray;
                var employeeList = new List<Employee>();

                foreach (var item in dataArray)
                {
                    var employee = new Employee
                    {
                        EmployeeID = (int)item["EmployeeID"],
                        Name = (string)item["Name"],
                        Project = (string)item["Project"],
                        Start = (DateTime)item["Start"],
                        End = (DateTime)item["End"]
                    };
                    employeeList.Add(employee);
                }

                // Skicka listan till vyn
                return View(employeeList);
            }
        }

        // GET: Employee by ID
        public async Task<IActionResult> Details(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                // Läs JSON-svaret
                string responseBody = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseBody);
                var dataArray = json["data"] as JArray;

                // Hitta anställd baserat på ID
                var employee = dataArray.FirstOrDefault(e => (int)e["EmployeeID"] == id);

                if (employee == null)
                {
                    return NotFound(); // Returnera 404 om ingen anställd hittas
                }

                var selectedEmployee = new Employee
                {
                    EmployeeID = (int)employee["EmployeeID"],
                    Name = (string)employee["Name"],
                    Project = (string)employee["Project"],
                    Start = (DateTime)employee["Start"],
                    End = (DateTime)employee["End"]
                };

                // Skicka anställd till vyn
                return View(selectedEmployee);
            }
        }
    }
}
