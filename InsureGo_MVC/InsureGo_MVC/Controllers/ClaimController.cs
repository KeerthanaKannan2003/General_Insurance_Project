using InsureGo_MVC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace InsureGo_MVC.Controllers
{
    public class ClaimController : Controller
    {
        // ===================== CREATE CLAIM =====================
        [HttpGet]
        public ActionResult Create()
        {
            return View(new ClaimViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ClaimViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                // TODO: Lookup PolicyId by PolicyNumber via API
                // TODO: Insert into Claims table via API

                TempData["Success"] = "Claim submitted. Our team will contact you.";
                return RedirectToAction("History");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error submitting claim: " + ex.Message;
                return View(model);
            }
        }

        // ===================== CLAIM HISTORY =====================
        [HttpGet]
        public ActionResult History()
        {
            try
            {
                // TODO: Query ClaimHistory via API and pass to view
                var claims = new List<ClaimViewModel>(); // Replace with API data

                return View(claims);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error loading claim history: " + ex.Message;
                return View(new List<ClaimViewModel>());
            }
        }
    }
}
