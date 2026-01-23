using InsureGo_MVC.Models.ViewModels;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace InsureGo_MVC.Controllers
{
    public class InsuranceController : Controller
    {
        private readonly string INSURANCE_API = "https://localhost:44365/api/insurance/";

        // STEP 1: Vehicle Details (GET)
        [HttpGet]
        public ActionResult Buy()
        {
            return View(new VehicleViewModel());
        }

        // STEP 1: Vehicle Details (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Buy(VehicleViewModel vehicle)
        {
            if (!ModelState.IsValid)
                return View(vehicle);

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(INSURANCE_API);
                var response = await client.PostAsJsonAsync("addvehicle", vehicle);

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Vehicle save failed");
                    return View(vehicle);
                }

                int vehicleId = int.Parse(await response.Content.ReadAsStringAsync());
                vehicle.VehicleId = vehicleId;

                Session["VehicleInfo"] = vehicle;
                return RedirectToAction("SelectPlan");
            }
        }

        // STEP 2: Select Plan (GET)
        [HttpGet]
        public ActionResult SelectPlan()
        {
            var vehicle = Session["VehicleInfo"] as VehicleViewModel;
            if (vehicle == null)
                return RedirectToAction("Buy");

            var model = new InsuranceViewModel
            {
                VehicleType = vehicle.VehicleType,
                VehicleModel = vehicle.VehicleModel,
                RegistrationNumber = vehicle.RegistrationNumber
            };

            return View(model);
        }

        // STEP 2: Select Plan (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SelectPlan(InsuranceViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            Session["InsurancePlan"] = model;
            return RedirectToAction("Pay", "Payment");
        }
    }
}
