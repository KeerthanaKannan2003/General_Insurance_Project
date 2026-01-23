using System.Web.Mvc;
using InsureGo_MVC.Models.ViewModels;

namespace InsureGo_MVC.Controllers
{
    public class VehicleController : Controller
    {
        [HttpGet]
        public ActionResult Add()
        {
            return View(new VehicleViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Add(VehicleViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Save vehicle info temporarily in Session (or call API to save)
            Session["VehicleInfo"] = model;

            // Redirect to Plans page
            return RedirectToAction("SelectPlan", "Insurance");
        }
    }
}
