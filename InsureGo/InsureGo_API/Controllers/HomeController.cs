using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace InsureGo_API.Controllers
{
    public class HomeController : Controller
    {
        private readonly string POLICY_API = "https://localhost:44365/api/insurance/"; 
        private readonly string CLAIM_API = "https://localhost:44365/api/claim/";
        public ActionResult Index()
        {
            if (Session["UserName"] == null || Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            //  LOAD POLICIES (Home)
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(POLICY_API);
                var response = client.GetAsync("userpolicies/" + Session["UserId"]).Result;

                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    ViewBag.Policies = JsonConvert.DeserializeObject<List<dynamic>>(json);
                }
                else
                {
                    ViewBag.Policies = new List<dynamic>();
                }
            }

            //  LOAD CLAIMS 
            var allClaims = new List<dynamic>();

            if (ViewBag.Policies != null)
            {
                foreach (var policy in ViewBag.Policies)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var res = client.GetAsync(CLAIM_API + "history/" + policy.PolicyId).Result;

                        if (res.IsSuccessStatusCode)
                        {
                            var json = res.Content.ReadAsStringAsync().Result;
                            var claims = JsonConvert.DeserializeObject<List<dynamic>>(json);
                            allClaims.AddRange(claims);   
                        }
                    }
                }
            }

            ViewBag.Claims = allClaims;  

            ViewBag.Success = TempData["Success"];
            ViewBag.PolicyNumber = TempData["PolicyNumber"];

            return View();
        }


    }
}
