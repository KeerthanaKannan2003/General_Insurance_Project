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
        public ActionResult Login()
        {
            return View(new AdminLoginViewModel());
        }

        [HttpPost]
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

        // ---------- DASHBOARD ----------
        [HttpGet]
        public async Task<ActionResult> Dashboard()
        {
            if (Session["Admin"] == null)
                return RedirectToAction("Login");

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(API_URL);

                // Pending Claims Count
                var claimsResponse = await client.GetAsync("claims");
                if (claimsResponse.IsSuccessStatusCode)
                {
                    var json = await claimsResponse.Content.ReadAsStringAsync();
                    var claims = JsonConvert.DeserializeObject<List<ClaimViewModel>>(json);
                    ViewBag.PendingClaims = claims.Count;
                }
                else
                {
                    ViewBag.PendingClaims = 0;
                }

                // Pending Tickets (demo / optional)
                ViewBag.PendingTickets = 0; // change when ticket API ready
            }

            return View();
        }

        // ---------- SET CLAIM AMOUNT ----------
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
        // ---------- VALIDATE TICKETS ----------
        [HttpGet]
        public async Task<ActionResult> ValidateTicket()
        {
            if (Session["Admin"] == null)
                return RedirectToAction("Login");

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
                await client.PostAsJsonAsync("validateticket", new { TicketId = ticketId });
            }

            TempData["Success"] = "Ticket validated successfully";
            return RedirectToAction("Dashboard");
        }


        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
