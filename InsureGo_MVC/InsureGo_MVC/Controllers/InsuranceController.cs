
using InsureGo_MVC.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace InsureGo_MVC.Controllers
{
    public class InsuranceController : Controller
    {
        private readonly string INSURANCE_API = "https://localhost:44365/api/insurance/";

        // GET & POST SELECT PLAN

        [HttpGet]
        public async Task<ActionResult> SelectPlan()
        {
            var vehicle = Session["VehicleInfo"] as VehicleViewModel;
            if (vehicle == null)
                return RedirectToAction("Add", "Vehicle");

            var model = new InsuranceViewModel
            {
                VehicleType = vehicle.VehicleType,
                VehicleModel = vehicle.VehicleModel,
                RegistrationNumber = vehicle.RegistrationNumber
            };

            ViewBag.Plans = await LoadPlans();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> SelectPlan(InsuranceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Plans = await LoadPlans();
                return View(model);
            }

            var vehicle = Session["VehicleInfo"] as VehicleViewModel;
            if (vehicle == null)
            {
                ViewBag.Error = "Vehicle info missing.";
                return RedirectToAction("Add", "Vehicle");
            }

            decimal calculatedPremium = 0;
            int generatedPolicyId = 0;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(INSURANCE_API);
                client.Timeout = TimeSpan.FromSeconds(30);

                int vehicleAge = DateTime.Now.Year - vehicle.PurchaseDate.Year;
                if (vehicleAge < 0) vehicleAge = 0; 

                var premiumResponse = await client.GetAsync($"calculatepremium?vehicleTypeId={vehicle.VehicleTypeId}&vehicleAge={vehicleAge}");

                if (!premiumResponse.IsSuccessStatusCode)
                {
                    string pError = await premiumResponse.Content.ReadAsStringAsync();
                    ViewBag.Error = "Premium calculation failed: " + pError;
                    ViewBag.Plans = await LoadPlans();
                    return View(model);
                }

                var premiumJson = await premiumResponse.Content.ReadAsStringAsync();
                calculatedPremium = JsonConvert.DeserializeObject<decimal>(premiumJson);

                if (calculatedPremium <= 0)
                {
                    ViewBag.Error = "Calculated premium is zero. Please check vehicle details.";
                    ViewBag.Plans = await LoadPlans();
                    return View(model);
                }


                var newPolicy = new
                {
                    UserId = vehicle.UserId,
                    InsuranceTypeId = 1, 
                    VehicleId = vehicle.VehicleId,
                    PlanId = model.PlanId,
                    DurationId = model.DurationYears, 
                    PremiumAmount = calculatedPremium
                };

                var policyResponse = await client.PostAsJsonAsync("buy", newPolicy);

                if (!policyResponse.IsSuccessStatusCode)
                {
                    string error = await policyResponse.Content.ReadAsStringAsync();
                    ViewBag.Error = "Policy creation failed: " + error;
                    ViewBag.Plans = await LoadPlans();
                    return View(model);
                }

                generatedPolicyId = await policyResponse.Content.ReadAsAsync<int>();


                var plans = await LoadPlans();
                var selectedPlan = plans.FirstOrDefault(p => p.PlanId == model.PlanId);
                string planName = selectedPlan != null ? selectedPlan.PlanName : "General Insurance";

                Session["PlanInfo"] = new PlanSelectionViewModel
                {
                    PolicyId = generatedPolicyId,
                    PlanName = planName,
                    PremiumAmount = calculatedPremium,
                    DurationYears = model.DurationYears,
                    VehicleModel = vehicle.VehicleModel,
                    RegistrationNumber = vehicle.RegistrationNumber
                };
            }

            return RedirectToAction("MakePayment", "Payment");
        }


        private async Task<List<InsurancePlanViewModel>> LoadPlans()
        {
            List<InsurancePlanViewModel> plans = new List<InsurancePlanViewModel>();

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(INSURANCE_API);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync("plans");
                if (response.IsSuccessStatusCode)
                {
                    plans = await response.Content.ReadAsAsync<List<InsurancePlanViewModel>>();
                }
            }

            return plans;
        }


        // RENEW POLICY

        [HttpGet]
        public ActionResult Renew()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Renew(RenewPolicyViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(INSURANCE_API);
                var response = await client.GetAsync("renewaldetails/" + model.PolicyNumber);

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Policy is not expired after policy expired you can renew.";
                    return View(model);
                }

                var json = await response.Content.ReadAsStringAsync();
                var vehicle = JsonConvert.DeserializeObject<VehicleViewModel>(json);


                Session["VehicleInfo"] = vehicle;
            }

            return RedirectToAction("SelectPlan");
        }

        [HttpGet]
        public ActionResult ClaimHistory(string policyNumber)
        {
            var claims = new List<ClaimHistoryViewModel>();

            using (HttpClient client = new HttpClient())
            {
                var response = client
                    .GetAsync("https://localhost:44365/api/claim/history/" + policyNumber)
                    .Result;

                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    claims = JsonConvert.DeserializeObject<List<ClaimHistoryViewModel>>(json);
                }
            }

            ViewBag.PolicyNumber = policyNumber;
            return View(claims);
        }

    }
}
