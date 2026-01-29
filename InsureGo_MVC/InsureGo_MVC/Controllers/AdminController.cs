using InsureGo_MVC.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace InsureGo_MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly string API_URL = "https://localhost:44365/api/admin/";

        // GET & POST ADMIN LOGIN
        [HttpGet]
        public ActionResult Login()
        {
            return View(new AdminLoginViewModel());
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(AdminLoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (model.Username == "admin" && model.Password == "admin123")
            {
                Session["Admin"] = model.Username;
                return RedirectToAction("Dashboard");
            }

            ModelState.AddModelError("", "Invalid credentials");
            return View(model);
        }


        // GET & POST DASHBOARD

        [HttpGet]
        public async Task<ActionResult> Dashboard()
        {
            if (Session["Admin"] == null)
                return RedirectToAction("Login");

            List<HomeClaimViewModel> claims = new List<HomeClaimViewModel>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(API_URL);

                var claimsResponse = await client.GetAsync("claims");
                if (claimsResponse.IsSuccessStatusCode)
                {
                    var json = await claimsResponse.Content.ReadAsStringAsync();
                    claims = JsonConvert.DeserializeObject<List<HomeClaimViewModel>>(json);
                }
            }
            ViewBag.PendingClaims = claims.Count;
            ViewBag.PendingTickets = 0; 
            return View(claims); 
                                 
        }

        // GET & POST SETCLAIMAMOUNT
        [HttpGet]
        public ActionResult SetClaimAmount(int claimId)
        {
            if (Session["Admin"] == null)
                return RedirectToAction("Login");

            return View(new SetClaimAmountViewModel { ClaimId = claimId });
        }

        [HttpPost]
        public async Task<ActionResult> SetClaimAmount(SetClaimAmountViewModel model)
        {
            if (Session["Admin"] == null)
                return RedirectToAction("Login");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(API_URL);

                var response = await client.PostAsJsonAsync("setclaimamount", model);
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Claim approved successfully";
                    return RedirectToAction("Dashboard");
                }
            }

            TempData["Error"] = "Failed to approve claim";
            return View(model);
        }



        // APPROVE CLAIM AMOUNT

        [HttpPost]
        public async Task<ActionResult> ApproveClaim(int claimId, decimal claimAmount)
        {
            if (Session["Admin"] == null)
                return RedirectToAction("Login");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(API_URL);

                var postData = new
                {
                    ClaimId = claimId,
                    Amount = claimAmount
                };

                var json = JsonConvert.SerializeObject(postData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("setclaimamount", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Claim approved successfully";
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    TempData["Error"] = "Failed to approve claim.";
                    return RedirectToAction("Dashboard");
                }
            }
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
