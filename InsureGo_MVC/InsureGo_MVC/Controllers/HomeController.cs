using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Mvc;

namespace InsureGo_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly string POLICY_API = "https://localhost:44365/api/policy/";

        public ActionResult Index()
        {
            if (Session["UserName"] == null)
                return RedirectToAction("Login", "Account");

<<<<<<< HEAD
        public ActionResult About()
        {
=======
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(POLICY_API);
                var response = client.GetAsync("userpolicies/" + Session["UserId"]).Result;

                if (response.IsSuccessStatusCode)
                {
                    var json = response.Content.ReadAsStringAsync().Result;
                    ViewBag.Policies = JsonConvert.DeserializeObject<List<dynamic>>(json);
                }
            }

            ViewBag.Success = TempData["Success"];
            ViewBag.PolicyNumber = TempData["PolicyNumber"];
>>>>>>> 9c8b818 (initial commit)
            return View();
        }
    }
}
