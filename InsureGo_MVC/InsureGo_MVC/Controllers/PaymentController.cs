using InsureGo_MVC.Models.ViewModels;
using System;
using System.Net.Http;
using System.Web.Mvc;

public class PaymentController : Controller
{
    [HttpGet]
    public ActionResult Pay()
    {
        if (Session["InsurancePlan"] == null)
            return RedirectToAction("Buy", "Insurance");

        var plan = Session["InsurancePlan"] as InsuranceViewModel;

        decimal amount = plan.PlanType == "Comprehensive"
                            ? 5000 * plan.Duration
                            : 3000 * plan.Duration;

        var model = new PaymentViewModel
        {
            Amount = amount
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Pay(PaymentViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        using (HttpClient client = new HttpClient())
        {
            client.BaseAddress = new Uri("https://localhost:44365/api/payment/");
            var response = client.PostAsJsonAsync("makepayment", model).Result;

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Payment failed");
                return View(model);
            }

            string policyNumber = response.Content.ReadAsStringAsync().Result;

            TempData["Success"] = "Payment Successful!";
            TempData["PolicyNumber"] = policyNumber;
        }

        return RedirectToAction("Index", "Home");
    }
}
