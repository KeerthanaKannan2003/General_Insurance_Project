using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using InsureGo_MVC.Models.ViewModels;
using Newtonsoft.Json;

namespace InsureGo_MVC.Controllers
{
    public class PaymentController : Controller
    {
        private readonly string PAYMENT_API = "https://localhost:44365/api/payment/";

        
        [HttpGet]
        public ActionResult MakePayment()
        {
            var plan = Session["PlanInfo"] as PlanSelectionViewModel;
            if (plan == null)
            {
                ViewBag.Error = "Plan info missing.";
                return RedirectToAction("SelectPlan", "Insurance");
            }

            return View(plan); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MakePaymentConfirmed()
        {
            var plan = Session["PlanInfo"] as PlanSelectionViewModel;
            if (plan == null)
            {
                TempData["Error"] = "Session expired. Please reselect your plan.";
                return RedirectToAction("SelectPlan", "Insurance");
            }

            var payment = new
            {
                PolicyId = plan.PolicyId,
                Amount = plan.PremiumAmount
            };

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(PAYMENT_API);
                client.Timeout = TimeSpan.FromSeconds(30);

                var response = await client.PostAsJsonAsync("makepayment", payment);

                if (!response.IsSuccessStatusCode)
                {
                    string error = await response.Content.ReadAsStringAsync();
                    ViewBag.Error = "Payment failed: " + error;
                    return View("MakePayment", plan);
                }

                var policyNumber = await response.Content.ReadAsAsync<string>();

                TempData["ShowPaymentPopup"] = true;
                TempData["PolicyNumber"] = policyNumber;

                return RedirectToAction("Index", "Home");
            }
        }
    }
}