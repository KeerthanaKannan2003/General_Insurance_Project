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

        // GET & POST
        [HttpGet]
        public ActionResult Add()
        {
            return View(new VehicleViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(VehicleViewModel vehicle)
        {
            if (!ModelState.IsValid)
                return View(vehicle);

            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            if (!int.TryParse(Session["UserId"].ToString(), out int userId))
            {
                Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            int vehicleTypeId = vehicle.VehicleType == "2W" ? 1 : 2;

            var apiVehicle = new
            {
                VehicleTypeId = vehicleTypeId,
                Manufacturer = vehicle.Manufacturer,
                Model = vehicle.VehicleModel,
                DrivingLicenseNumber = vehicle.DrivingLicence,
                PurchaseDate = vehicle.PurchaseDate,
                RegistrationNumber = vehicle.RegistrationNumber,
                EngineNumber = vehicle.EngineNumber,
                ChassisNumber = vehicle.ChassisNumber,
                UserId = userId
            };

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(VEHICLE_API);
                client.Timeout = TimeSpan.FromSeconds(30);

                HttpResponseMessage response;
                try
                {
                    response = await client.PostAsJsonAsync("add", apiVehicle);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "API call failed: " + ex.Message);
                    return View(vehicle);
                }

                if (!response.IsSuccessStatusCode)
                {
                    string apiError = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", "API Error: " + apiError);
                    return View(vehicle);
                }

                int vehicleId = 0;
                try
                {
                    vehicleId = await response.Content.ReadAsAsync<int>();
                }
                catch
                {
                    ModelState.AddModelError("", "Failed to parse Vehicle ID from API response.");
                    return View(vehicle);
                }

                vehicle.VehicleId = vehicleId;
                vehicle.VehicleTypeId = vehicleTypeId;
                vehicle.UserId = userId;
                Session["VehicleInfo"] = vehicle;

                return RedirectToAction("SelectPlan", "Insurance");
            }
        }


        // GET: All Vehicles 
        [HttpGet]
        public async Task<ActionResult> All()
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(VEHICLE_API);

                var response = await client.GetAsync("all");
                if (!response.IsSuccessStatusCode)
                {
                    string apiError = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", "API Error: " + apiError);
                    return View("Error");
                }

                var vehicles = await response.Content.ReadAsAsync<object>();
                return View(vehicles); 
            }
        }
    }
}