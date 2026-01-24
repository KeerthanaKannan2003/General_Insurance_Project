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

        public ActionResult Index()
        {
            if (Session["UserName"] == null || Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            // Load user policies
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

            // Payment success message
            ViewBag.Success = TempData["Success"];
            ViewBag.PolicyNumber = TempData["PolicyNumber"];

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
        public ActionResult Help() => View(); 
        public ActionResult FAQ() => View();
        public ActionResult Home() => View();
        public ActionResult Error() => View();
        public ActionResult Contact() => View();
    }
}
