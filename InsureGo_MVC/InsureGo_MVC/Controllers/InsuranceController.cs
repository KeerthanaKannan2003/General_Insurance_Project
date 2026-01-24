using System.Web.Mvc;
using InsureGo_MVC.Models.ViewModels;

namespace InsureGo_MVC.Controllers
{
    public class InsuranceController : Controller
    {
        // STEP 2: Select Plan (GET)
        [HttpGet]
        public ActionResult SelectPlan()
        {
            var vehicle = Session["VehicleInfo"] as VehicleViewModel;
            if (vehicle == null)
                return RedirectToAction("Add", "Vehicle"); // Redirect back if session missing

            var model = new InsuranceViewModel
            {
                VehicleType = vehicle.VehicleType,
                VehicleModel = vehicle.VehicleModel,
                RegistrationNumber = vehicle.RegistrationNumber
            };

            return View(model);
        }


        // STEP 2: Select Plan (POST)
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SelectPlan(InsuranceViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Save selected plan in session
            Session["InsurancePlan"] = model;

            return RedirectToAction("Pay", "Payment");
        }
    }
}
