
using InsureGo_MVC.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace InsureGo_MVC.Controllers
{
    public class PolicyController : Controller
    {

        private readonly string INSURANCE_API = "https://localhost:44365/api/insurance/";

        // GET & POST POLICY DETAILS

        [HttpGet]
        public async Task<ActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                ViewBag.Error = "Invalid Policy Number.";
                return View((PolicyViewModel)null);
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync( INSURANCE_API + "policybyid/" + id );


                    if (!response.IsSuccessStatusCode)
                    {
                        ViewBag.Error = "Policy not found.";
                        return View((PolicyViewModel)null);
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject<dynamic>(json);

                    if (data == null)
                    {
                        ViewBag.Error = "Invalid policy data.";
                        return View((PolicyViewModel)null);
                    }

                    PolicyViewModel vm = new PolicyViewModel
                    {
                        PolicyNumber = data.PolicyNumber,
                        VehicleModel = data.VehicleModel,
                        RegistrationNumber = data.RegistrationNumber,
                        PremiumAmount = (decimal)data.PremiumAmount,
                        PolicyStatus = data.PolicyStatus
                    };

                    return View(vm);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error fetching policy details: " + ex.Message;
                return View((PolicyViewModel)null);
            }
        }

        [HttpGet]
        public async Task<ActionResult> UserPolicies()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            int userId = Convert.ToInt32(Session["UserId"]);
            List<PolicyViewModel> policies = new List<PolicyViewModel>();

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response =
                        await client.GetAsync(INSURANCE_API + "userpolicies/" + userId);

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        policies = JsonConvert.DeserializeObject<List<PolicyViewModel>>(json);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error loading policies: " + ex.Message;
            }

            return View(policies);
        }

        [HttpGet]
        public async Task<ActionResult> Ticket(string id)
        {
            return await Details(id);
        }
    }
}
