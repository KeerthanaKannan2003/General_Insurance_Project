using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using InsureGo_MVC.Models.ViewModels;

namespace InsureGo_MVC.Controllers
{
    public class PremiumController : Controller
    {
        private string apiUrl = "https://localhost:44365/api/premium"; // Your API base URL

        [HttpGet]
        public ActionResult Calculate()
        {
            return View(new PremiumViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Calculate(PremiumViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                using (var client = new HttpClient())
                {
                    string url = $"{apiUrl}/calculate/{model.VehicleTypeId}/{model.VehicleAge}";
                    var response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var premium = await response.Content.ReadAsAsync<decimal>();
                        model.PremiumAmount = premium;
                        TempData["Success"] = "Premium calculated successfully!";
                    }
                    else
                    {
                        ViewBag.Error = "Failed to calculate premium from API";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error: " + ex.Message;
            }

            return View(model);
        }
    }
}
