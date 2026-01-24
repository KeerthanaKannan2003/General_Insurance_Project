using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using InsureGo_MVC.Models.ViewModels;

namespace InsureGo_MVC.Controllers
{
    public class VehicleController : Controller
    {
        // Replace with your actual Web API base URL and port
        private readonly string VEHICLE_API = "https://localhost:44365/api/vehicle/";

        // GET: Add Vehicle
        [HttpGet]
        public ActionResult Add()
        {
            return View(new VehicleViewModel());
        }

        // POST: Add Vehicle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(VehicleViewModel vehicle)
        {
            if (!ModelState.IsValid)
                return View(vehicle);

            int vehicleTypeId = vehicle.VehicleType == "2W" ? 1 : 2;

            // Prepare data for API
            var apiVehicle = new
            {
                VehicleTypeId = vehicleTypeId,
                Manufacturer = vehicle.Manufacturer,
                Model = vehicle.VehicleModel,                // ✅ matches DB column
                DrivingLicenseNumber = vehicle.DrivingLicence, // ✅ matches DB column
                PurchaseDate = vehicle.PurchaseDate,
                RegistrationNumber = vehicle.RegistrationNumber,
                EngineNumber = vehicle.EngineNumber,
                ChassisNumber = vehicle.ChassisNumber
            };

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(VEHICLE_API);

                // Call API
                var response = await client.PostAsJsonAsync("add", apiVehicle);

                if (!response.IsSuccessStatusCode)
                {
                    string apiError = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", "API Error: " + apiError);
                    return View(vehicle);
                }

                // ✅ Deserialize VehicleId correctly
                var vehicleId = await response.Content.ReadAsAsync<int>();

                // Save Vehicle info in session
                vehicle.VehicleId = vehicleId;
                Session["VehicleInfo"] = vehicle;

                // Redirect to Select Plan page
                return RedirectToAction("SelectPlan", "Insurance");
            }
        }
    }
}
