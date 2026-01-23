using System;
using System.Web.Mvc;
using InsureGo_MVC.Models.ViewModels;

namespace InsureGo_MVC.Controllers
{
    public class PolicyController : Controller
    {
        [HttpGet]
        public ActionResult Details(string id)
        {
            try
            {
                // TODO: Replace these dummy values with real API call to fetch policy by PolicyNumber
                var vm = new PolicyViewModel
                {
                    PolicyNumber = id,
                    VehicleModel = "—",        // Replace with API data
                    RegistrationNumber = "—",  // Replace with API data
                    PremiumAmount = 0,         // Replace with API data
                    PolicyStatus = "Active"    // Replace with API data
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error fetching policy details: " + ex.Message;
                return View();
            }
        }
    }
}
