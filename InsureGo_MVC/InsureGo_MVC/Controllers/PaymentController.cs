using InsureGo_MVC.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Web.Mvc;

public class PaymentController : Controller
{
    [HttpGet]
    public ActionResult Pay()
    {
        if (Session["InsurancePlan"] == null || Session["VehicleInfo"] == null)
            return RedirectToAction("Buy", "Insurance");

        var plan = Session["InsurancePlan"] as InsuranceViewModel;

        decimal amount = plan.PlanType == "Comprehensive"
                            ? 5000 * plan.Duration
                            : 3000 * plan.Duration;

        return View(new PaymentViewModel { Amount = amount });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Pay(PaymentViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var plan = Session["InsurancePlan"] as InsuranceViewModel;
        var vehicle = Session["VehicleInfo"] as VehicleViewModel;

        // 🔹 Build policy object for API
        var policy = new
        {
            UserId = Session["UserId"],
            VehicleId = vehicle.VehicleId,
            PlanId = plan.PlanId,
            DurationId = plan.DurationId,
            PremiumAmount = model.Amount
        };

        using (HttpClient client = new HttpClient())
        {
            client.BaseAddress = new Uri("https://localhost:44365/api/insurance/");

            var content = new StringContent(
                JsonConvert.SerializeObject(policy),
                Encoding.UTF8,
                "application/json"
            );

            var response = client.PostAsync("createpolicy", content).Result;

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Policy creation failed");
                return View(model);
            }

            // ✅ READ JSON RESPONSE
            var json = response.Content.ReadAsStringAsync().Result;
            dynamic data = JsonConvert.DeserializeObject(json);

            string policyNumber = data.PolicyNumber;

            // ✅ STORE FOR HOME PAGE
            TempData["Success"] = "Payment & Policy Created Successfully!";
            TempData["PolicyNumber"] = policyNumber;
        }

        // ✅ Redirect to HOME
        return RedirectToAction("Index", "Home");
    }
}
