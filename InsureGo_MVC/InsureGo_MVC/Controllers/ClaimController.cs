using InsureGo_MVC.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Web.Mvc;

public class ClaimController : Controller
{
    private readonly string API = "https://localhost:44365/api/claim/";

    [HttpGet]
    public ActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(ClaimInsuranceViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        using (HttpClient client = new HttpClient())
        {
            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = client.PostAsync(API + "raise", content).Result;

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
