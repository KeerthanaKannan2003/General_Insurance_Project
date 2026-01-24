using InsureGo_MVC.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace InsureGo_MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly string API_URL = "https://localhost:44365/api/admin/";

        // ---------- LOGIN ----------
        [HttpGet]
        public ActionResult Login() => View(new AdminLoginViewModel());

        [HttpPost]
        public ActionResult Login(AdminLoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Demo: Replace with real API call for admin auth
            if (model.Username == "admin" && model.Password == "admin123")
            {
                Session["Admin"] = model.Username;
                return RedirectToAction("Dashboard");
            }

            ModelState.AddModelError("", "Invalid credentials");
            return View(model);
        }

        // ---------- DASHBOARD ----------
        [HttpGet]
        public async Task<ActionResult> Dashboard()
        {
            if (Session["Admin"] == null) return RedirectToAction("Login");

            List<ClaimViewModel> claims = new List<ClaimViewModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(API_URL);
                var response = await client.GetAsync("claims");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    claims = JsonConvert.DeserializeObject<List<ClaimViewModel>>(json);
                }
            }

            return View(claims); // list of claims to show in dashboard
        }

        // ---------- SET CLAIM AMOUNT ----------
        [HttpGet]
        public ActionResult SetClaimAmount(int claimId)
        {
            return View(new SetClaimAmountViewModel { ClaimId = claimId });
        }

        [HttpPost]
        public async Task<ActionResult> SetClaimAmount(SetClaimAmountViewModel model)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(API_URL);
                var response = await client.PostAsJsonAsync("setclaimamount", model);
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = $"Claim #{model.ClaimId} updated.";
                    return RedirectToAction("Dashboard");
                }
            }

            TempData["Error"] = "Failed to update claim";
            return View(model);
        }

        // ---------- VALIDATE TICKET ----------
        [HttpGet]
        public async Task<ActionResult> ValidateTicket()
        {
            List<dynamic> tickets = new List<dynamic>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(API_URL);
                var response = await client.GetAsync("tickets");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    tickets = JsonConvert.DeserializeObject<List<dynamic>>(json);
                }
            }

            return View(tickets);
        }

        [HttpPost]
        public async Task<ActionResult> ValidateTicket(int ticketId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(API_URL);
                var response = await client.PostAsJsonAsync("validateticket", new { TicketId = ticketId });
            }

            TempData["Success"] = $"Ticket #{ticketId} validated.";
            return RedirectToAction("Dashboard");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}