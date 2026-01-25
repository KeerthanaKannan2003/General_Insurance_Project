using InsureGo_MVC.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace InsureGo_MVC.Controllers
{
    public class PolicyController : Controller
    {
        // ✅ CORRECT BASE URL
        private readonly string INSURANCE_API = "https://localhost:44365/api/insurance/";

        // ============================
        // POLICY DETAILS PAGE
        // ============================
        [HttpGet]
        public async Task<ActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index", "Home");

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // ✅ CALLS: api/insurance/policy/{policyNumber}
                    var response = await client.GetAsync(INSURANCE_API + "policy/" + id);

                    if (!response.IsSuccessStatusCode)
                    {
                        ViewBag.Error = "Policy not found";
                        return View();
                    }

                    var json = await response.Content.ReadAsStringAsync();

                    var data = JsonConvert.DeserializeObject<dynamic>(json);

                    var vm = new PolicyViewModel
                    {
                        PolicyNumber = data.PolicyNumber,
                        VehicleModel = data.VehicleModel,
                        RegistrationNumber = data.RegistrationNumber,
                        PremiumAmount = (decimal)data.PremiumAmount,
                        PolicyStatus = data.PolicyStatus
                    };

                    return View(vm);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error fetching policy details: " + ex.Message;
                return View();
            }
        }
    }
}
