using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Mvc;

namespace InsureGo_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly string POLICY_API = "https://localhost:44365/api/insurance/";
        private readonly string CLAIM_API = "https://localhost:44365/api/claim/"; 

        public ActionResult Index()
        {
            if (Session["UserName"] == null || Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            

            // LOADING POLICIES DETAILS

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


            // LOADING CLAIM DEATILS

            var allClaims = new List<dynamic>();

            if (ViewBag.Policies != null)
            {
                foreach (var policy in ViewBag.Policies)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var res = client.GetAsync(
                            CLAIM_API + "history/" + (int)policy.PolicyId).Result;

                        if (res.IsSuccessStatusCode)
                        {
                            var json = res.Content.ReadAsStringAsync().Result;
                            var policyClaims = JsonConvert.DeserializeObject<List<dynamic>>(json);
                            if (policyClaims != null)
                            {
                                allClaims.AddRange(policyClaims);
                            }
                        }
                    }
                }
            }
            ViewBag.Claims = allClaims;

            ViewBag.Success = TempData["Success"];
            ViewBag.PolicyNumber = TempData["PolicyNumber"];

            return View();
        }

        public ActionResult About() => View();
        public ActionResult Help() => View();
        public ActionResult FAQ() => View();
        public ActionResult Home() => View();
        public ActionResult Error() => View();
        public ActionResult Contact() => View();
    }
}
