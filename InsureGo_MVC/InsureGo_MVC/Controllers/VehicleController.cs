using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using InsureGo_MVC.Models.ViewModels;

namespace InsureGo_MVC.Controllers
{
    public class VehicleController : Controller
    {
        private readonly string VEHICLE_API = "https://localhost:44365/api/vehicle/";

        // GET: Add Vehicle
        [HttpGet]
        public ActionResult Add()
        {
            return View(new VehicleViewModel());
        }

        // POST: Add Vehicle
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(VehicleViewModel vehicle)
        {
            if (!ModelState.IsValid)
                return View(vehicle);

            int vehicleTypeId = vehicle.VehicleType == "2W" ? 1 : 2;

            // Prepare data for API
            var apiVehicle = new
            {
                VehicleTypeId = vehicleTypeId,
                vehicle.Manufacturer,
                vehicle.VehicleModel,
                vehicle.DrivingLicence,
                vehicle.PurchaseDate,
                vehicle.RegistrationNumber,
                vehicle.EngineNumber,
                vehicle.ChassisNumber
            };

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(VEHICLE_API);

                // Call API
                var response = await client.PostAsJsonAsync("add", apiVehicle);
                string apiResult = await response.Content.ReadAsStringAsync();

                // Check for errors
                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "API Error: " + apiResult);
                    return View(vehicle);
                }

                // Parse VehicleId returned from API
                if (!int.TryParse(apiResult, out int vehicleId))
                {
                    ModelState.AddModelError("", "API did not return a valid VehicleId.");
                    return View(vehicle);
                }

                // Save Vehicle info in session
                vehicle.VehicleId = vehicleId;
                Session["VehicleInfo"] = vehicle;

                // Redirect to Select Plan page
                return RedirectToAction("SelectPlan", "Insurance");
            }
        }
    }
}
