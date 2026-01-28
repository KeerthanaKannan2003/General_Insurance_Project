using InsureGo_MVC.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Web.Mvc;

public class ClaimController : Controller
{
    private readonly string API = "https://localhost:44365/api/claim/";


    // GET & POST CLAIM CREATE

    [HttpGet]
    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(ClaimInsuranceViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        using (HttpClient client = new HttpClient())
        {
            var payload = new
            {
                PolicyNumber = model.PolicyNumber,
                MobileNumber = model.MobileNumber,
                ClaimReason = model.ClaimReason
            };

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(API + "raise", content);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Invalid policy details or claim failed.";
                return View(model);
            }
        }

        TempData["Success"] = "Claim submitted successfully";
        return RedirectToAction("Index", "Home");
    }
}

